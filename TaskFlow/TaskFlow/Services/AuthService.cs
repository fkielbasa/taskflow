using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.Models;
using TaskFlow.Models.Dto;

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
        public string Register(UserDtoRequest userDto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            var user = new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Username = userDto.Username,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Password = passwordHash
            };
            _users.InsertOne(user);
            return CreateToken(user);
        }
        public List<UserDtoResponse> GetUsers()
        {
            var users = _users.Find(_ => true).ToList();
            var usersDto = users.Select(user =>
                new UserDtoResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                }).ToList();
            return usersDto;
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

        public string Login(UserDtoRequest userDto)
        {
            var user = _users.Find(u => u.Email == userDto.Email).SingleOrDefault();
            if (user == null)
            {
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password);
            if (!isPasswordValid)
            {
                return null;
            }
            return CreateToken(user);
        }

    }
}