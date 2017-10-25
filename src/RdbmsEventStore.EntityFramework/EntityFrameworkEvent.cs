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
        public virtual TId EventId { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        [Index(Order = 1)]
        [Required]
        public virtual TStreamId StreamId { get; set; }

        [Required]
        [Index(Order = 2)]
        public virtual long Version { get; set; }

        [Required]
        public virtual string Type { get; set; }

        [Required]
        public virtual byte[] Payload { get; set; }
    }
}