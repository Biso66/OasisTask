using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Oasis.DataOfToken;
using Oasis.Helpers;
using Oasis.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TryJWT.Models;

namespace Oasis.Services
{
    public class AuthService :IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JWT _jwt;
        private readonly WorkDbContext _workContext;
        public AuthService(UserManager<IdentityUser> userManager, IOptions<JWT> jwt, WorkDbContext workContext)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _workContext = workContext;
        }

        public async Task<AuthModel> GetToken(TokenRequestModel model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is Incorrect";
                return authModel;
            }
            var roleList = await _userManager.GetRolesAsync(user);
            var jwtSecurityToken = await CreateJwtToken(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = model.Email;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = roleList.ToList();

            return authModel;
        }

        public async Task<AuthModel> Registeration(RegisterModel model)
        {
            if(await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return new AuthModel { Email = model.Email, Message = "Email is alyeady Exist" };
            }
            var user = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email,
            };
            var resulte = await _userManager.CreateAsync(user, model.Password);
            if (!resulte.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in resulte.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, Roles.User);
            var NewUser = new User
            {
                Name=model.Username,
                Email = model.Email,
            };
            _workContext.Users.Add(NewUser);
            _workContext.SaveChanges();
            var jwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = model.Email,
                Username = model.Username,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { Roles.User },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            };
        }

        private async Task<JwtSecurityToken> CreateJwtToken(IdentityUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
