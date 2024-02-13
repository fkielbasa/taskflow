using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class AuthService: IAuthService
    {
        private IConfiguration _configuration;
        public AuthService(IConfiguration configuration) { 
            _configuration = configuration;
        }
        public String Register(User user)
        {
            string passwordHash
               = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Username = user.Username;
            user.FirstName = user.FirstName;
            user.LastName = user.LastName;
            user.Email = user.Email;
            user.Password = passwordHash;
            return CreateToken(user);
        }
        public String CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name,user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    claims: claims,
                  
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


        public User Login()
        {
            throw new NotImplementedException();
        }

      
    }
}
