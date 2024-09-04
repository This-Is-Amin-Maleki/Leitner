using SharedLeit;

namespace ModelsLeit.DTOs.Collection
{
    public record CollectionModifyDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long BoxCount { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; }
        public CollectionStatus Status { get; set; }
    }
}
