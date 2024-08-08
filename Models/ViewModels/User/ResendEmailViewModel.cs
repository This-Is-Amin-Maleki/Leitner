using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record ResendEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Message { get; set; }
    }
}
