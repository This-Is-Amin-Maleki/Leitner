using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record UserModifyLimitedViewModel
    {
        public long Id { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string? NewPassword { get; set; }    [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string? Name { get; set; }
        [StringLength(255, MinimumLength = 0, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string? Bio { get; set; }
    }
    public record UserModifyProfileLimitedViewModel
    {
        public long Id { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string? Name { get; set; }
        [StringLength(255, MinimumLength = 0, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string? Bio { get; set; }
    }
    public record UserChangePasswordLimitedViewModel
    {
        public long Id { get; set; }
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string? NewPassword { get; set; }
    }
}
