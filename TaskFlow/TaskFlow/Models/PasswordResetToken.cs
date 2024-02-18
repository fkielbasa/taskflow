namespace TaskFlow.Models
{
    public class PasswordResetToken
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
