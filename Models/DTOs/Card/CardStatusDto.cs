using SharedLeit;

namespace ModelsLeit.DTOs.Card
{
    public record CardStatusDto
    {
        public long Id { get; set; }
        public CardStatus Status { get; set; }
    }
}
