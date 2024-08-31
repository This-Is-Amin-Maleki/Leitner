using SharedLeit;

namespace ModelsLeit.DTOs.Card
{
    public record CardCheckDto
    {
        public long Id { get; set; }
        public string Ask { get; set; }
        public string Answer { get; set; }
        public string Description { get; set; }
        public bool HasMp3 { get; set; }
        public CardStatus Status { get; set; }
        public long CollectionId { get; set; }
    }
}
