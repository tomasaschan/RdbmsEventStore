using Microsoft.EntityFrameworkCore;

namespace RdbmsEventStore.EFCore
{
    public interface IEFCoreEventStoreContext<TEvent> where TEvent : class
    {
        DbSet<TEvent> Events { get; }
    }
}