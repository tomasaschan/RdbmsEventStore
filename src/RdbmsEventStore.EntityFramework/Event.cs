using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RdbmsEventStore.Serialization;

namespace RdbmsEventStore.EntityFramework
{
    public class EntityFrameworkEvent<TId, TStreamId> : IPersistedEvent<TStreamId>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TId EventId { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        [Index(Order = 1)]
        [Required]
        public TStreamId StreamId { get; set; }

        [Required]
        [Index(Order = 2)]
        public long Version { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public byte[] Payload { get; set; }
    }
}