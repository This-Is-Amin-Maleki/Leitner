using DataAccessLeit.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ModelsLeit.DTOs.User;
using ModelsLeit.Entities;
using ModelsLeit.ViewModels.User;
using ServicesLeit.Interfaces;
using SharedLeit;
using System.Data;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;

namespace ServicesLeit.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly UrlEncoder _urlEncoder;
        public UserService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<UserRole> roleManager, UrlEncoder urlEncoder, ILogger<UserService> logger)
        {
            _urlEncoder = urlEncoder;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<UserRegisterDto> RegisterAsync(UserRegisterViewModel model)
        {
            UserRegisterDto output = new();
            var user = await _userManager.Users
                    .FirstOrDefaultAsync(x =>
                    (model.Phone != null && x.PhoneNumber == model.Phone) ||
                    x.Email == model.Email);
            if (user is not null)
            {
                output.Result = RegisterResult.EmailOrPhoneInUse;
                return output;
            }

            user = new()
            {
                Active = model.Active,
                Email = model.Email,
                PhoneNumber = model.Phone,
                UserName = model.UserName,
                Name = model.Name,
                Bio = string.Empty,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                output.Result = RegisterResult.Fail;
                return output;
            }

            if (model.Email is not null)
            {
                var mailToken = await EmailTokenGeneratorAsync(model.Email);
                if (mailToken is null)
                {
                    return output;
                }
                output.Result = RegisterResult.Success;
                output.Email = model.Email;
                output.EmailToken = mailToken.Token;
            }

            if (model.Phone is not null)
            {
                var phoneToken = await PhoneTokenGeneratorAsync(model.Phone);
                if (phoneToken is null)
                {
                    return output;
                }
                output.Result = RegisterResult.Success;
                output.Phone = model.Phone;
                output.PhoneToken = phoneToken.Token;
            }


            result = await _userManager.AddToRoleAsync(user, model.Type.ToString());
            if (!result.Succeeded)
            {
                result = await _userManager.DeleteAsync(user);
                output.Result = RegisterResult.RoleFail;
                return output;
            }

            if (!result.Succeeded) //when addRole Faild and can't delete registered User 
            {
                output.Result = RegisterResult.RoleAndDeleteFail;
            }

            return output;
        }

        public async Task<List<UserListDto>> ReadAllAsync(bool? active = null, UserType? type = null)
        {
            if (type is null)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(nameof(type));
                //use auto mapper
                return usersInRole
                    .Where(x => (active != null && x.Active == active))
                    .Select(x => new UserListDto
                    {
                        Id = x.Id,
                        Name = x.UserName,
                        Email = x.Email,
                    })
                    .ToList();
            }
            return await _userManager.Users
                .Where(x => (active != null && x.Active == active))
                .Select(x => new UserListDto
                {
                    Id = x.Id,
                    Name = x.UserName,
                    Email = x.Email,
                })
                .ToListAsync();
        }

        public async Task<UserDto> ReadAsync(long id)
        {//use auto mapper

            //use auto mapper
            return await _userManager.Users
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    Name = x.UserName,
                    Email = x.Email,
                    Phone = x.PhoneNumber,
                    TwoFactorEnabled = x.TwoFactorEnabled,
                    LockoutEnabled = x.LockoutEnabled,
                    AccessFailedCount = x.AccessFailedCount,
                    Active = x.Active,
                })
                .FirstAsync(x => x.Id == id);
        }

        public async Task<UserModifyDto?> ModifyLimitedAsync(UserModifyLimitedViewModel model)
        {
            UserModifyDto? output = null;
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                return output;
            }

            ApplicationUser userModified = user;
            userModified.Name = model.Name ?? user.Name;
            userModified.Bio = model.Bio ?? user.Bio;

            var result = await _userManager.UpdateAsync(userModified);
            if (!result.Succeeded)
            {
                return output;
            }

            output = new();

            if (user.Email != model.Email)
            {
                output.Email = model.Email;
                output.EmailToken = await _userManager.GenerateChangeEmailTokenAsync(user, model.Email);
            }

            if (user.PhoneNumber != model.Phone)
            {
                output.Phone = model.Phone;
                output.PhoneToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.Phone);
            }

            if (!(model.Password.IsNullOrEmpty() &&
                model.NewPassword.IsNullOrEmpty()))
            {
                var passResult = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
                if (passResult.Succeeded)
                {
                    output.PassChanged = true;
                }
            }
            return output;
        }
        public async Task<bool> ModifyAsync(UserModifyViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                return false;
            }

            ApplicationUser userModified = new()
            {
                Bio = model.Bio ?? user.Bio,
                Name = model.Name ?? user.Name,
                TwoFactorEnabled = model.TwoFactorAuthentication ?? user.TwoFactorEnabled,
                Active = model.Active ?? user.Active,
                Email = model.Email ?? user.Email,
                PhoneNumber = model.Phone ?? user.PhoneNumber,
                UserName = model.UserName ?? user.UserName,
                EmailConfirmed = model.EmailConfirmed ?? user.EmailConfirmed,
                PhoneNumberConfirmed = model.PhoneConfirmed ?? user.PhoneNumberConfirmed,
                LockoutEnabled = model.LockoutEnabled ?? user.LockoutEnabled,
                LockoutEnd = model.LockoutEnd ?? user.LockoutEnd,
            };

            var result = await _userManager.UpdateAsync(userModified);
            if (result.Succeeded)
            {
                UserModifyRoleDto roleModifier = new()
            {
                    Id = model.Id,
                    Type = model.Type,
                };
                ModifyRoleAsync(roleModifier);
            }
            return true;
        }
        public async Task<bool> ModifyRoleAsync(UserModifyRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                return false;
            }
            IdentityResult? result =new();
            var oldRoles = await _userManager.GetRolesAsync(user);
            if (oldRoles.Any())
            {
                result = await _userManager.RemoveFromRolesAsync(user, oldRoles);
            }
            if (!result.Succeeded)
            {
                return false;
            }
            result = await _userManager.AddToRoleAsync(user, model.Type.ToString());
            if (!result.Succeeded)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> ModifyRolesAsync(UserModifyRolesDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                return false;
            }
            IdentityResult? result;
            var oldRoles = await _userManager.GetRolesAsync(user);
            result = await _userManager.RemoveFromRolesAsync(user, oldRoles);
            if (!result.Succeeded)
            {
                return false;
            }
            result = await _userManager.AddToRoleAsync(user, nameof(model.Type));
            if (!result.Succeeded)
            {
                return false;
            }
            return true;
        }

        public async Task<EmailTokenViewModel?> PhoneTokenGeneratorAsync(string phone)
        {
            EmailTokenViewModel? model = null;
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            if (user is null)
            {
                return model;
            }
            var confirmationCode = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phone);
            model = new()
            {
                Identifier = phone,
                Token = confirmationCode
            };
            return model;
        }
        public async Task<EmailTokenViewModel?> EmailTokenGeneratorAsync(string email)
        {
            EmailTokenViewModel? model = null;
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return model;
            }
            var confirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            model = new()
            {
                Identifier = email,
                Token = confirmationCode
            };
            return model;
        }

        /// <summary>
        /// out is null so its success, empty is fail, have char mean error
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string?> EmailConfirmAsync(EmailTokenViewModel model)
        {
            string output = string.Empty;
            var user = await _userManager.FindByEmailAsync(model.Identifier);
            if (user is null)
            {
                return output;
            }
            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            if (result.Errors.Any())
            {
                return resultErrors(result);
            }
            if (!result.Succeeded)
            {
                return output;
            }
            return null;
        }
        public async Task<bool> PhoneConfirmAsync(EmailTokenViewModel model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.Identifier);
            if (user is null)
            {
                return false;
            }
            var result = await _userManager.VerifyChangePhoneNumberTokenAsync(user, model.Token, model.Identifier);
            if (!result)
            {
                return false;
            }
            return true;
        }
        public async Task<string?> ResetPasswordTokenGeneratorAsync(ResetPasswordRequestViewModel model)
        {
            string? modelView = null;
            var user = await UserGetAsync(model.Identifier, model.Mode);
            if (user is null)
            {
                return modelView;
            }

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<bool> ResetPasswordConfirmAsync(UserResetPasswordViewModel model)
        {
            var output = false;

            if (model.Mode is null)
            {
                return output;
            }

            var user = await UserGetAsync(model.Identifier, (UserCheckMode)model.Mode);

            if (user is null)
            {
                return output;
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (!result.Succeeded)
            {
                return output;
            }

            return true;
        }
        public async Task<bool> ResetPasswordCheckTokenAsync(UserResetPasswordViewModel model)
        {
            bool output = false;
            if (model.Mode is null)
            {
                return output;
            }

            var user = await UserGetAsync(model.Identifier, (UserCheckMode)model.Mode);
            if (user is null)
            {
                return output;
            }

            output = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", model.Token);
            return output;

        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<LoginResult> LoginTwoFactorAsync(PreLoginViewModel model)
        {
            ApplicationUser? user = await UserGetAsync(model.Identifier, model.Mode);

            if (user is null)
            {
                return LoginResult.NotFound;
            }

            if (!user.EmailConfirmed)
            {
                return LoginResult.EmailNotConfirmed;
            }

            if (!user.Active)
            {
                return LoginResult.Deactive;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);

            if (result.IsLockedOut)
            {
                return LoginResult.LockedOut;
            }

            if (result.RequiresTwoFactor && model.Token is null && _signInManager.GetTwoFactorAuthenticationUserAsync() is not null)
            {
                var resultTwoFactor = await TwoFactorCheckAsync(model.Token);
                if (resultTwoFactor is LoginResult.TwoFactorInvalid)
                {
                    return LoginResult.TwoFactorInvalid;
                }
            }

            if (!result.Succeeded)
            {
                return LoginResult.Fail;
            }

            await _signInManager.SignInAsync(user, false);
            return LoginResult.Success;
        }
        public async Task<ApplicationUser?> PreLoginFakeAsync(PreLoginViewModel model)
        {
            var user = await UserGetAsync(model.Identifier, model.Mode);

            return user;
        }
        public async Task<LoginCheckDto> PreLoginAsync(PreLoginViewModel model)
        {
            var user = await UserGetAsync(model.Identifier, model.Mode);

            return new LoginCheckDto
            {
                User = user,
                Password = model.Password,
            };
        }
        public async Task<LoginResult> LoginAsync(LoginCheckDto model)
        {
            if (model.User is null)
            {
                return LoginResult.NotFound;
            }

            if (!model.User.EmailConfirmed)
            {
                return LoginResult.EmailNotConfirmed;
            }

            if (!model.User.Active)
            {
                return LoginResult.Deactive;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(model.User, model.Password, true);

            if (result.IsLockedOut)
            {
                return LoginResult.LockedOut;
            }

            if (result.RequiresTwoFactor)
            {
                return LoginResult.TwoFactorRequire;
            }

            if (!result.Succeeded)
            {
                return LoginResult.Fail;
            }

            await _signInManager.SignInAsync(model.User, false);
            return LoginResult.Success;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal">It's "User" that come from ControllerBase! [HttpContext?.User!]</param>
        /// <returns></returns>
        public async Task<ActiveTFADto?> TwoFactorActivatorAsync(ClaimsPrincipal principal)
        {
            ActiveTFADto? model = null;
            var user = await _userManager.GetUserAsync(principal);
            if (user is null)
            {
                return model;
            }
            string AuthenticationUriFormat = "otpauth://totp/{0}:{1}?secret={2}";
        otpauth://totp/Example:alice@google.com?secret=JBSWY3DPEHPK3PXP&issuer=Example&algorithm=SHA1&digits=6&period=30
            await _userManager.ResetAuthenticatorKeyAsync(user!);
            var token = await _userManager.GetAuthenticatorKeyAsync(user!);
            string AuthenticationUri = string.Format(AuthenticationUriFormat, _urlEncoder.Encode("Leitner"), _urlEncoder.Encode(user!.Email!), token);
            model = new()
            {
                Token = token,
                QRUri = AuthenticationUri
            };
            return model;
        }

        public async Task<bool> TwoFactorConfirmAsync(TFAConfirmViewModel model)
        {
            var user = await _userManager.GetUserAsync(model.Principal);
            if (user is null)
            {
                return false;
            }

            var verify = await _userManager.VerifyTwoFactorTokenAsync(user!, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.Code);
            if (!verify)
            {
                return false;
            }

            var result = await _userManager.SetTwoFactorEnabledAsync(user!, true);
            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal">It's "User" that come from ControllerBase! [HttpContext?.User!]</param>
        /// <returns></returns>
        public async Task<bool> TwoFactorDeactivatorAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user is null)
            {
                return false;
            }

            var result = await _userManager.ResetAuthenticatorKeyAsync(user!);
            if (!result.Succeeded)
            {
                return false;
            }

            result = await _userManager.SetTwoFactorEnabledAsync(user!, false);
            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }

        public async Task<LoginResult> TwoFactorCheckAsync(string token)
        {
            var resultTwoFactor = await _signInManager.TwoFactorAuthenticatorSignInAsync(token, false, false);
            if (resultTwoFactor.Succeeded)
            {
                return LoginResult.Success;
            }
            if (resultTwoFactor.IsLockedOut)
            {
                return LoginResult.LockedOut;
            }
            return LoginResult.TwoFactorInvalid;
        }

        //////////////// Unsuccessful attempt to handle scenarios and create a workaround when 2FA does not work. د:
        public async Task<LoginResult> MyLoginAsync(ApplicationUser? model, string password)
        {
            if (model is null)
                return LoginResult.NotFound;

            if (!model.EmailConfirmed)
                return LoginResult.EmailNotConfirmed;

            if (!model.Active)
                return LoginResult.Deactive;

            if (model.LockoutEnabled && model.LockoutEnd > DateTime.Now)
                return LoginResult.LockedOut;

            if (model.TwoFactorEnabled)
                return LoginResult.TwoFactorRequire;

            var result = await _signInManager.CheckPasswordSignInAsync(model, password, true);

            if (!result.Succeeded)
            {
                return LoginResult.Fail;
            }

            await _signInManager.SignInAsync(model, false);
            return LoginResult.Success;
        }
        public async Task<LoginResult> MyTFA(User2FAViewModel model)
        {
            if (model.User is null)
            {
                return LoginResult.NotFound;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(model.User, model.Password, true);

            SignInResult? resultTwoFactor = null;
            if (result.RequiresTwoFactor || model.User.TwoFactorEnabled)
            {
                resultTwoFactor = await _signInManager.TwoFactorAuthenticatorSignInAsync(model.Token, false, false);

            }

            if (resultTwoFactor is null)
            {
                return LoginResult.TwoFactorInvalid;
            }

            if (result.IsLockedOut || resultTwoFactor.IsLockedOut)
            {
                return LoginResult.LockedOut;
            }

            if (resultTwoFactor.Succeeded)
            {
                await _signInManager.SignInAsync(model.User, false);
                return LoginResult.Success;
            }

            return LoginResult.TwoFactorInvalid;
        }

        ///////////////////////////////////
        private async Task<ApplicationUser?> UserGetAsync(string identifier, UserCheckMode mode)
        {
            var t = (int)mode;

            ApplicationUser? user = null;

            if (identifier.Contains("@") && ((int)mode & 0b001) != 0)
            {
                user = await _userManager.FindByEmailAsync(identifier);
            }
            if (user is null && ((int)mode & 0b100) != 0)
            {
                user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == identifier);
            }
            if (user is null && ((int)mode & 0b010) != 0)
            {
                user = await _userManager.FindByNameAsync(identifier);
            }

            return user;
        }
        private string resultErrors(IdentityResult result)
        {

            StringBuilder errors = new();
            foreach (var e in result.Errors)
            {
                errors.Append(e.Description + ". ");
            }
            var error = errors.ToString();
            return error.Substring(0, error.Length - 2);

        }
    }
}