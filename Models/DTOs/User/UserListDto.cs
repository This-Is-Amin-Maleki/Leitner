namespace ModelsLeit.DTOs.User
{
    public record UserListDto
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneConfirmed { get; set; }
    }
}
