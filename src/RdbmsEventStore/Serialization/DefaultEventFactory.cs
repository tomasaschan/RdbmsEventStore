using System;
using System.Collections.Generic;

namespace RdbmsEventStore.Serialization
{
    public class DefaultEventFactory<TStreamId, TEvent> : IEventFactory<TStreamId, TEvent> where TEvent : IMutableEvent<TStreamId>, new()
    {
        public virtual IEnumerable<TEvent> Create(TStreamId streamId, IEnumerable<object> payloads)
        {
            return new EventCollection<TStreamId, TEvent>(streamId, CreateSingle, payloads);
        }

        protected virtual TEvent CreateSingle(TStreamId streamId, object payload)
        {
            return new TEvent
            {
                StreamId = streamId,
                Timestamp = DateTimeOffset.UtcNow,
                Type = payload.GetType(),
                Payload = payload
            };
        }
    }
}