using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RdbmsEventStore.Serialization;

namespace RdbmsEventStore.EntityFramework
{
    public class EntityFrameworkEventStore<TId, TStreamId, TContext, TEvent, TEventMetadata, TPersistedEvent> : IEventStore<TStreamId, TEvent, TEventMetadata>
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TContext : DbContext, IEventDbContext<TPersistedEvent>
        where TEvent : class, TEventMetadata, IMutableEvent<TStreamId>, new()
        where TPersistedEvent : class, IPersistedEvent<TStreamId>, TEventMetadata, new()
        where TEventMetadata : class, IEventMetadata<TStreamId>
    {
        private readonly TContext _context;
        private readonly IEventFactory<TStreamId, TEvent> _eventFactory;
        private readonly IWriteLock<TStreamId> _writeLock;
        private readonly IEventSerializer<TEvent, TPersistedEvent> _serializer;

        public EntityFrameworkEventStore(TContext context, IEventFactory<TStreamId, TEvent> eventFactory, IWriteLock<TStreamId> writeLock, IEventSerializer<TEvent, TPersistedEvent> serializer)
        {
            _context = context;
            _eventFactory = eventFactory;
            _writeLock = writeLock;
            _serializer = serializer;
        }

        public async Task<IEnumerable<TEvent>> Events(Func<IQueryable<TEventMetadata>, IQueryable<TEventMetadata>> query)
        {
            var storedEvents = await _context.Events
                .AsNoTracking()
                .OrderBy(e => e.Timestamp)
                .ThenBy(e => e.Version)
                .Apply(query)
                .ToListAsync();

            var events = storedEvents
                .Cast<TPersistedEvent>()
                .Select(_serializer.Deserialize);

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
                var highestVersionNumber = await _context.Events
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

                var events = _eventFactory
                    .Create(streamId, payloads)
                    .Select(_serializer.Serialize)
                    .VersionedPerTimestamp<TPersistedEvent, TStreamId>();

                _context.Events.AddRange(events);
                await _context.SaveChangesAsync();
            }
        }
    }
}
