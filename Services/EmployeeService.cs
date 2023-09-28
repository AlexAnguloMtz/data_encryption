using encrypt_server.Models;
using encrypt_server.Repositories;

namespace encrypt_server.Services
{
    public class EmployeeService
    {

        private readonly EmployeeRepository repository;

        public EmployeeService(EmployeeRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<Employee>> GetAll()
        {
            return await repository.FindAll();
        }

        public async Task Save(SaveEmployeeRequest request)
        {
            await repository.Save(MapToEntity(request));
            await Task.CompletedTask;
        }

        public async Task<Employee> GetById(int id)
        {
            Employee? employee = await repository.FindById(id);
            return employee ?? throw new Exception($"Could not find employee with id = '{id}'");
        }

        public async Task Update(int id, SaveEmployeeRequest request)
        {
            Employee employee = MapToEntity(id, request);
            await repository.Save(employee);
            await Task.CompletedTask;
        }

        private Employee MapToEntity(SaveEmployeeRequest request)
        {
            return new Employee
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                MonthlySalaryUSD = request.MonthlySalaryUSD,
                Address = request.Address,
            };
        }

        private Employee MapToEntity(int id, SaveEmployeeRequest request)
        {
            return new Employee
            {
                Id = id,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                MonthlySalaryUSD = request.MonthlySalaryUSD,
                Address = request.Address,
            };
        }

       
    }
}