using SharedLeit;
using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record UserRegisterViewModel
    {
        [Required]
        [Length(5, 25)]
        public string UserName { get; set; }

        [Required]
        [Length(5, 100)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Phone { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string Password { get; set; }
        public bool Active { get; set; }
        [Required]
        public UserType Type { get; set; }
        public string? Domain { get; set; }
    }
}
