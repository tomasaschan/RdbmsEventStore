using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RdbmsEventStore
{
    public class EventCollection<TStreamId, TEvent> : IEnumerable<TEvent> where TEvent : IEvent<TStreamId>
    {
        private readonly IEnumerable<TEvent> _events;

        public EventCollection(TStreamId streamId, Func<TStreamId, object, TEvent> factory, IEnumerable<object> payloads)
        {
            _events = payloads.Select(payload => factory(streamId, payload));
        }

        public IEnumerator<TEvent> GetEnumerator() => _events.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}