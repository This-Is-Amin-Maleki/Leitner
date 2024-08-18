using SharedLeit;
using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record PreLoginViewModel
    {
        public UserCheckMode Mode { get; set; }
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Token { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
