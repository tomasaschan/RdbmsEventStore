using System;
using System.Collections.Generic;

namespace RdbmsEventStore.EventRegistry
{
    public interface IEventRegistry
    {
        Type TypeFor(string eventType);

        string NameFor(Type eventType);
    }

    public interface IComposableEventRegistry : IEventRegistry
    {
        IReadOnlyDictionary<string, Type> Registrations { get; }
    }
}