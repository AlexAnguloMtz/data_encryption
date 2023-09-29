using encrypt_server.Models;
using encrypt_server.Services;
using Microsoft.AspNetCore.Mvc;

namespace encrypt_server.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly EmployeeService service;

        public EmployeeController(EmployeeService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await service.GetAll());
            }
            catch (Exception e)
            {
                return StatusCode(500, e.GetType().FullName + " " + e.Message);
            }
        }

        [HttpGet("{id}")] 
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                return Ok(await service.GetById(id));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.GetType().FullName + " " + e.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(SaveEmployeeRequest request)
        {
            try
            {
                await service.Save(request);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.GetType().FullName + " " + e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]SaveEmployeeRequest request)
        {
            try
            {
                await service.Update(id, request);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.GetType().FullName + " " + e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await service.DeleteById(id);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.GetType().FullName + " " + e.Message);
            }
        }

    }

}