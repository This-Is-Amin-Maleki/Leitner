using ModelsLeit.Entities;
using SharedLeit;
using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record User2FAViewModel
    {
        public UserCheckMode Mode { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string Password { get; set; }
        public ApplicationUser? User {  get; set; }
    }
}
