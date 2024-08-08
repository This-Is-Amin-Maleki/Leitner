using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record UserTokenViewModel
    {
        [Required]
        public string Token { get; set; }
    }
}
