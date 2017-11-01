using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RdbmsEventStore
{
    public static class EventStoreExtensions
    {
        public static Task<IEnumerable<TEvent>> Events<TStreamId, TEvent, TEventMetadata>(this IEventStore<TStreamId, TEvent, TEventMetadata> store)
            where TStreamId : IEquatable<TStreamId>
            where TEvent : class, TEventMetadata, IEvent<TStreamId>
            where TEventMetadata : class, IEventMetadata<TStreamId>
            => store.Events(events => events);

        public static Task<IEnumerable<TEvent>> Events<TStreamId, TEvent, TEventMetadata>(this IEventStore<TStreamId, TEvent, TEventMetadata> store, TStreamId streamId)
            where TStreamId : IEquatable<TStreamId>
            where TEvent : class, TEventMetadata, IEvent<TStreamId>
            where TEventMetadata : class, IEventMetadata<TStreamId>
            => store.Events(streamId, events => events);

        public static Task<IEnumerable<TEvent>> Events<TStreamId, TEvent, TEventMetadata>(this IEventStore<TStreamId, TEvent, TEventMetadata> store, TStreamId streamId, Func<IQueryable<TEventMetadata>, IQueryable<TEventMetadata>> query)
            where TStreamId : IEquatable<TStreamId>
            where TEvent : class, TEventMetadata, IEvent<TStreamId>
            where TEventMetadata : class, IEventMetadata<TStreamId>
            => store.Events(events => events.Where(e => e.StreamId.Equals(streamId)).Apply(query));

        public static Task Append<TStreamId, TEvent, TEventMetadata>(this IEventStore<TStreamId, TEvent, TEventMetadata> store, TStreamId streamId, long versionBefore, object payload)
            where TStreamId : IEquatable<TStreamId>
            where TEvent : class, TEventMetadata, IEvent<TStreamId>
            where TEventMetadata : class, IEventMetadata<TStreamId>
            => store.Append(streamId, versionBefore, new[] { payload });
    }
}