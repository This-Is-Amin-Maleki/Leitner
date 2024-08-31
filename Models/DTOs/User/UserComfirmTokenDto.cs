using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.DTOs.User
{
    public record UserComfirmTokenDto
    {
        public string Identifier { get; set; }
        public string Token { get; set; }
        public bool IsPassResetRequest { get; set; }
    }
}
