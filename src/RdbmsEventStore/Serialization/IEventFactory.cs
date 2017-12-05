﻿using System.Collections.Generic;

namespace RdbmsEventStore.Serialization
{
    public interface IEventFactory<in TStreamId, out TEvent> where TEvent : IEvent<TStreamId>
    {
        IEnumerable<TEvent> Create(TStreamId streamId, IEnumerable<object> payloads);
    }
}