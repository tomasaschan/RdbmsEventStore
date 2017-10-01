using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RdbmsEventStore
{
    public class EventFactory<TId, TEvent> : EventFactoryBase, IEventFactory<TId, TEvent>
        where TEvent : IMutableEvent<TId>, new()
    {
        public EventFactory(IEventRegistry registry) : base(registry)
        {
        }

        public virtual IEnumerable<TEvent> Create(TId streamId, long version, params object[] payloads)
        {
            return new EventCollection<TId, TEvent>(streamId, version, CreateSingle, payloads);
        }

        private TEvent CreateSingle(TId streamId, long version, object payload)
        {
            var writer = new StringWriter();
            serializer.Serialize(writer, payload);
            return new TEvent
            {
                StreamId = streamId,
                Timestamp = DateTimeOffset.UtcNow,
                Version = version,
                Type = _registry.NameFor(payload.GetType()),
                Payload = Encoding.UTF8.GetBytes(writer.ToString())
            };
        }
    }
}