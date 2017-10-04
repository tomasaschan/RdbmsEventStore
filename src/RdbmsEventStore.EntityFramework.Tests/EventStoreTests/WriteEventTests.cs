using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RdbmsEventStore.EntityFramework.Tests.Infrastructure;
using RdbmsEventStore.EntityFramework.Tests.TestData;
using Xunit;


namespace RdbmsEventStore.EntityFramework.Tests.EventStoreTests
{
    public class WriteEventTests : EventStoreTestBase<Guid, Guid, GuidGuidEvent>
    {
        public WriteEventTests(EventStoreFixture<Guid, Guid, GuidGuidEvent> fixture, AssemblyInitializerFixture initializer) : base(fixture, initializer)
        {
        }

        [Fact]
        public async Task CommittingEventStoresEventInContext()
        {
            var store = _fixture.BuildEventStore(_dbContext);
            await store.Commit(Guid.NewGuid(), 0, new FooEvent { Foo = "Bar" });
            Assert.Equal(1, await _dbContext.Events.CountAsync());
        }

        [Fact]
        public async Task CommittingWithOutOfSyncDataThrowsConflictException()
        {
            var store = _fixture.BuildEventStore(_dbContext);
            var stream = Guid.NewGuid();
            _dbContext.Events.AddRange(_fixture.EventFactory.Create(stream, 0, new FooEvent { Foo = "Bar" }));
            await _dbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<ConflictException>(() => store.Commit(stream, 0, new FooEvent { Foo = "Qux" }));
        }

        [Fact]
        public async Task CommittingMultipleEventsStoresAllEventsInContext()
        {
            Assert.Empty(await _dbContext.Events.ToListAsync());

            var store = _fixture.BuildEventStore(_dbContext);

            await store.Commit(Guid.NewGuid(), 0, new FooEvent { Foo = "Foo" }, new FooEvent { Foo = "Bar" });

            Assert.Equal(2, await _dbContext.Events.CountAsync());
        }

        [Fact]
        public async Task CommittingMultipleEventsStoresEventsInOrder()
        {
            Assert.Empty(await _dbContext.Events.ToListAsync());

            var store = _fixture.BuildEventStore(_dbContext);

            await store.Commit(Guid.NewGuid(), 0, new FooEvent { Foo = "Foo" }, new BarEvent { Bar = "Bar" });

            Assert.Collection(await _dbContext.Events.OrderBy(e => e.Version).ToListAsync(),
                foo => Assert.Equal(typeof(FooEvent), _fixture.EventRegistry.TypeFor(foo.Type)),
                bar => Assert.Equal(typeof(BarEvent), _fixture.EventRegistry.TypeFor(bar.Type)));
        }

        [Fact]
        public async Task CommittingMultipleEventsIncrementsVersionForEachEvent()
        {
            Assert.Empty(await _dbContext.Events.ToListAsync());

            var store = _fixture.BuildEventStore(_dbContext);

            await store.Commit(Guid.NewGuid(), 0, new FooEvent { Foo = "Foo" }, new BarEvent { Bar = "Bar" });

            var events = await _dbContext.Events.OrderBy(e => e.Timestamp).ToListAsync();
            Assert.Collection(events,
                foo => Assert.Equal(1, foo.Version),
                bar => Assert.Equal(2, bar.Version));
        }
    }
}