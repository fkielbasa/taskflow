using TaskFlow.Models;

namespace TaskFlow.Services
{
    public interface IAuthService
    {
        string Register(User user);
        User Login();
        string CreateToken(User user);
        List<User> GetUsers();
    }
}
