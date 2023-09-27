using encrypt_server.Exceptions;
using encrypt_server.Models;
using encrypt_server.Repositories;

namespace encrypt_server.Services
{
    public class BusinessTypeService
    {

        private readonly BusinessTypeRepository repository;

        public BusinessTypeService(BusinessTypeRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<BusinessType>> FindAll()
        {
            return await repository.FindAll();
        }

    }
}