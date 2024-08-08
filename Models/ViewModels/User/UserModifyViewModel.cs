using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record UserModifyViewModel
    {
        public long Id { get; set; }
        [Length(5, 25)]
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool? Active { get; set; }
        [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string? Password { get; set; }
        [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string? NewPassword { get; set; }
    }
}
