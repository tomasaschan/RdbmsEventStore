using Newtonsoft.Json;

namespace RdbmsEventStore
{
    /// <summary>
    /// This class exists just to make sure that we only get one instance of the serializer, even if we
    /// have many combinations of generic type arguments on the <see cref="EventFactory{TId,TEvent}" />
    /// </summary>
    public abstract class EventFactoryBase
    {
        protected readonly IEventRegistry _registry;
        protected static readonly JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.None };

        protected EventFactoryBase(IEventRegistry registry)
        {
            _registry = registry;
        }
    }
}