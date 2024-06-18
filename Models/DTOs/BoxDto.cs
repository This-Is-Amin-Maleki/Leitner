using ModelsLeit.Entities;
using ModelsLeit.ViewModels;
using SharedLeit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLeit.DTOs
{
    public record BoxDto
    {
        public long Id { get; set; }
        public bool Completed { get; set; }
        public DateTime DateAdded { get; set; }

        public DateTime DateStudied { get; set; }
        /// <summary>
        /// Default value should be -2, indicating: 'Start studying from slot 4 and decrease the slot number after each study, until it reaches -2'.
        /// -1 indicating : 'It'ss tempSlot, add some new cards to fill and study mistakes'.
        /// 0-4 indicating : 'Start studying cards of slot[(2^StdySlot) - 1]'.
        /// </summary>
        public int LastSlot { get; set; }
        public long LastCardId { get; set; }
        public int CardPerDay { get; set; }
        public CollectionDto Collection { get; set; }
        public ICollection<ContainersDto> Containers { get; set; }
    }
}
