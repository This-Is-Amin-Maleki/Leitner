using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public record CollectionDTO
    {
        public long? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        [Required]
        public CollectionStatus Status { get; set; }

        public long UserId { get; set; }
    }
}
