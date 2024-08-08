using Microsoft.AspNetCore.Mvc.Rendering;
using SharedLeit;

namespace ModelsLeit.DTOs.User
{
    public record UserModifyRoleDto
    {
        public long Id { get; set; }
        public UserType Type { get; set; }

        public List<SelectListItem>? Types { get; set; }
    }
}
