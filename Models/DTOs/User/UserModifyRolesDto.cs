using Microsoft.AspNetCore.Mvc.Rendering;
using SharedLeit;

namespace ModelsLeit.DTOs.User
{
    public record UserModifyRolesDto
    {
        public long Id { get; set; }
        public List<UserType> Type { get; set; }

        public List<SelectListItem>? Types { get; set; }
    }
}
