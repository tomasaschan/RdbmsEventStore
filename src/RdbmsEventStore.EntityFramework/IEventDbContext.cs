using System.Data.Entity;

namespace RdbmsEventStore.EntityFramework
{
    public interface IEventDbContext<TEvent> where TEvent : class
    {
        DbSet<TEvent> Events { get; }
    }
}