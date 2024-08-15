namespace ModelsLeit.DTOs.User
{
    public record ActiveTFADto
    {
        public string? Token { get; set; }
        public string? QRUri { get; set; }
    }
}
