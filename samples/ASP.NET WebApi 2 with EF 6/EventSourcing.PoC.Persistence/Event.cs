using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RdbmsEventStore.EntityFramework;

namespace EventSourcing.PoC.Persistence
{
    public class Event : EntityFrameworkEvent<long, string>
    {
        [Required]
        [Index(Order = 1)]
        [MaxLength(255)]
        public override string StreamId { get; set; }
    }
}