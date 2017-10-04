using System;

namespace RdbmsEventStore
{
    public interface IEvent<out TId, out TStreamId>
    {
        DateTimeOffset Timestamp { get; }

        TId EventId { get; }

        TStreamId StreamId { get; }

        long Version { get; }

        string Type { get; }

        byte[] Payload { get; }
    }

    public interface IMutableEvent<TId, TStreamId> : IEvent<TId, TStreamId>
    {
        new DateTimeOffset Timestamp { get; set; }

        new TId EventId { get; set; }

        new TStreamId StreamId { get; set; }

        new long Version { get; set; }

        new string Type { get; set; }

        new byte[] Payload { get; set; }
    }
}