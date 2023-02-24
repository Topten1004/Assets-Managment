using Backend.Data.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Utils
{
    public class Helper : ControllerBase    
    {
        public UserEntity GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                Role role;
                Enum.TryParse<Role>(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value, out role);

                return new UserEntity
                {
                    UserEmail = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    Role = role
                };
            }

            return null;

        }
    }
}
