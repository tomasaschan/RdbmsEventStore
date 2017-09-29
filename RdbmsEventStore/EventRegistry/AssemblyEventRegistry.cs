using System;
using System.Linq;
using System.Reflection;

namespace RdbmsEventStore.EventRegistry
{
    public class AssemblyEventRegistry : SimpleEventRegistry
    {
        public AssemblyEventRegistry(Type markerType) : this(markerType, type => true)
        {
        }

        public AssemblyEventRegistry(Type markerType, Func<Type, bool> inclusionPredicate)
            : base(markerType
                  .GetTypeInfo()
                  .Assembly
                  .GetTypes()
                  .Where(inclusionPredicate)
                  .ToDictionary(type => type.Name, type => type))
        {
        }
    }
}