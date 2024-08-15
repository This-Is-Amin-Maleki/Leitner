using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ModelsLeit.ViewModels.User
{
    public record TFAConfirmViewModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public ClaimsPrincipal Principal {  get; set; }
    }
}
