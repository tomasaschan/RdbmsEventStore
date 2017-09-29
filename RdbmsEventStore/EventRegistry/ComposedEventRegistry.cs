using System.Linq;

namespace RdbmsEventStore.EventRegistry
{
    public class ComposedEventRegistry : SimpleEventRegistry
    {
        public ComposedEventRegistry(params IEventRegistry[] registries)
            : base(registries
                .SelectMany(r => r.Registrations)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
        {
        }
    }
}