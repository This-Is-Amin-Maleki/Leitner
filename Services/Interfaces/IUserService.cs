using ModelsLeit.DTOs.User;
using ModelsLeit.Entities;
using ModelsLeit.ViewModels.User;
using SharedLeit;
using System.Security.Claims;

namespace ServicesLeit.Interfaces
{
    public interface IUserService
    {
        Task<UserComfirmResultDto> ConfirmationAsync(string input);
        Task<LoginResult> LoginAsync(UserLoginCheckDto model);
        Task<LoginResult> LoginTwoFactorAsync(PreLoginViewModel model);
        Task LogoutAsync();
        Task<bool> ModifyAsync(UserModifyViewModel model);
        Task<bool?> ModifyLimitedAsync(UserModifyLimitedViewModel model);
        Task<bool> ModifyRoleAsync(UserModifyRoleDto model);
        Task<bool> ModifyRolesAsync(UserModifyRolesDto model);
        Task<LoginResult> MyLoginAsync(ApplicationUser? model, string password);
        Task<LoginResult> MyTFA(User2FAViewModel model);
        Task<bool> PhoneConfirmAsync(UserComfirmTokenDto model);
        Task<UserLoginCheckDto> PreLoginAsync(PreLoginViewModel model);
        Task<ApplicationUser?> PreLoginFakeAsync(PreLoginViewModel model);
        Task<List<UserListDto>> ReadAllAsync(bool? active = null, UserType? type = null);
        Task<UserRegisterDto> RegisterAsync(UserRegisterViewModel model);
        Task<bool> ResetPasswordConfirmAsync(UserResetPasswordViewModel model);
        Task SendPasswordResetTokenAsync(ResetPasswordRequestViewModel model);
        Task<UserActiveTFADto?> TwoFactorActivatorAsync(ClaimsPrincipal principal);
        Task<LoginResult> TwoFactorCheckAsync(string token);
        Task<bool> TwoFactorConfirmAsync(TFAConfirmViewModel model);
        Task<bool> TwoFactorDeactivatorAsync(ClaimsPrincipal principal);
    }
}