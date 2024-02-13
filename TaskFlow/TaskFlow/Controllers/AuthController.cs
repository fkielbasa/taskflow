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
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public ActionResult<string> Register(UserDtoRequest request)
        {
            //string token = _authService.Register(request);
            return Ok(_authService.Register(request));
        }
        [HttpGet]
        public ActionResult<User> Get()
        {
            return Ok(_authService.GetUsers());
        }

    }
}
