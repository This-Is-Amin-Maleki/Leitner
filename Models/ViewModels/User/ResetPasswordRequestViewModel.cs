using SharedLeit;
using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record ResetPasswordRequestViewModel
    {
        public UserCheckMode Mode { get; set; }
        [Required]
        [EmailAddress]
        public string Identifier { get; set; }
        [Required]
        public string Domain { get; set; }
    }
}
