using System.Linq;
using System.Threading.Tasks;
using RdbmsEventStore.EntityFramework.Tests.Infrastructure;
using RdbmsEventStore.EntityFramework.Tests.TestData;
using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.EventStoreTests
{
    public class QueryEventsTests : EventStoreTestBase<long, string, StringEvent, IEventMetadata<string>, LongStringPersistedEvent>
    {
        public QueryEventsTests(EventStoreFixture<long, string, StringEvent, IEventMetadata<string>, LongStringPersistedEvent> fixture, AssemblyInitializerFixture initializer) : base(fixture, initializer)
        {
            var stream1 = _fixture.EventFactory.Create("stream-1", 0, new object[] {
                    new FooEvent { Foo = "Foo" },
                    new BarEvent { Bar = "Bar" },
                    new FooEvent { Foo = "Baz" }
                })
                .Select(_fixture.EventSerializer.Serialize);
            var stream2 = _fixture.EventFactory.Create("stream-2", 0, new object[] {
                    new FooEvent { Foo = "Boo" },
                    new BarEvent { Bar = "Far" }
                })
                .Select(_fixture.EventSerializer.Serialize);

            _dbContext.Events.AddRange(stream1);
            _dbContext.Events.AddRange(stream2);
            _dbContext.SaveChanges();
        }

        [Theory]
        [InlineData("stream-1", 3)]
        [InlineData("stream-2", 2)]
        public async Task ReturnsEventsFromCorrectStreamOnly(string streamId, long expectedCount)
        {
            var store = _fixture.BuildEventStore(_dbContext) as IEventStore<string, StringEvent, IEventMetadata<string>>;
            var events = await store.Events(streamId);
            Assert.Equal(expectedCount, events.Count());
        }

        [Theory]
        [InlineData("stream-1", 2)]
        [InlineData("stream-2", 1)]
        public async Task ReturnsEventsAccordingToQuery(string streamId, long expectedCount)
        {
            var store = _fixture.BuildEventStore(_dbContext) as IEventStore<string, StringEvent, IEventMetadata<string>>;
            var events = await store.Events(streamId, es => es.Where(e => e.Version > 1));
            Assert.Equal(expectedCount, events.Count());
        }

        [Fact]
        public async Task ReturnsAllEvents()
        {
            var store = _fixture.BuildEventStore(_dbContext) as IEventStore<string, StringEvent, IEventMetadata<string>>;
            var events = await store.Events();
            Assert.Equal(5, events.Count());
        }

        [Fact]
        public async Task ReturnsAllEventsAccordingToQuery()
        {
            var store = _fixture.BuildEventStore(_dbContext) as IEventStore<string, StringEvent, IEventMetadata<string>>;
            var events = await store.Events(es => es.Where(e => e.Version > 1));
            Assert.Equal(3, events.Count());
        }
    }
}