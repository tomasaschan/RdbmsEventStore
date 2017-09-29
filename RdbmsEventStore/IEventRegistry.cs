using System;
using System.Collections.Generic;

namespace RdbmsEventStore
{
    public interface IEventRegistry
    {
        Type TypeFor(string eventType);

        string NameFor(Type eventType);

        IReadOnlyDictionary<string, Type> Registrations { get; }
    }
}