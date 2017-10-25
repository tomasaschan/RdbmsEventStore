using System;
using System.Linq;
using Xunit;

namespace RdbmsEventStore.Tests
{
    public class EventCollectionTests
    {
        private class TestEvent : IEvent<Guid>
        {
            public DateTimeOffset Timestamp { get; set; }
            public Guid StreamId { get; set; }
            public long Version { get; set; }
            public Type Type { get; set; }
            public object Payload { get; set; }
        }

        private static TestEvent Factory(Guid stream, long version, object payload)
            => new TestEvent
            {
                StreamId = stream,
                Version = version,
                Timestamp = DateTimeOffset.Now,
                Type = payload.GetType(),
                Payload = payload
            };

        private readonly EventCollection<Guid, TestEvent> _collection;

        public EventCollectionTests()
        {
            var streamId = Guid.NewGuid();
            _collection = new EventCollection<Guid, TestEvent>(streamId, 0, Factory, new object[] { new { Foo = "Foo" }, new { Bar = "Bar" } });
        }

        [Fact]
        public void EventCollectionContainsCorrectNumberOfElements()
        {
            Assert.Equal(2, _collection.Count());
        }

        [Fact]
        public void EventCollectionCorrectlyVersionsEvents()
        {
            Assert.Collection(_collection.Select(e => e.Version),
                v => Assert.Equal(1, v),
                v => Assert.Equal(2, v));
        }
    }
}