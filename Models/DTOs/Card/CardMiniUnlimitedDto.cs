using ModelsLeit.DTOs.Collection;
using SharedLeit;

namespace ModelsLeit.DTOs.Card
{
    public record CardMiniUnlimitedDto
    {
        public long Id { get; set; }
        public string Ask { get; set; }
        public string Answer { get; set; }
        public CardStatus Status { get; set; }
        public bool HasMp3 { get; set; }
        public CollectionMiniDto Collection { get; set; }
    }
}
