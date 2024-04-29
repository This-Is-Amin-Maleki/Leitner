using SharedLeit;

namespace ModelsLeit.ViewModels
{
    public record CollectionMiniViewModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public CollectionStatus? Status { get; set; }
    }
}
