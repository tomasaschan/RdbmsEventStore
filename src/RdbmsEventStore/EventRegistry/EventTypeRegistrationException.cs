using System;

namespace RdbmsEventStore.EventRegistry
{
    public class EventTypeRegistrationException : InvalidOperationException
    {
        public EventTypeRegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}