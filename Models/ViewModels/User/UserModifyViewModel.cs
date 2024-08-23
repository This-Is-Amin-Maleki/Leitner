using SharedLeit;
using System.ComponentModel.DataAnnotations;

namespace ModelsLeit.ViewModels.User
{
    public record UserModifyViewModel
    {
        public long Id { get; set; }
        [Length(5, 25)]
        public string UserName { get; set; }
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Phone { get; set; }
        public bool PhoneConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd {  get; set; }
        public bool Active { get; set; }
        public bool TwoFactorAuthentication { get; set; }

        [StringLength(255, MinimumLength = 0, ErrorMessage = "{0} - {1} - {2} Characters")]
        public string Bio { get; set; }
        public UserType Type { get; set; }

        public UserType ParameterType { get; set; }
        public bool ParameterActive { get; set; }
    }
}
