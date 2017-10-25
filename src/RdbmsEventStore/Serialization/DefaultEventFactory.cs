using System;
using System.Collections.Generic;

namespace RdbmsEventStore.Serialization
{
    public class DefaultEventFactory<TStreamId, TEvent> : IEventFactory<TStreamId, TEvent> where TEvent : IMutableEvent<TStreamId>, new()
    {
        public virtual IEnumerable<TEvent> Create(TStreamId streamId, long version, IEnumerable<object> payloads)
        {
            return new EventCollection<TStreamId, TEvent>(streamId, version, CreateSingle, payloads);
        }

        protected virtual TEvent CreateSingle(TStreamId streamId, long version, object payload)
        {
            return new TEvent
            {
                StreamId = streamId,
                Timestamp = DateTimeOffset.UtcNow,
                Version = version,
                Type = payload.GetType(),
                Payload = payload
            };
        }
    }
}