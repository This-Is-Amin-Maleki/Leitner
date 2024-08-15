using SharedLeit;
using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record PEditUserViewModel
    {
        public long Id { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} - {1} - {2} Characters")]
        [Required]
        public string Name { get; set; }

        [StringLength(255, MinimumLength = 0, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string Bio { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string Password { get; set; }
    }
}
