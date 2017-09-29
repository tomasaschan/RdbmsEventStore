using System;

namespace RdbmsEventStore
{
    public interface IEvent<out TId>
    {
        DateTimeOffset Timestamp { get; }

        TId EventId { get; }

        TId StreamId { get; }

        long Version { get; }

        string Type { get; }

        byte[] Payload { get; }
    }
    public interface IMutableEvent<TId> : IEvent<TId>
    {
        new DateTimeOffset Timestamp { get; set; }

        new TId EventId { get; set; }

        new TId StreamId { get; set; }

        new long Version { get; set; }

        new string Type { get; set; }

        new byte[] Payload { get; set; }
    }
}