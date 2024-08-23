using ModelsLeit.DTOs.Collection;

namespace ModelsLeit.DTOs.Card
{
    public record CardCheckedResultDto
    {
        public CollectionMiniDto Collection { get; set; }
        public long CheckedCards { get; set; }
    }
}