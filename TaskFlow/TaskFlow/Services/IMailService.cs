namespace TaskFlow.Services
{
    public interface IMailService
    {
        Task SendResetPasswordEmail(string email);
    }
}