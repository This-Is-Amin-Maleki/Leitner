using SharedLeit;
using ModelsLeit.ViewModels;

namespace ModelsLeit.ViewModels.User
{
    public class HomePageViewModel
    {
        public UserFormType Type { get; set; } = UserFormType.Login;
        public PreLoginViewModel? Login { get; set; }
        public UserRegisterViewModel? Register { get; set; }
        public ResetPasswordRequestViewModel? Forgot { get; set; }
        public UserResetPasswordViewModel? PasswordReset { get; set; }
    }
}
