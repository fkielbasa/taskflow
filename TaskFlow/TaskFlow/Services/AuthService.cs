using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class AuthService: IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User> _users;
        public AuthService(IConfiguration configuration,IOptions<DatabaseSettings> settings) { 
            _configuration = configuration;
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            _users = mongoClient.GetDatabase(settings.Value.DatabaseName)
                .GetCollection<User>(settings.Value.UsersCollectionName);
        }
        public string Register(User user)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = passwordHash;
            user.Id = ObjectId.GenerateNewId().ToString();
            _users.InsertOne(user);
            return CreateToken(user);
        }
        public List<User> GetUsers()
        {
            var users = _users.Find(_ => true).ToList();
            return users;
        }
        public string CreateToken(User user)
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
