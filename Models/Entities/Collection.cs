using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public record Collection
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public CollectionStatus Status { get; set; }

        public ICollection<Card> Cards { get; set; }
        public ICollection<Box> Boxes { get; set; }
    }
}
