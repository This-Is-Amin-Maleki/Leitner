using ModelsLeit.DTOs.User;
using ModelsLeit.ViewModels.User;
using SharedLeit;
using System.Security.Claims;

namespace ServicesLeit.Interfaces
{
    public interface IUserService
    {
        Task<UserRegisterDto> RegisterAsync(UserRegisterViewModel model);

        Task<List<UserListDto>> ReadAllAsync(bool active);

        Task<UserDto> ReadAsync(long id);

        Task<UserModifyDto?> ModifyAsync(UserModifyViewModel model);

        Task<bool> ModifyRoleAsync(UserModifyRoleDto model);
        Task<bool> ModifyRolesAsync(UserModifyRolesDto model);
        Task<EmailTokenViewModel?> PhoneTokenGeneratorAsync(string phone);
        Task<EmailTokenViewModel?> EmailTokenGeneratorAsync(string email);
        Task<string?> EmailConfirmAsync(EmailTokenViewModel model);
        Task<bool> PhoneConfirmAsync(EmailTokenViewModel model);
        Task<string?> ResetPasswordTokenGeneratorAsync(ResetPasswordRequestViewModel model);
        Task<bool> ResetPasswordConfirmAsync(UserResetPasswordViewModel model);
        Task<bool> ResetPasswordCheckTokenAsync(UserResetPasswordViewModel model);
        Task LogoutAsync();
        Task<LoginResult> LoginTwoFactorAsync(PreLoginViewModel model);
        Task<LoginCheckDto> PreLoginAsync(PreLoginViewModel model);
        Task<LoginResult> LoginAsync(LoginCheckDto model);
        Task<ActiveTFADto?> TwoFactorActivatorAsync(ClaimsPrincipal principal);
        Task<bool> TwoFactorConfirmAsync(TFAConfirmViewModel model);
        Task<bool> TwoFactorDeactivatorAsync(ClaimsPrincipal principal);
        Task<LoginResult> TwoFactorCheckAsync(string token);
    }
}