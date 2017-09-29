using System.Data.Entity;

namespace RdbmsEventStore.EntityFramework
{
    public class EventStoreContext<TEvent> : DbContext, IEventDbContext<TEvent>
        where TEvent : class
    {
        public EventStoreContext(string connectionInfo) : base(connectionInfo) { }
        public DbSet<TEvent> Events { get; set; }
    }
}