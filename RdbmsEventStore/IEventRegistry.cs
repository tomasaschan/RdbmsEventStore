using System;

namespace RdbmsEventStore
{
    public interface IEventRegistry
    {
        Type TypeFor(string eventType);

        string NameFor(Type eventType);
    }
}