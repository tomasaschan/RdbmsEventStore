using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RdbmsEventStore
{
    public class EventCollection<TStreamId, TEvent> : IEnumerable<TEvent> where TEvent : IEvent<TStreamId>
    {
        private readonly IEnumerable<TEvent> _events;

        public EventCollection(TStreamId streamId, long currentVersion, Func<TStreamId, long, object, TEvent> factory, IEnumerable<object> payloads)
        {
            _events = payloads.Select((payload, i) => factory(streamId, currentVersion + 1 + i, payload));
        }

        public IEnumerator<TEvent> GetEnumerator() => _events.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}