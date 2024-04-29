using SharedLeit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLeit.Entities
{
    public record ContainerCard
    {
        public long Id { get; set; }

        //[ForeignKey("Container")]
        public long ContainerId { get; set; }
        public Container Container{ get; set; }

        //[ForeignKey("Card")]
        public long CardId { get; set; }
        public Card Card { get; set; }
    }
}