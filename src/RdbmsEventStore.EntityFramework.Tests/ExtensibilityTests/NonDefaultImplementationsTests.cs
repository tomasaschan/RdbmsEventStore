using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using RdbmsEventStore.EntityFramework.Tests.Infrastructure;
using RdbmsEventStore.EntityFramework.Tests.TestData;
using RdbmsEventStore.Serialization;
using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.ExtensibilityTests
{
    public class NonDefaultEvent : IMutableEvent<long>
    {
        public DateTimeOffset Timestamp { get; set; }
        public long StreamId { get; set; }
        public Type Type { get; set; }
        public object Payload { get; set; }
    }

    public class NonDefaultPersistedEvent : IPersistedEvent<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EventId { get; set; }
        [Required]
        public long StreamId { get; set; }
        [Required]
        public DateTimeOffset Timestamp { get; set; }
        [Required]
        public long Version { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public byte[] Payload { get; set; }
    }

    public class NonDefaultContext : DbContext, IEventDbContext<NonDefaultPersistedEvent>
    {
        public DbSet<NonDefaultPersistedEvent> Events { get; set; }
    }

    [Collection(nameof(InMemoryDatabaseCollection))]
    public class NonDefaultImplementationsTests : IClassFixture<EventStoreFixture<long, long, NonDefaultEvent, IEventMetadata<long>, NonDefaultPersistedEvent>>, IDisposable
    {
        private readonly EventStoreFixture<long, long, NonDefaultEvent, IEventMetadata<long>, NonDefaultPersistedEvent> _fixture;
        private readonly NonDefaultContext _dbContext;

        public NonDefaultImplementationsTests(EventStoreFixture<long, long, NonDefaultEvent, IEventMetadata<long>, NonDefaultPersistedEvent> fixture)
        {
            EffortProviderFactory.ResetDb();
            _fixture = fixture;
            _dbContext = new NonDefaultContext();
        }

        [Fact]
        public async Task CanCommitEventsToStoreWithDefaultImplementations()
        {
            var store = _fixture.BuildEventStore(_dbContext);

            await store.Append(1, null, new[] { new FooEvent { Foo = "Bar" } });
        }

        [Fact]
        public async Task CanReadEventsFromStoreWithNonDefaultImplementations()
        {
            _dbContext.Events.AddRange(new[]
            {
                new NonDefaultPersistedEvent
                {
                    StreamId = 1,
                    Timestamp = DateTimeOffset.UtcNow,
                    Version = 1,
                    Type = "FooEvent",
                    Payload = Encoding.UTF8.GetBytes(@"{""Foo"":""Bar""}")
                }
            });
            await _dbContext.SaveChangesAsync();

            var store = _fixture.BuildEventStore(_dbContext) as IEventStore<long, NonDefaultEvent, IEventMetadata<long>>;

            var events = await store.Events(1);

            Assert.Single(events);
        }
        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}