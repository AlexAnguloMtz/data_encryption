using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using encrypt_server.Models;
using encrypt_server.Services;

namespace encrypt_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService service;

        public AuthController(AuthService authService)
        {
            service = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost]
        public String Create(RegistrationRequest request)
        {
            return service.RegisterUser(request);
        }
    }

}