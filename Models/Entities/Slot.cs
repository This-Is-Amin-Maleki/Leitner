using SharedLeit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLeit.Entities
{
    public record Slot
    {
        public long Id { get; set; }
        public ICollection<Container> Containers { get; set; }

        [ForeignKey("Box")]
        public long BoxId { get; set; }
        public Box Box { get; set; }
    }
}