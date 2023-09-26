using Microsoft.AspNetCore.Mvc;

using encrypt_server.Models;
using encrypt_server.Services;
using encrypt_server.Exceptions;

namespace encrypt_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService service;

        public AuthController(AuthService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Create(RegistrationRequest request)
        {
            try
            {
                return Ok(await service.RegisterUser(request));
            }
            catch (ConflictException)
            {
                return Conflict(
                    new {
                        error="El nombre de usuario ya existe"
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(500, "Error inesperado");
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                return Ok(
                    new 
                    {
                        token=service.LoginUser(request)
                    }    
                );
            }
            catch (UnauthorizedException)
            {
                return Unauthorized(
                    new
                    {
                        error = "Credenciales inválidas"
                    }
                );
            }
            catch (Exception)
            {
                return StatusCode(500, "Error inesperado");
            }
        }
    }

}