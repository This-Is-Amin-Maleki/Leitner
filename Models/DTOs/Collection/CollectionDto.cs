using SharedLeit;

namespace ModelsLeit.DTOs.Collection
{
    public record CollectionDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public CollectionStatus Status { get; set; }

        public long CardsQ { get; set; }
        public long BoxCount { get; set; }
    }
}
