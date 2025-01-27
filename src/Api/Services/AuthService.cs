using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Models;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;

namespace Services
{
    public class AuthService
    {

        public string GenerateToken(User user)
        {
            Env.Load();

            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            if (apiKey == null)
            {
                throw new ArgumentNullException("API_KEY", "A chave não está definida");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
