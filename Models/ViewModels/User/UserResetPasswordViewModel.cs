using SharedLeit;
using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record UserResetPasswordViewModel
    {
        public UserCheckMode? Mode { get; set; }
        [Required]
        public string Identifier { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string Password { get; set; }
        [Required]
        //[StringLength(100, MinimumLength = 8, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string Token { get; set; }
    }
}
