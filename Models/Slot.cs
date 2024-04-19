using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public record Slot
    {
        public long Id { get; set; }
        public ICollection<Group>? Groups { get; set; }

        [ForeignKey("Box")]
        public long BoxId { get; set; }
        public Box Box { get; set; }
    }
}