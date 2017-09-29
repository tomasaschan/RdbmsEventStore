using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RdbmsEventStore
{
    public class EventCollection<TId, TEvent> : IEnumerable<TEvent> where TEvent : IEvent<TId>
    {
        private readonly IEnumerable<TEvent> _events;

        public EventCollection(TId streamId, long currentVersion, Func<TId, long, object, TEvent> factory, params object[] events)
        {
            _events = events.Select((payload, i) => factory(streamId, currentVersion + i, payload));
        }

        public IEnumerator<TEvent> GetEnumerator() => _events.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}