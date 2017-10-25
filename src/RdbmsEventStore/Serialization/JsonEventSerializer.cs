using Newtonsoft.Json;

namespace RdbmsEventStore.Serialization
{
    public abstract class JsonEventSerializer
    {
        protected static readonly JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.None };
    }
}