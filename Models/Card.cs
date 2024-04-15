using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public record Card
    {
        public long Id { get; set; }
        public String Ask { get; set; }
        public String Answer { get; set; }
        public String? Description { get; set; }
        public bool HasMp3 { get; set; }

        [ForeignKey("Collection")]
        public long CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
