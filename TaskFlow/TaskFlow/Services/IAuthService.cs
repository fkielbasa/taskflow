using TaskFlow.Models;
using TaskFlow.Models.Dto;

namespace TaskFlow.Services
{
    public interface IAuthService
    {
        User Register(UserDtoRequest user);
        LoginDtoResponse Login(LoginDto user);
        string CreateToken(User user);
        List<UserDtoResponse> GetUsers();
        User GetUserByEmail(string email);
        PasswordResetToken IsResetTokenValid(string resetToken);
    }
}