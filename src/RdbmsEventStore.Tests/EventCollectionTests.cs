using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Xunit;

namespace RdbmsEventStore.Tests
{
    public class EventCollectionTests
    {
        private class TestEvent : IEvent<Guid>
        {
            public DateTimeOffset Timestamp { get; set; }
            public Guid EventId { get; set; }
            public Guid StreamId { get; set; }
            public long Version { get; set; }
            public string Type { get; set; }
            public byte[] Payload { get; set; }
        }

        private static TestEvent Factory(Guid stream, long version, object payload)
            => new TestEvent
            {
                EventId = Guid.NewGuid(),
                StreamId = stream,
                Version = version,
                Timestamp = DateTimeOffset.Now,
                Type = payload.GetType().Name,
                Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload))
            };

        private readonly EventCollection<Guid, TestEvent> _collection;

        public EventCollectionTests()
        {
            var streamId = Guid.NewGuid();
            _collection = new EventCollection<Guid, TestEvent>(streamId, 0, Factory, new { Foo = "Foo" }, new { Bar = "Bar" });
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
                v => Assert.Equal(0, v),
                v => Assert.Equal(1, v));
        }
    }
}