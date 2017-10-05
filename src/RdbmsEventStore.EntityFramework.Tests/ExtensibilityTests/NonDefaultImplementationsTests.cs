using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RdbmsEventStore.EntityFramework.Tests.Infrastructure;
using RdbmsEventStore.EntityFramework.Tests.TestData;
using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.ExtensibilityTests
{
    public class NonDefaultEvent : IMutableEvent<long, long>
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

    public class NonDefaultContext : DbContext, IEventDbContext<NonDefaultEvent>
    {
        public DbSet<NonDefaultEvent> Events { get; set; }
    }

    [Collection(nameof(InMemoryDatabaseCollection))]
    public class NonDefaultImplementationsTests : IClassFixture<EventStoreFixture<long, long, NonDefaultEvent>>, IDisposable
    {
        private readonly EventStoreFixture<long, long, NonDefaultEvent> _fixture;
        private readonly NonDefaultContext _dbContext;

        public NonDefaultImplementationsTests(EventStoreFixture<long, long, NonDefaultEvent> fixture)
        {
            EffortProviderFactory.ResetDb();
            _fixture = fixture;
            _dbContext = new NonDefaultContext();
        }

        [Fact]
        public async Task CanCommitEventsToStoreWithDefaultImplementations()
        {
            var store = _fixture.BuildEventStore(_dbContext);

            await store.Commit(1, 0, new FooEvent { Foo = "Bar" });
        }

        [Fact]
        public async Task CanReadEventsFromStoreWithNonDefaultImplementations()
        {
            _dbContext.Events.AddRange(new[]
            {
                new NonDefaultEvent
                {
                    StreamId = 1,
                    Timestamp = DateTimeOffset.UtcNow,
                    Version = 1,
                    Type = "FooEvent",
                    Payload =Encoding.UTF8.GetBytes(@"{""Foo"":""Bar""}")
                }
            });
            await _dbContext.SaveChangesAsync();

            var store = _fixture.BuildEventStore(_dbContext);

            var events = await store.Events(1);

            Assert.Equal(1, events.Count());
        }
        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}