using System.Data.Entity;

namespace RdbmsEventStore.EntityFramework
{
    public class EntityFrameworkEventStoreContext<TEvent> : DbContext, IEventDbContext<TEvent>
        where TEvent : class
    {
        public EntityFrameworkEventStoreContext() { }
        public EntityFrameworkEventStoreContext(string connectionInfo) : base(connectionInfo) { }
        public DbSet<TEvent> Events { get; set; }
    }
}