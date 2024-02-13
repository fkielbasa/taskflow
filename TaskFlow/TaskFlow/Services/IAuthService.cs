using TaskFlow.Models;
using TaskFlow.Models.Dto;

namespace TaskFlow.Services
{
    public interface IAuthService
    {
        string Register(UserDtoRequest user);
        User Login();
        string CreateToken(User user);
        List<UserDtoResponse> GetUsers();
    }
}
