namespace ModelsLeit.ViewModels.User
{
    public record LoginResultViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
        public string Button { get; set; } = "Back to Login";
        public string Destination { get; set; } = "#Login";
    }
}
