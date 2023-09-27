using Microsoft.AspNetCore.Mvc;

using encrypt_server.Models;
using encrypt_server.Services;

namespace encrypt_server.Controllers
{
    [Route("api/registration-options")]
    [ApiController]
    public class RegistrationOptionsController : ControllerBase
    {

        private readonly BusinessTypeService businessTypeService;
        private readonly StateService stateService;


        public RegistrationOptionsController
        (
            BusinessTypeService businessTypeService,
            StateService stateService
        )
        {
            this.businessTypeService = businessTypeService ?? throw new ArgumentNullException(nameof(businessTypeService));
            this.stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));

        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(
                new RegistrationOptions
                {
                    States = await stateService.FindAll(),
                    BusinessTypes = await businessTypeService.FindAll(),
                }
            );
        }
    }

}