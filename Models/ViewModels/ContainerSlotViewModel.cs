namespace ModelsLeit.ViewModels
{
    public record ContainerSlotViewModel
    {
        public long Id { get; set; }
        public int Order { get; set; }
        public long BoxId {  get; set; }
    }
}