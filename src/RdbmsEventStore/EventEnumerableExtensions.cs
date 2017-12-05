using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RdbmsEventStore.Serialization;

namespace RdbmsEventStore
{
    public static class EventEnumerableExtensions
    {
        public static IEnumerable<TPersistedEvent> VersionedPerTimestamp<TPersistedEvent, TStreamId>(this IEnumerable<TPersistedEvent> source)
            where TPersistedEvent : IPersistedEvent<TStreamId>
            => source.GroupBy(@event => @event.Timestamp)
                .OrderBy(group => group.Key)
                .Select(group => group.Select((@event, i) =>
                {
                    @event.Version = i;
                    return @event;
                }))
                .SelectMany(group => group);
    }
}