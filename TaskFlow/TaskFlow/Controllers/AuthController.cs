using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Models;
using TaskFlow.Models.Dto;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //public static User user = new User();
        private readonly IAuthService _authService;
        private readonly IMailService _mailService;
        public AuthController(IAuthService authService, IMailService mailService)
        {
            _authService = authService;
           _mailService = mailService;
        }

        [HttpPost("register")]
        public ActionResult<string> Register(UserDtoRequest request)
        {
            if(_authService.Register(request) != null)
            {
                return Ok();
            }
            return BadRequest(new { message = "Invalid data" }); 
        }

        [HttpGet("users")]
        public ActionResult<User> Get()
        {
            return Ok(_authService.GetUsers());
        }

        [HttpPost("login")]
        public ActionResult<LoginDtoResponse> Login(LoginDto user)
        {
            var loginResult = _authService.Login(user);
            if (loginResult != null)
            {
                return Ok(loginResult);
            }
            return BadRequest(new { message = "Invalid email or password" });
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(string email)
        {
            var user = _authService.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound("User with this email doesn't exist.");
            }

            _mailService.SendResetPasswordEmail(email);

            return Ok("Email with a password reset link has been sent to your email address.");
        }
        [HttpPost("check-reset-token")]
        public async Task<IActionResult> CheckResetToken(string token)
        {
            var user = _authService.IsResetTokenValid(token);
            if (user != null)
            {
                return Ok();
            }
            return BadRequest();
        }

    }
}
