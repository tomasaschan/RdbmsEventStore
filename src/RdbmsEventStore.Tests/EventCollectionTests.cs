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
            public Type Type { get; set; }
            public object Payload { get; set; }
        }

        private static TestEvent Factory(Guid stream, object payload)
            => new TestEvent
            {
                StreamId = stream,
                Timestamp = DateTimeOffset.Now,
                Type = payload.GetType(),
                Payload = payload
            };

        private readonly EventCollection<Guid, TestEvent> _collection;

        public EventCollectionTests()
        {
            var streamId = Guid.NewGuid();
            _collection = new EventCollection<Guid, TestEvent>(streamId, Factory, new object[] { new { Foo = "Foo" }, new { Bar = "Bar" } });
        }

        [Fact]
        public void EventCollectionContainsCorrectNumberOfElements()
        {
            Assert.Equal(2, _collection.Count());
        }
    }
}