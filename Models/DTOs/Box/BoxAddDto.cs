namespace ModelsLeit.DTOs.Box
{
    public record BoxAddDto
    {
        public string? Name {  get; set; }
        public string? Description { get; set; }
        public long CollectionId { get; set; }
        public long UserId { get; set; }
        public int CardPerDay { get; set; }
    }
}
