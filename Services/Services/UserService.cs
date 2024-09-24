using DataAccessLeit.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ModelsLeit.DTOs.User;
using ModelsLeit.Entities;
using ModelsLeit.ViewModels.User;
using NetTopologySuite.Densify;
using RTools_NTS.Util;
using ServicesLeit.Interfaces;
using SharedLeit;
using System.Data;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

/*/////////////
checked Register and email is verified
    OK => 2FA Check

    NO => Go Ahead for others
    */
namespace ServicesLeit.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly NotificationService _notificationService;
        private readonly ITokenService _tokenService;
        public UserService(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<long>> roleManager,
            UrlEncoder urlEncoder,
            ILogger<UserService> logger,
            NotificationService notificationService,
            ITokenService tokenService)
        {
            _urlEncoder = urlEncoder;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _notificationService = notificationService;
            _tokenService = tokenService;
        }

        public async Task<UserRegisterDto> RegisterAsync(UserRegisterViewModel model)
        {
            model.Type = UserType.User;
            model.Active = true;

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

            result = await _userManager.AddToRoleAsync(user, model.Type.ToString());
            if (!result.Succeeded)
            {
                result = await _userManager.DeleteAsync(user);
                output.Result = RegisterResult.RoleFail;
                return output;
            }

            if (model.Email is not null)
            {
                await SendEmailConfirmTokenAsync(user, model.Domain);
            }

            if (model.Phone is not null)
            {
                await SendPhoneConfirmTokenAsync(user);
            }

            if (!result.Succeeded) //when addRole Faild and can't delete registered User 
            {
                output.Result = RegisterResult.RoleAndDeleteFail;
            }
            output.Result = RegisterResult.Success;
            return output;
        }
        public async Task<List<UserListDto>> ReadAllAsync(bool? active = null, UserType? type = null)
        {
            IEnumerable<ApplicationUser> users = await AllUsersOrUsersInRole(type);

            if (active is null)
            {
                return users
                    .OrderByDescending(x => x.Id)
                    .Select(x => new UserListDto
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        Email = x.Email,
                        EmailConfirmed = x.EmailConfirmed,
                        Phone = x.PhoneNumber,
                        PhoneConfirmed = x.PhoneNumberConfirmed,
                    })
                    .ToList();
            }

            return users
                .Where(x => (x.Active == active))
                .OrderByDescending(x => x.Id)
                .Select(x => new UserListDto
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    EmailConfirmed = x.EmailConfirmed,
                    Phone = x.PhoneNumber,
                    PhoneConfirmed = x.PhoneNumberConfirmed,
                })
                .ToList();
        }

        public async Task<IdentityResult?> ChangePasswordLimitedAsync(UserChangePasswordLimitedViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                return null;
            }

            return await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
        }

        public async Task<bool?> ModifyLimitedAsync(UserModifyLimitedViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                return null;
            }

            ApplicationUser userModified = user;
            userModified.Name = model.Name ?? user.Name;
            userModified.Bio = model.Bio ?? user.Bio;

            var result = await _userManager.UpdateAsync(userModified);
            if (!result.Succeeded)
            {
                return false;
            }

            if (user.Email != model.Email)
            {
                //send Email Confirm
            }

            if (user.PhoneNumber != model.Phone)
            {
                //send Sms Confirm
            }

            if (!(model.Password.IsNullOrEmpty() &&
                model.NewPassword.IsNullOrEmpty()))
            {
                var passResult = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
            }
            return true;
        }
        public async Task<bool?> ModifyProfileLimitedAsync(UserModifyProfileLimitedViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                return null;
            }

            ApplicationUser userModified = user;
            userModified.Name = model.Name ?? user.Name;
            userModified.Bio = model.Bio ?? user.Bio;

            var result = await _userManager.UpdateAsync(userModified);
            if (!result.Succeeded)
            {
                return false;
            }

            if (user.Email != model.Email)
            {
                //send Email Confirm
            }

            if (user.PhoneNumber != model.Phone)
            {
                //send Sms Confirm
            }

            return true;
        }
        public async Task<bool> ModifyAsync(UserModifyViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                return false;
            }

            user.Bio = model.Bio is "0" ? string.Empty : model.Bio ?? user.Bio;
            user.Email = model.Email is "0@0.0" ? string.Empty : model.Email ?? user.Email;
            user.PhoneNumber = model.Phone is "0" ? string.Empty : model.Phone ?? user.PhoneNumber;

            user.Name = model.Name ?? user.Name;
            user.TwoFactorEnabled = model.TwoFactorAuthentication;
            user.Active = model.Active;
            user.UserName = model.UserName ?? user.UserName;
            user.EmailConfirmed = model.EmailConfirmed;
            user.PhoneNumberConfirmed = model.PhoneConfirmed;
            user.LockoutEnabled = model.LockoutEnabled;
            user.LockoutEnd = model.LockoutEnd ?? user.LockoutEnd;

            var result = await _userManager.UpdateAsync(user);
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
#warning Ein Problem über Role Catastrofe!!
            return true;
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                return false;
            }
            IdentityResult? result = new();
            var oldRoles = await _userManager.GetRolesAsync(user);
            if (oldRoles.Count > 0)
            {
                try
                {
                    foreach (var role in oldRoles)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }
                catch (Exception ex)
                {
                    var rolesInTable = _roleManager.Roles.Select(x => Convert.ToInt64(x.Id));
                    var toDelUserRole = _dbContext.UserRoles.Where(x => rolesInTable.Contains(x.RoleId) && x.UserId == user.Id);
                    foreach (var role in toDelUserRole)
                    {
                        _dbContext.UserRoles.Remove(role);
                    }
                }
            }
            if (!result.Succeeded)
            {
                return false;
            }
            try
            {
                result = await _userManager.AddToRoleAsync(user, model.Type.ToString());
            }
            catch (Exception ex)
            {
            }
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

        /// <summary>
        /// out is null so its success, empty is fail, have char mean error
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /*
        public async Task<ComfirmationResult> EmailConfirmAsync(ComfirmTokenDto model)
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
        }*/
        public async Task<UserComfirmResultDto> ConfirmationAsync(string input)
        {
            var model = await TokenReaderAsync(input);

            var user = await _userManager.FindByEmailAsync(model.identifier);
            if (user is null)
            {
                return new()
                {
                    Errors = new() { "User not found!" },
                    Status = ComfirmationStatus.Fail,
                };
            }

            if (!model.isPassResetRequest)
            {
                return await ConfirmEmailAsync(user, model.token);
            }

            return await ConfirmResetPasswordAsync(user, model.identifier, model.token);
        }

        public async Task<bool> PhoneConfirmAsync(UserComfirmTokenDto model)
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
        public async Task SendPasswordResetTokenAsync(ResetPasswordRequestViewModel model)
        {
            string? modelView = null;
            var user = await UserGetAsync(model.Identifier, model.Mode);
            if (user is null)
            {
                throw new Exception("Token generation failed.");
            }
#warning REMOVE
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var linkToken = TokenCreator(user.Email, token);

            string title = "Password Reset Confirmation for Your Leitner Account";
            string link = $"{model.Domain}/Confirm/i{linkToken}";
            string body =
                $"<p>Hi Dear {user.Name},<br>" +
                " it looks like you’ve requested a password reset. Please verify your request by clicking the link below:<br>" +
                $"<center><a href=\"{link}\">{link}</a></center><br><br>" +
                "<small>If the link isn’t clickable, try copying and pasting it into your browser's address bar.</small><br>" +
                "If you didn't request a password reset, please ignore this email." +
                "<br>Best regards,<br>Leitner Team</p>";

            await _notificationService.SendEmailAsync(user.Email, title, body);
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
        public async Task<UserLoginCheckDto> PreLoginAsync(PreLoginViewModel model)
        {
            var user = await UserGetAsync(model.Identifier, model.Mode);

            return new UserLoginCheckDto
            {
                User = user,
                Password = model.Password,
            };
        }
        public async Task<LoginResult> LoginAsync(UserLoginCheckDto model)
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

        public async Task<LoginApiReposponseDto> LoginApiAsync(UserLoginCheckDto model)
        {

            var responce = new LoginApiReposponseDto();

            if (model.User is null)
            {
                responce.Result = LoginResult.NotFound;
            }

            if (!model.User.EmailConfirmed)
            {
                responce.Result = LoginResult.EmailNotConfirmed;
            }

            if (!model.User.Active)
            {
                responce.Result = LoginResult.Deactive;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(model.User, model.Password, true);

            if (result.IsLockedOut)
            {
                responce.Result = LoginResult.LockedOut;
            }

            if (result.RequiresTwoFactor)
            {
                responce.Result = LoginResult.TwoFactorRequire;
            }

            if (!result.Succeeded)
            {
                responce.Result = LoginResult.Fail;
            }

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(model.User, false);

                List<string> roles = _roleManager.Roles.Select(x => x.Name).ToList();

                var jwsToken = _tokenService.CreateJWTToken(model.User, roles);

                responce.JwtToken = jwsToken;
                responce.Result = LoginResult.Success;
            }

            return responce;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal">It's "User" that come from ControllerBase! [HttpContext?.User!]</param>
        /// <returns></returns>
        public async Task<UserActiveTFADto?> TwoFactorActivatorAsync(ClaimsPrincipal principal)
        {
            UserActiveTFADto? model = null;
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

        private string TokenCreator(string email, string token) =>
            EncodeToBase64(email + "|" + token)
                .Replace("/", "_")
                .Replace("+", "-")
                .Replace("=", "!");
        private async Task<(string identifier, string token, bool isPassResetRequest)> TokenReaderAsync(string input)
        {
            bool passReset = char.ToLower(input[0]) is 'i';
            var tempA = DecodeBase64(input.
                Substring(1)
                .Replace("_", "/")
                .Replace("-", "+")
                .Replace("!", "="));
            var tempB = tempA.Split('|');
            string identifier = tempB[0];
            string token = tempB[1];
            return (identifier, token, passReset);
        }

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

        private async Task SendPhoneConfirmTokenAsync(ApplicationUser user)
        {/*
            EmailTokenViewModel? model = null;
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            if (user is null)
            {
                return model;
            }*/

            if (user is null || user.PhoneNumber is null)
            {
                throw new Exception("The phone number is not valid.");
            }

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

            if (token is null)
            {
                throw new Exception("Token generation failed.");
            }

            string sms = $"Hi {user.Name}, your registration code is:\r\n{token}." +
                "\r\n Please enter this code to complete your registration." +
                "\r\n If you did not request this, please ignore this message.";

            //await _notificationService.SendEmailAsync(user.Email, body);
        }
        private async Task SendEmailConfirmTokenAsync(ApplicationUser user, string domain)
        {
            if (user is null || user.Email is null)
            {
                throw new Exception("The email is not valid.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            if (token is null)
            {
                throw new Exception("Token generation failed.");
            }
            string linkToken = TokenCreator(user.Email, token);
            string title = "Welcome to Leitner! Please Verify Your Email Address";
            string link = $"{domain}/Confirm/t{linkToken}";
            string body = $"<p>Hi Dear {user.Name},<br>" +
                " welcome to Leitner! Please verify your email by clicking the link below to complete your registration:<br>" +
                $"<center><a href=\"{link}\">{link}</a></center><br><br>" +
                "<small>If the above link is not clickable, try copying and pasting it into the address bar of your web browser.</small><br>" +
                "This will ensure you receive important updates and notifications.<br>If you didn't sign up, please ignore this email.<br><br>" +
                "Thank you for joining us!<br>Best regards,<br>Leitner Team</p>";

            await _notificationService.SendEmailAsync(user.Email, title, body);
        }


        private string DecodeBase64(string base64EncodedData)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            string decodedString = Encoding.UTF8.GetString(base64EncodedBytes);
            return decodedString;
        }
        private string EncodeToBase64(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            string base64EncodedData = Convert.ToBase64String(plainTextBytes);
            return base64EncodedData;
        }

        private async Task<UserComfirmResultDto> ConfirmResetPasswordAsync(ApplicationUser user, string identifier, string token)
        {
            UserComfirmResultDto output = new()
            {
                Status = ComfirmationStatus.SuccessEmail
            };

            UserResetPasswordViewModel checkData = new()
            {
                Identifier = identifier,
                Token = token,
            };

            //check token for password reset, if true in controller get new password and send UserResetPasswordViewModel to ResetPasswordConfirmAsync
            bool reset = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", token);

            if (reset)
            {
                output.ResetPassword = new()
                {
                    Identifier = identifier,
                    Token = token,
                };
                output.Status = ComfirmationStatus.PasswordReset;
                return output;
            }

            output.Status = ComfirmationStatus.Fail;
            return output;
        }

        private async Task<UserComfirmResultDto> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            UserComfirmResultDto output = new()
            {
                Status = ComfirmationStatus.SuccessEmail
            };

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Errors.Any())
            {
                output.Errors = result.Errors.Select(x => x.Description).ToList<string>();
                output.Status = ComfirmationStatus.Fail;
                return output;
            }

            if (!result.Succeeded)
            {
                output.Status = ComfirmationStatus.Fail;
                return output;
            }

            return output;
        }
        private async Task<IEnumerable<ApplicationUser>> AllUsersOrUsersInRole(UserType? type)
        {
            if (type is not null)
            {
                return await _userManager.GetUsersInRoleAsync(type.ToString());
            }

            return _userManager.Users;
        }
    }
}