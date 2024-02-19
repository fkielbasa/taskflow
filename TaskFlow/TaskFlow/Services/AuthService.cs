using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.Models;
using TaskFlow.Models.Dto;
using TaskFlow.Utils;

namespace TaskFlow.Services
{
    public class AuthService: IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<PasswordResetToken> _tokenCollection;
        public AuthService(IConfiguration configuration,IOptions<DatabaseSettings> settings) { 
            _configuration = configuration;
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            _users = mongoClient.GetDatabase(settings.Value.DatabaseName)
                .GetCollection<User>(settings.Value.UsersCollectionName);
            _tokenCollection = mongoClient.GetDatabase(settings.Value.DatabaseName)
                .GetCollection<PasswordResetToken>(settings.Value.ResetTokensCollectionName);
        }
        public User Register(UserDtoRequest userDto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            if (!UserUtils.IsEmailAvailable(userDto.Email, _users))
            {
                return null;
            }
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
            return user;
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
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                 _configuration.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var payload = new JwtPayload
            { 
                { "username", user.Username },
                { "firstName", user.FirstName },
                { "lastName", user.LastName },
                { "email", user.Email } 
            };

            var token = new JwtSecurityToken(
                new JwtHeader(creds),
                payload
            );

            var jwt = tokenHandler.WriteToken(token);

            return jwt;
        }

        public LoginDtoResponse Login(LoginDto usr)
        {
            var user = _users.Find(u => u.Email == usr.Email).SingleOrDefault();
            if (user == null)
            {
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(usr.Password, user.Password);
            if (!isPasswordValid)
            {
                return null;
            }
            string token = CreateToken(user);

            DateTime expiration = DateTime.Now.AddDays(1);
            return new LoginDtoResponse
            {
                Token = token,
                Expiration = expiration
            };
        }
        public User GetUserByEmail(string email)
        {
            var user = _users.Find(u => u.Email == email).FirstOrDefault();
            return user;
        }
        public PasswordResetToken IsResetTokenValid(string resetToken)
        {
            var user = _tokenCollection.Find(t => t.Token == resetToken).FirstOrDefault();
            if (user == null)
            {
                return null; 
            }
            if (user.ExpiryDate < DateTime.Now)
            {
                return null; 
            }
            return user; 
        }
    }
}