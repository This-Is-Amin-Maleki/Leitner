namespace ModelsLeit.DTOs.User
{
    public record Authentication2FactorActivatorDto
    {
        public string? Token { get; set; }
        public string? QRUri { get; set; }
    }
}
