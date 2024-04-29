namespace ModelsLeit.ViewModels
{
    public record CardViewModel
    {
        public long Id { get; set; }
        public string Ask { get; set; }
        public string Answer { get; set; }
        public string Description { get; set; }
        public bool HasMp3 { get; set; }
        public CollectionMiniViewModel Collection { get; set; }
    }
}
