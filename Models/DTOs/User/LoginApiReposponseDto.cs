using SharedLeit;

namespace ModelsLeit.DTOs.User
{
    public record LoginApiReposponseDto
    {
        public string JwtToken { get; set; }
        public LoginResult Result { get; set; }
    }
}
