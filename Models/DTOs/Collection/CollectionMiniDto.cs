using SharedLeit;

namespace ModelsLeit.DTOs.Collection
{
    public record CollectionMiniDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public CollectionStatus? Status { get; set; }
    }
}
