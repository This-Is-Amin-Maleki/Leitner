using SharedLeit;

namespace ModelsLeit.DTOs.User
{
    public record UserMiniDto

    {
        public long Id { get; set; }
        public string UserName { get; set; }
    }
}
