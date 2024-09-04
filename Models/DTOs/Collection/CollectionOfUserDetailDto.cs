using SharedLeit;

namespace ModelsLeit.DTOs.Collection
{
    public record CollectionOfUserDetailDto
    {
        public long[] Boxes { get; set; }
        public string? Bio { get; set; }
        public List<CollectionShowDto> Collections { get; set; }
    }
}
