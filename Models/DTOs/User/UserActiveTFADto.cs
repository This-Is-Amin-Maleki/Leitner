namespace ModelsLeit.DTOs.User
{
    public record UserActiveTFADto
    {
        public string? Token { get; set; }
        public string? QRUri { get; set; }
    }
}
