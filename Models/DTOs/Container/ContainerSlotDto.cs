namespace ModelsLeit.DTOs.Container
{
    public record ContainerSlotDto
    {
        public long Id { get; set; }
        public int Order { get; set; }
        public long BoxId {  get; set; }
    }
}