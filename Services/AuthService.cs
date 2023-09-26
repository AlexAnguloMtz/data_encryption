using encrypt_server.Exceptions;
using encrypt_server.Models;
using encrypt_server.Repositories;

namespace encrypt_server.Services
{
    public class AuthService
    {

        private readonly UserRepository repository;

        public AuthService(UserRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository)); 
        }

        public async Task<AppUserResponse> RegisterUser(RegistrationRequest request)
        {
            AppUser? user = await repository.FindByUsername(request.Username);
            if (user != null)
            {
                throw new ConflictException();
            }
            AppUser savedUser = await repository.Save(MapToUser(request));
            return MapToResponse(savedUser);
        }

        public async Task<string> LoginUser(LoginRequest request)
        {
            AppUser? user = await repository.FindByUsername(request.Username);
            if (user == null)
            {
                throw new UnauthorizedException();
            }
        }

        private AppUser MapToUser(RegistrationRequest request)
        {
            // The password is encrypted at the database level,
            // with a database specific encryption implementation
            return new AppUser
            {
                Username = request.Username,
                EncryptedPassword = request.Password
            };
        }

        private AppUserResponse MapToResponse(AppUser user)
        {
            return new AppUserResponse
            {
                Id = user.Id,
                Username = user.Username
            };
        }

    }
}