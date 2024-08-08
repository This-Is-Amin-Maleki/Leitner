namespace ModelsLeit.DTOs.User
{
    public record UserListDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
