namespace ModelsLeit.DTOs.Collection
{
    public record CollectionShowDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; }

        public long CardsQ { get; set; }

        public string UserFullName {  get; set; }
        public string UserName {  get; set; }
    }
}
