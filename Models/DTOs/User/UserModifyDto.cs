namespace ModelsLeit.DTOs.User
{
    public record UserModifyDto
    {
        public bool PassChanged { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? EmailToken { get; set; }
        public string? PhoneToken { get; set; }
    }
}
