using Microsoft.AspNetCore.Identity;
using SharedLeit;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ModelsLeit.Entities
{
    public class UserRole : IdentityRole<long>
    {
        public UserType UserType { get; set; }

        public UserRole() { }

        public UserRole(string roleName, UserType userType) : base(roleName)
        {
            UserType = userType;
        }
    }
}
