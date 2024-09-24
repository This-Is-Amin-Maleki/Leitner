using Microsoft.AspNetCore.Identity;
using ModelsLeit.DTOs.Box;
using ModelsLeit.DTOs.Card;
using ModelsLeit.DTOs.Container;
using ModelsLeit.Entities;

namespace ServicesLeit.Interfaces
{
    public interface ITokenService
    {
        string CreateJWTToken(ApplicationUser user, List<string> roles);
    }
}