using encrypt_server.Models;

namespace encrypt_server.Services
{
    public class AuthService
    {
        public String RegisterUser(RegistrationRequest request)
        {
            return request.Username;
        }
    }
}
