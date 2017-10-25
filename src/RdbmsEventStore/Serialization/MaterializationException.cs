using System;

namespace RdbmsEventStore.Serialization
{
    public class MaterializationException : InvalidOperationException
    {
        public MaterializationException(string type, string payload, Exception innerException)
            : base($"Unable to deserialize '{payload}' into event of type '{type}'.", innerException)
        {
        }
    }
}