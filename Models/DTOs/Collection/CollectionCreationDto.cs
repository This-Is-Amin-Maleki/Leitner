using SharedLeit;

namespace ModelsLeit.DTOs.Collection
{
    public record CollectionCreationDto
    {
        public long UserId { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; }
    }
}
