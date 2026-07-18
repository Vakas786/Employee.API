using Employee.API.DTOs;
using Employee.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employee.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        // POST: api/Auth/Register

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authRepository.RegisterAsync(dto);

            if (string.IsNullOrEmpty(result))
            {
                return BadRequest(new
                {
                    Message = "User already exists."
                });
            }

            return Ok(new
            {
                Message = "User Registered Successfully."
            });
        }

        // POST: api/Auth/Login

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authRepository.LoginAsync(dto);

            if (response == null)
            {
                return Unauthorized(new
                {
                    Message = "Invalid Email or Password."
                });
            }

            return Ok(response);
        }
    }
}