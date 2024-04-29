namespace ModelsLeit.ViewModels
{
    public record BoxAddViewModel
    {
        public string? Name {  get; set; }
        public string? Description { get; set; }
        public long CollectionId { get; set; }
        public int CardPerDay { get; set; }
    }
}
