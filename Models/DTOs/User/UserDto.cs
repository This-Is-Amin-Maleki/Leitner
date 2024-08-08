namespace ModelsLeit.DTOs.User
{
    public record UserDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool TwoFactorEnabled {  get; set; }
        public bool LockoutEnabled { get;set; }
        public int AccessFailedCount {  get; set; }
        public bool Active { get; set; }
    }
}
