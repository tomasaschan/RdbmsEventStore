using System;
using System.Collections.Generic;

namespace RdbmsEventStore
{
    public class DefaultEventFactory<TId, TEvent> : IEventFactory<TId, TEvent>
        where TEvent : IMutableEvent<TId>, new()
    {
        private readonly IEventRegistry _registry;
        private readonly IEventSerializer _serializer;

        public DefaultEventFactory(IEventRegistry registry, IEventSerializer serializer)
        {
            _registry = registry;
            _serializer = serializer;
        }

        public virtual IEnumerable<TEvent> Create(TId streamId, long version, params object[] payloads)
        {
            return new EventCollection<TId, TEvent>(streamId, version, CreateSingle, payloads);
        }

        protected virtual TEvent CreateSingle(TId streamId, long version, object payload)
        {
            return new TEvent
            {
                StreamId = streamId,
                Timestamp = DateTimeOffset.UtcNow,
                Version = version,
                Type = _registry.NameFor(payload.GetType()),
                Payload = _serializer.Serialize(payload)
            };
        }
    }
}