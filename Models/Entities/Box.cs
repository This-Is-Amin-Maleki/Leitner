using SharedLeit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLeit.Entities
{
    public record Box
    {
        public long Id { get; set; }

        public DateTime AddedForStudyDate { get; set; }

        public DateTime LastReviewedDate { get; set; }
        /// <summary>
        /// Default value should be -2, indicating: 'Start studying from slot 4 and decrease the slot number after each study, until it reaches -2'.
        /// -1 indicating : 'It'ss tempSlot, add some new cards to fill and study mistakes'.
        /// 0-4 indicating : 'Start studying cards of slot[(2^StdySlot) - 1]'.
        /// </summary>
        public int LastReviewedSlot { get; set; }
        public long LastReviewedCardId { get; set; }

        //[ForeignKey("Collection")]
        public long CollectionId { get; set; }
        public Collection Collection { get; set; }

        public ICollection<Slot> Slots { get; set; }
    }
}
