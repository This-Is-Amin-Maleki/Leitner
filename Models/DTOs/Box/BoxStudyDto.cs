namespace ModelsLeit.DTOs.Box   
{
    public record BoxStudyDto
    {
        public long Id { get; set; }
        public int CardPerDay { get; set; }
        public int LastSlot { get; set; }
        public long LastCardId { get; set; }
        public long CollectionId { get; set; }
        public DateTime DateStudied { get; set; }
    }
}
