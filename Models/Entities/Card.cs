using SharedLeit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelsLeit.Entities
{
    public record Card
    {
        public long Id { get; set; }
        public string Ask { get; set; }
        public string Answer { get; set; }
        public string? Description { get; set; }
        public bool HasMp3 { get; set; }
        public CardStatus Status { get; set; }

        [ForeignKey("Collection")]
        public long CollectionId { get; set; }
        public Collection Collection { get; set; }

        //[ForeignKey("Container")]
        public ICollection<ContainerCard> ContainerCards { get; set; }

        [Required]
        public long UserId { get; set; }

    }
}
