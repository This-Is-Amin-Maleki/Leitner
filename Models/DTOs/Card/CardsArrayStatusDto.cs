namespace ModelsLeit.DTOs.Card
{
    public record CardsArrayStatusDto
    {
        public long Id { get; set; }
        public long[]? Approved { get; set; }
        public long[]? Submited { get; set; }
        public long[]? Blocked { get; set; }
        public long[]? Rejected { get; set; }
    }
}