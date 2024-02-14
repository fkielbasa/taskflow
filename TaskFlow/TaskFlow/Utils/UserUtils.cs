using MongoDB.Driver;
using TaskFlow.Models;

namespace TaskFlow.Utils
{
    public static class UserUtils
    {
        public static bool IsEmailAvailable(string email, IMongoCollection<User> users) => !users.Find(u => u.Email == email).Any();
    }
}
