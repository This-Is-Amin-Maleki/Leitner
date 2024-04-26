using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Models.Entities
{
    public record Card
    {
        public long Id { get; set; }
        public string Ask { get; set; }
        public string Answer { get; set; }
        public string Description { get; set; }
        public bool HasMp3 { get; set; }

        [ForeignKey("Collection")]
        public long CollectionId { get; set; }
        public Collection Collection { get; set; }

        //[ForeignKey("Container")]
        public ICollection<Container> Containers { get; set; }

    }
}
