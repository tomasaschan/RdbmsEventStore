using System.Linq;
using System.Threading.Tasks;
using RdbmsEventStore.EntityFramework.Tests.Infrastructure;
using RdbmsEventStore.EntityFramework.Tests.TestData;
using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.EventStoreTests
{
    public class QueryEventsTests : EventStoreTestBase<long, string, LongStringEvent>
    {
        public QueryEventsTests(EventStoreFixture<long, string, LongStringEvent> fixture, AssemblyInitializerFixture initializer) : base(fixture, initializer)
        {
            _dbContext.Events.AddRange(_fixture.EventFactory.Create("stream-1", 0, 
                new FooEvent{Foo = "Foo"},
                new BarEvent{Bar = "Bar"},
                new FooEvent{Foo = "Baz"}));
            _dbContext.Events.AddRange(_fixture.EventFactory.Create("stream-2", 0,
                new FooEvent {Foo = "Boo"},
                new BarEvent {Bar = "Far"}));
            _dbContext.SaveChanges();
        }
        
        [Theory]
        [InlineData("stream-1", 3)]
        [InlineData("stream-2", 2)]
        public async Task ReturnsEventsFromCorrectStreamOnly(string streamId, long expectedCount)
        {
            var store = _fixture.BuildEventStore(_dbContext);
            var events = await store.Events(streamId);
            Assert.Equal(expectedCount, events.Count());
        }

        [Theory]
        [InlineData("stream-1", 2)]
        [InlineData("stream-2", 1)]
        public async Task ReturnsEventsAccordingToQuery(string streamId, long expectedCount)
        {
            var store = _fixture.BuildEventStore(_dbContext);
            var events = await store.Events(streamId, es => es.Where(e => e.Type == "FooEvent"));
            Assert.Equal(expectedCount, events.Count());
        }

    }
}