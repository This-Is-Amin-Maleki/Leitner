using SharedLeit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLeit.Entities
{
    public record Container
    {
        public long Id { get; set; }
        public ICollection<Card> Cards { get; set; }

        [ForeignKey("Slot")]
        public long SlotId { get; set; }
        public Slot Slot { get; set; }
    }
}