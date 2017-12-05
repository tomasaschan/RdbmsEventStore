using Microsoft.EntityFrameworkCore;
using RdbmsEventStore.Serialization;

namespace RdbmsEventStore.EFCore
{
    public class EFCoreEventStoreContext<TStreamId, TEvent> : DbContext, IEFCoreEventStoreContext<TEvent> where TEvent : class, IPersistedEvent<TStreamId>
    {
        public EFCoreEventStoreContext()
        {
        }

        public EFCoreEventStoreContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
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