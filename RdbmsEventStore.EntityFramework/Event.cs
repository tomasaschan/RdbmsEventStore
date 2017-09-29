using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RdbmsEventStore.EntityFramework
{
    public class Event<TId> : IEvent<TId>, IMutableEvent<TId>
    {
        [Key]
        public TId EventId { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        [Index(Order = 1)]
        [Required]
        public TId StreamId { get; set; }

        [Required]
        [Index(Order = 2)]
        public long Version { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public byte[] Payload { get; set; }
    }
}