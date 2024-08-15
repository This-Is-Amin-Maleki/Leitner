using ModelsLeit.DTOs.User;
using ModelsLeit.ViewModels.User;
using SharedLeit;
using System.Security.Claims;

namespace ServicesLeit.Interfaces
{
    public interface IUserService
    {
        Task<string?> EmailConfirmAsync(EmailTokenViewModel model);
        Task<EmailTokenViewModel?> EmailTokenGeneratorAsync(string email);
        Task<LoginResult> LoginAsync(LoginCheckDto model);
        Task<LoginResult> LoginTwoFactorAsync(PreLoginViewModel model);
        Task LogoutAsync();
        Task<bool> ModifyAsync(UserModifyViewModel model);
        Task<UserModifyDto?> ModifyLimitedAsync(UserModifyLimitedViewModel model);
        Task<bool> ModifyRoleAsync(UserModifyRoleDto model);
        Task<bool> ModifyRolesAsync(UserModifyRolesDto model);
        Task<bool> PhoneConfirmAsync(EmailTokenViewModel model);
        Task<EmailTokenViewModel?> PhoneTokenGeneratorAsync(string phone);
        Task<LoginCheckDto> PreLoginAsync(PreLoginViewModel model);
        Task<List<UserListDto>> ReadAllAsync(bool? active = null, UserType? type = null);
        Task<UserDto> ReadAsync(long id);
        Task<UserRegisterDto> RegisterAsync(UserRegisterViewModel model);
        Task<bool> ResetPasswordCheckTokenAsync(UserResetPasswordViewModel model);
        Task<bool> ResetPasswordConfirmAsync(UserResetPasswordViewModel model);
        Task<string?> ResetPasswordTokenGeneratorAsync(ResetPasswordRequestViewModel model);
        Task<ActiveTFADto?> TwoFactorActivatorAsync(ClaimsPrincipal principal);
        Task<LoginResult> TwoFactorCheckAsync(string token);
        Task<bool> TwoFactorConfirmAsync(TFAConfirmViewModel model);
        Task<bool> TwoFactorDeactivatorAsync(ClaimsPrincipal principal);
    }
}