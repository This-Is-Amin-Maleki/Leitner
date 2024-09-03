using SharedLeit;

namespace ModelsLeit.DTOs.Card
{
    public record CardCheckStatusDto
    {
        public long Id { get; set; }
        public CardStatus Status { get; set; }
        public CardStatus DefaultStatus { get; set; }
        public long CollectionId { get; set; }
        public int Skip {  get; set; }

    }
}
