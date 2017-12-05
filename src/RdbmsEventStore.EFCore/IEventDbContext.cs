using Microsoft.EntityFrameworkCore;

namespace RdbmsEventStore.EFCore
{
    public interface IEventDbContext<TEvent> where TEvent : class
    {
        DbSet<TEvent> Events { get; }
    }
}