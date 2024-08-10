using ModelsLeit.DTOs.Collection;
using ModelsLeit.DTOs.User;

namespace ModelsLeit.DTOs.Card
{
    public record CardDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Ask { get; set; }
        public string Answer { get; set; }
        public string Description { get; set; }
        public bool HasMp3 { get; set; }
        public CollectionMiniDto Collection { get; set; }
    }
}
