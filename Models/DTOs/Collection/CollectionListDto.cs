using ModelsLeit.DTOs.User;
using SharedLeit;

namespace ModelsLeit.DTOs.Collection
{
    public record CollectionListDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public DateTime PublishedDate { get; set; }
        public CollectionStatus Status { get; set; }
        public UserMiniDto User { get; set; }

        public long CardsQ { get; set; }
        public long BoxCount { get; set; }
    }
}
