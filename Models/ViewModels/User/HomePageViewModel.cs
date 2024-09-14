using SharedLeit;
using ModelsLeit.ViewModels;
using ModelsLeit.DTOs.Collection;

namespace ModelsLeit.ViewModels.User
{
    public class HomePageViewModel
    {
        public string? Error { get; set; } 
        public string? ErrorTitle { get; set; }
        public string? Message { get; set; }
        public UserFormType Type { get; set; } = UserFormType.Login;
        public PreLoginViewModel? Login { get; set; } = new();
        public UserRegisterViewModel? Register { get; set; } = new();
        public ResetPasswordRequestViewModel? Forgot { get; set; } = new();
        public UserResetPasswordViewModel? PasswordReset { get; set; } = new();
        public ResendEmailViewModel? ResendEmail { get; set; } = new();
        public IList<CollectionShowDto> Collections { get; set; }
    }
}
