using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record EmailTokenViewModel
    {
        [Required]
        public string Identifier { get; set; }
        [Required]
        //[StringLength(100, MinimumLength = 8, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string Token { get; set; }
    }
}
