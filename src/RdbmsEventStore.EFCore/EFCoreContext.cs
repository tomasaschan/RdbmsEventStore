using Microsoft.EntityFrameworkCore;
using RdbmsEventStore.Serialization;

namespace RdbmsEventStore.EFCore
{
    public class EFCoreContext<TStreamId, TEvent> : DbContext, IEventDbContext<TEvent> where TEvent : class, IPersistedEvent<TStreamId>
    {
        public EFCoreContext()
        {
        }

        public EFCoreContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<TEvent> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TEvent>()
                .HasIndex(e => e.StreamId);
        }
    }
}