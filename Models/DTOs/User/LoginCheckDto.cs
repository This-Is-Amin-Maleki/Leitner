using ModelsLeit.Entities;

namespace ModelsLeit.DTOs.User
{
    public record LoginCheckDto
    {
        public ApplicationUser User { get; set; }
        public string Password { get; set; }
    }
}
