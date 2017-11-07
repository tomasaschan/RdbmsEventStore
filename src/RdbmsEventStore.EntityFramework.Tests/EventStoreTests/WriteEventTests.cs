using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RdbmsEventStore.EntityFramework.Tests.Infrastructure;
using RdbmsEventStore.EntityFramework.Tests.TestData;
using Moq;
using Xunit;


namespace RdbmsEventStore.EntityFramework.Tests.EventStoreTests
{
    public class WriteEventTests : EventStoreTestBase<Guid, Guid, GuidEvent, IEventMetadata<Guid>, GuidGuidPersistedEvent>
    {
        public WriteEventTests(EventStoreFixture<Guid, Guid, GuidEvent, IEventMetadata<Guid>, GuidGuidPersistedEvent> fixture, AssemblyInitializerFixture initializer) : base(fixture, initializer)
        {
        }

        [Fact]
        public async Task CommittingEventStoresEventInContext()
        {
            var store = _fixture.BuildEventStore(_dbContext);
            await store.Append(Guid.NewGuid(), 0, new[] { new FooEvent { Foo = "Bar" } });
            Assert.Equal(1, await _dbContext.Events.CountAsync());
        }

        [Fact]
        public async Task CommittingWithOutOfSyncDataThrowsConflictException()
        {
            var store = _fixture.BuildEventStore(_dbContext);
            var stream = Guid.NewGuid();
            _dbContext.Events.AddRange(_fixture.EventFactory.Create(stream, 0, new[] { new FooEvent { Foo = "Bar" } }).Select(_fixture.EventSerializer.Serialize));
            await _dbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<ConflictException>(() => store.Append(stream, 0, new[] { new FooEvent { Foo = "Qux" } }));
        }

        [Fact]
        public async Task CommittingNoEventsExitsEarly() {
            var context = new Mock<EventStoreContext<GuidGuidPersistedEvent>>(MockBehavior.Strict);
            var set = new Mock<DbSet<GuidGuidPersistedEvent>>(MockBehavior.Strict);
            context.Setup(c => c.Set<GuidGuidPersistedEvent>()).Returns(set.Object);
            var stream = Guid.NewGuid();

            var store = _fixture.BuildEventStore(context.Object);

            try {
                await store.Append(stream, 0, new object[] { });
            } catch (NotImplementedException) {
                // Thrown by the mock DbSet if we try to query for existing events
                // This indicates that we didn't exit early

                Assert.False(true, "Expected to exit early, but apparently didn't.");
            }
        }

        [Fact]
        public async Task CommittingMultipleEventsStoresAllEventsInContext()
        {
            Assert.Empty(await _dbContext.Events.ToListAsync());

            var store = _fixture.BuildEventStore(_dbContext);

            var events = new[] { new FooEvent { Foo = "Foo" }, new FooEvent { Foo = "Bar" } };
            await store.Append(Guid.NewGuid(), 0, events);

            Assert.Equal(2, await _dbContext.Events.CountAsync());
        }

        [Fact]
        public async Task CommittingMultipleEventsStoresEventsInOrder()
        {
            Assert.Empty(await _dbContext.Events.ToListAsync());

            var store = _fixture.BuildEventStore(_dbContext);

            var events = new object[] { new FooEvent { Foo = "Foo" }, new BarEvent { Bar = "Bar" } };
            await store.Append(Guid.NewGuid(), 0, events);

            Assert.Collection(await _dbContext.Events.OrderBy(e => e.Version).ToListAsync(),
                foo => Assert.Equal(typeof(FooEvent), _fixture.EventRegistry.TypeFor(foo.Type)),
                bar => Assert.Equal(typeof(BarEvent), _fixture.EventRegistry.TypeFor(bar.Type)));
        }

        [Fact]
        public async Task CommittingMultipleEventsIncrementsVersionForEachEvent()
        {
            Assert.Empty(await _dbContext.Events.ToListAsync());

            var store = _fixture.BuildEventStore(_dbContext);
            var events = new object[] { new FooEvent { Foo = "Foo" }, new BarEvent { Bar = "Bar" } };
            await store.Append(Guid.NewGuid(), 0, events);

            var storedEvents = await _dbContext.Events.OrderBy(e => e.Timestamp).ToListAsync();
            Assert.Collection(storedEvents,
                foo => Assert.Equal(1, foo.Version),
                bar => Assert.Equal(2, bar.Version));
        }
    }
}
