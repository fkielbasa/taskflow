
using MailKit.Net.Smtp;
using System.Net;
using MongoDB.Driver;
using TaskFlow.Models;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using MailKit.Security;
using MimeKit;
using Amazon.SecurityToken.Model;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskFlow.Services
{
    public class MailService : IMailService
    {
        private readonly IMongoCollection<PasswordResetToken> _tokenCollection;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public MailService(IConfiguration configuration, IOptions<DatabaseSettings> settings,IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _tokenCollection = database.GetCollection<PasswordResetToken>(settings.Value.ResetTokensCollectionName);
        }
        
        public Task SendResetPasswordEmail(string userEmail)
        {
            string resetToken = Guid.NewGuid().ToString();
            string userId = _authService.GetUserByEmail(userEmail).Id;

            _tokenCollection.InsertOneAsync(new PasswordResetToken
            {
                UserId = userId,
                Token = resetToken,
                ExpiryDate = DateTime.Now.AddMinutes(5)
            });

            string smtpHost = _configuration.GetSection("EmailSettings:Host").Value;
            int smtpPort = Convert.ToInt32(_configuration.GetSection("EmailSettings:Port").Value);
            string smtpSender = _configuration.GetSection("EmailSettings:Sender").Value;
            string smtpPassword = _configuration.GetSection("EmailSettings:Password").Value;

            string resetLink = $"http://localhost:5173/reset-password/{resetToken}";
            string subject = "Password reset";
            string body = $"To reset your password, please click on the link: <a href=\"{resetLink}\">link</a>";

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(smtpSender));
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body; 
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(smtpSender, smtpPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
            return Task.CompletedTask;
        }
       
    }
}
