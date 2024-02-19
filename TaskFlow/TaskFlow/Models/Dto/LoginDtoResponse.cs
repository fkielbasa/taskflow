namespace TaskFlow.Models.Dto
{
    public class LoginDtoResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}