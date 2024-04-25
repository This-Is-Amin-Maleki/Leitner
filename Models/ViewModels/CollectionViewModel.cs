using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public record CollectionViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public CollectionStatus Status { get; set; }

        public long CardsQ { get; set; }
        public long BoxCount { get; set; }
    }
}
