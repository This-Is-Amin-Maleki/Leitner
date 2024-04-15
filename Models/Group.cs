using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public record Group
    {
        public long Id { get; set; }
        public ICollection<Card>? Cards { get; set; }
        public ICollection<Slot>? Slots { get; set; }
    }
}