using System;
using System.Collections.Generic;

namespace RdbmsEventStore
{
    public class DefaultEventFactory<TId, TStreamId, TEvent> : IEventFactory<TId, TStreamId, TEvent>
        where TEvent : IMutableEvent<TId, TStreamId>, new()
    {
        private readonly IEventRegistry _registry;
        private readonly IEventSerializer _serializer;

        public DefaultEventFactory(IEventRegistry registry, IEventSerializer serializer)
        {
            _registry = registry;
            _serializer = serializer;
        }

        public virtual IEnumerable<TEvent> Create(TStreamId streamId, long version, params object[] payloads)
        {
            return new EventCollection<TId, TStreamId, TEvent>(streamId, version, CreateSingle, payloads);
        }

        protected virtual TEvent CreateSingle(TStreamId streamId, long version, object payload)
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