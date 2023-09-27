using encrypt_server.Exceptions;
using encrypt_server.Models;
using encrypt_server.Repositories;

namespace encrypt_server.Services
{
    public class StateService
    {

        private readonly StateRepository repository;

        public StateService(StateRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<State>> FindAll()
        {
            return await repository.FindAll();
        }

    }
}