using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RdbmsEventStore.Serialization;

namespace RdbmsEventStore.EFCore
{
    public class EFCoreEventStore<TId, TStreamId, TEventStoreContext, TEvent, TEventMetadata, TPersistedEvent>
        : IEventStore<TStreamId, TEvent, TEventMetadata>
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TEventStoreContext : DbContext, IEFCoreEventStoreContext<TPersistedEvent>
        where TEvent : class, TEventMetadata, IMutableEvent<TStreamId>, new()
        where TPersistedEvent : class, IPersistedEvent<TStreamId>, TEventMetadata, new()
        where TEventMetadata : class, IEventMetadata<TStreamId>
    {
        private readonly TEventStoreContext _dbContext;
        private readonly IEventFactory<TStreamId, TEvent> _eventFactory;
        private readonly IWriteLock<TStreamId> _writeLock;
        private readonly IEventSerializer<TEvent, TPersistedEvent> _eventSerializer;

        public EFCoreEventStore(TEventStoreContext dbContext, IEventFactory<TStreamId, TEvent> eventFactory, IWriteLock<TStreamId> writeLock, IEventSerializer<TEvent, TPersistedEvent> eventSerializer)
        {
            _dbContext = dbContext;
            _eventFactory = eventFactory;
            _writeLock = writeLock;
            _eventSerializer = eventSerializer;
        }

        public async Task<IEnumerable<TEvent>> Events(Func<IQueryable<TEventMetadata>, IQueryable<TEventMetadata>> query)
        {
            var storedEvents = await _dbContext.Events
                .AsNoTracking()
                .Apply(query)
                .OrderBy(e => e.Timestamp)
                .ToListAsync();

            var events = storedEvents
                .Cast<TPersistedEvent>()
                .Select(_eventSerializer.Deserialize);

            return events;
        }

        public async Task Append(TStreamId streamId, DateTimeOffset? versionBefore, IEnumerable<object> payloads)
        {
            if (!payloads.Any())
            {
                return;
            }

            using (await _writeLock.Aquire(streamId))
            {
                var highestVersionNumber = await _dbContext.Events
                    .Where(e => e.StreamId.Equals(streamId))
                    .Select(e => e.Timestamp)
                    .DefaultIfEmpty(DateTimeOffset.MinValue)
                    .MaxAsync() as DateTimeOffset?;

                if (highestVersionNumber == DateTimeOffset.MinValue)
                {
                    highestVersionNumber = null;
                }

                if (highestVersionNumber != versionBefore)
                {
                    throw new ConflictException(streamId, versionBefore, highestVersionNumber, payloads);
                }

                var events = _eventFactory.Create(streamId, payloads)
                    .Select(_eventSerializer.Serialize)
                    .VersionedPerTimestamp<TPersistedEvent, TStreamId>();
                _dbContext.Events.AddRange(events);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
