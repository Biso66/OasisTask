using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oasis.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Oasis.Services
{
    public class GetUser : IGetUser
    {
        private readonly WorkDbContext _workContext;
        public GetUser(WorkDbContext workDbContext)
        {
            _workContext = workDbContext;
        }
        public async Task<int> GetUserByToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var tokenS = jwtSecurityToken as JwtSecurityToken;
            var User = await _workContext.Users.FirstOrDefaultAsync(ww => ww.Email == tokenS.Payload["email"].ToString());
            var UserId = User.Id;
            return UserId;
        }
    }
}
