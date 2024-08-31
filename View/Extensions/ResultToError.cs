using Microsoft.EntityFrameworkCore;
using SharedLeit;
using System.Text;

namespace ViewLeit.Extensions
{
    public static class ResultToError
    {
        public static (string key, string message) LoginResultError(this LoginResult result, DateTimeOffset? lockoutOffset = null )
        {
            string lockoutTimeString = string.Empty;
            if (lockoutOffset is not null)
            {
                TimeSpan? lockoutTimer = lockoutOffset - DateTime.Now;
                List<string> lockoutTime = new();
                lockoutTime.Add(
                    lockoutTimer.Value.Days is not 0 ?
                    lockoutTimer.Value.Days is 1 ?
                    $"{lockoutTimer.Value.Days} day" :
                    $"{lockoutTimer.Value.Days} days" :
                    string.Empty);
                lockoutTime.Add(
                    lockoutTimer.Value.Hours is not 0 ?
                    lockoutTimer.Value.Hours is 1 ?
                    $"{lockoutTimer.Value.Hours} hour" :
                    $"{lockoutTimer.Value.Hours} hours" :
                    string.Empty);
                lockoutTime.Add(
                    lockoutTimer.Value.Minutes is not 0 ?
                    lockoutTimer.Value.Minutes is 1 ?
                    $"{lockoutTimer.Value.Minutes} minute" :
                    $"{lockoutTimer.Value.Minutes} minutes" :
                    string.Empty);

                lockoutTimeString = lockoutTime.Count is 0 ? string.Empty : $" for {string.Join(" and ", lockoutTime.Where(value => !string.IsNullOrEmpty(value)))}";
            }

            return result switch
            {
                LoginResult.Deactive => ("Disabled", "Your account is blocked."),// not activated by admin."),
                LoginResult.EmailNotConfirmed => ("NotConfirmed", "Your email not confirmed!"),
                LoginResult.LockedOut => ("LockOut", $"User account is locked out{lockoutTimeString}."),
                LoginResult.NotFound => ("Invalid", "Invalid login attempt!"),
                LoginResult.TwoFactorInvalid => ("TwoFactor", "Invalid two-factor authentication code!"),
                _ => ("Fail", "Login failed. Please try again later!"),
            };
        }

        public static (string key, string message) RegisterResultError(this RegisterResult result) =>
            result switch
            {
                RegisterResult.EmailOrPhoneInUse => ("EmailOrPhoneInUse", "The email address is already in use!"),
                //RegisterResult.EmailOrPhoneInUse => ("EmailInUse", "The email address or The phone number is already in use!"),
                //RegisterResult.PhoneInUse => ("PhoneInUse", "The phone number is already in use!"),

                //RegisterResult.Fail
                _ => ("Fail", "Registration failed. Please try again later!"),
            };
    }
}
