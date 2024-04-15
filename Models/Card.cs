using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public record Card
    {
        public long Id { get; set; }
        public String Name { get; set; }
        public String? Description { get; set; }
        public DateTime Date { get; set; }
        public CardStatus Status { get; set; }
    }
}
