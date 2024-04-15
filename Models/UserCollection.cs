using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public record UserCollection
    {
        public long Id { get; set; }


        public DateTime AddedForStudyDate { get; set; }

        public DateTime LastReviewedDate { get; set; }
        public long LastReviewedCardId { get; set; }

        public Card[][] Boxes { get; set; }
        public Card[] TempBox { get; set; }

        [ForeignKey("ApplicationUser")]
        public long UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Collection")]
        public long CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
