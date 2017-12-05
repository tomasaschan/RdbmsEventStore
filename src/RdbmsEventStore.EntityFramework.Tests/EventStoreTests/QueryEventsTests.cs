using System.Data.Entity;
using System.Linq;
using System.Threading;
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
            AddEvents("stream-1", new FooEvent { Foo = "Foo" });
            Thread.Sleep(10); // to make timestamps differ between the first and the rest
            AddEvents("stream-1", new BarEvent { Bar = "Bar" }, new FooEvent { Foo = "Baz" });
            AddEvents("stream-2", new FooEvent { Foo = "Boo" });
            Thread.Sleep(10);
            AddEvents("stream-2", new BarEvent { Bar = "Far" });

            _dbContext.SaveChanges();
        }

        private void AddEvents(string streamId, params object[] payloads)
        {
            var events = _fixture.EventFactory.Create(streamId, payloads)
                .Select(_fixture.EventSerializer.Serialize)
                .VersionedPerTimestamp<LongStringPersistedEvent, string>();
            _dbContext.Events.AddRange(events);
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
            var firstEventTimestamp = await _dbContext.Events
                .Where(e => e.StreamId == streamId)
                .Select(e => e.Timestamp)
                .FirstAsync();

            var store = _fixture.BuildEventStore(_dbContext) as IEventStore<string, StringEvent, IEventMetadata<string>>;
            var events = await store.Events(streamId, es => es.Where(e => e.Timestamp > firstEventTimestamp));
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
            var firstEventTimestamp = await _dbContext.Events
                .Select(e => e.Timestamp)
                .FirstAsync();

            var store = _fixture.BuildEventStore(_dbContext) as IEventStore<string, StringEvent, IEventMetadata<string>>;
            var events = await store.Events(es => es.Where(e => e.Timestamp > firstEventTimestamp));
            Assert.Equal(4, events.Count());
        }
    }
}