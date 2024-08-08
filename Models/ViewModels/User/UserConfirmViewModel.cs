using SharedLeit;
using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record UserConfirmViewModel
    {
        [Required]
        public UserCheckMode Mode { get; set; }
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
