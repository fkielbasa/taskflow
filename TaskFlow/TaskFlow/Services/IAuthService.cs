using TaskFlow.Models;
using TaskFlow.Models.Dto;

namespace TaskFlow.Services
{
    public interface IAuthService
    {
        string Register(UserDtoRequest user);
        string Login(UserDtoRequest user);
        string CreateToken(User user);
        List<UserDtoResponse> GetUsers();
    }
}