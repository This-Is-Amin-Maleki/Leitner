using ModelsLeit.ViewModels.User;
using SharedLeit;

namespace ModelsLeit.DTOs.User
{
    public record UserComfirmResultDto
    {
        public ComfirmationStatus Status { get; set; }
        public List<string>? Errors { get; set; }
        public UserResetPasswordViewModel? ResetPassword { get; set; }
    }
}
