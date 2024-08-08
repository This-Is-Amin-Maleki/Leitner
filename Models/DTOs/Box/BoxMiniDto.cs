using ModelsLeit.DTOs.Collection;
using ModelsLeit.Entities;

namespace ModelsLeit.DTOs.Box
{
    public record BoxMiniDto
    {
        public long Id { get; set; }
        public int CardPerDay { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateStudied { get; set; }
        public int LastSlot { get; set; }
        public long LastCardId { get; set; }
        public bool Completed { get; set; }

        public CollectionMiniDto Collection { get; set; }
        public ICollection<Slot> Slots { get; set; }
    }
}
