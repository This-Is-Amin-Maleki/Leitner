namespace ModelsLeit.DTOs.Card
{
    public record CardsListStatusDto
    {
        public long Id { get; set; }
        public string? CollectionName { get; set; }
        public List<CardCheckDto> Submitted { get; set; }
        public List<CardCheckDto> Approved { get; set; }
        public List<CardCheckDto> Blocked { get; set; }
        public List<CardCheckDto> Rejected { get; set; }        

    }
}
