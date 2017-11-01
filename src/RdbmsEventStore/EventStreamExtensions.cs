using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RdbmsEventStore
{
    public static class EventStreamExtensions
    {
        public static Task<IEnumerable<TEvent>> Events<TStreamId, TEvent, TEventMetadata>(this IEventStream<TStreamId, TEvent, TEventMetadata> stream)
            where TStreamId : IEquatable<TStreamId>
            where TEvent : class, TEventMetadata, IEvent<TStreamId>
            where TEventMetadata : class, IEventMetadata<TStreamId>
            => stream.Events(events => events);

        public static Task<IEnumerable<TEvent>> Events<TStreamId, TEvent, TEventMetadata>(this IEventStream<TStreamId, TEvent, TEventMetadata> stream, TStreamId streamId)
            where TStreamId : IEquatable<TStreamId>
            where TEvent : class, TEventMetadata, IEvent<TStreamId>
            where TEventMetadata : class, IEventMetadata<TStreamId>
            => stream.Events(streamId, events => events);

        public static Task<IEnumerable<TEvent>> Events<TStreamId, TEvent, TEventMetadata>(this IEventStream<TStreamId, TEvent, TEventMetadata> stream, TStreamId streamId, Func<IQueryable<TEventMetadata>, IQueryable<TEventMetadata>> query)
            where TStreamId : IEquatable<TStreamId>
            where TEvent : class, TEventMetadata, IEvent<TStreamId>
            where TEventMetadata : class, IEventMetadata<TStreamId>
            => stream.Events(events => events.Where(e => e.StreamId.Equals(streamId)).Apply(query));

        public static Task Append<TStreamId, TEvent, TEventMetadata>(this IEventStream<TStreamId, TEvent, TEventMetadata> stream, TStreamId streamId, long versionBefore, object payload)
            where TStreamId : IEquatable<TStreamId>
            where TEvent : class, TEventMetadata, IEvent<TStreamId>
            where TEventMetadata : class, IEventMetadata<TStreamId>
            => stream.Append(streamId, versionBefore, new[] { payload });
    }
}