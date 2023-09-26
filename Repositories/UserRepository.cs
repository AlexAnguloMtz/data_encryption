using encrypt_server.Models;
using Npgsql;
using System.Data;

namespace encrypt_server.Repositories
{
    public class UserRepository
    {

        private readonly string connectionString;

        public UserRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<AppUser?> FindByUsername(String username)
        {

            var query =
                "SELECT id, username, encrypted_password " +
                "FROM app_user " +
                "WHERE username = $1 " +
                "LIMIT 1";

            try
            {
                await using var dataSource = NpgsqlDataSource.Create(this.connectionString);
                await using var command = dataSource.CreateCommand(query);
                command.Parameters.AddWithValue(username);
                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return ReadUser(reader);
                }

            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return null;
        }

        public async Task<AppUser> Save(AppUser user)
        {
            try
            {
                var query =
                    "INSERT INTO app_user(username, encrypted_password) " +
                    "VALUES($1, $2) " +
                    "RETURNING id, username, encrypted_password";

                await using var dataSource = NpgsqlDataSource.Create(this.connectionString);

                await using var command = dataSource.CreateCommand(query);
                command.Parameters.AddWithValue(user.Username);
                command.Parameters.AddWithValue(user.EncryptedPassword);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return ReadUser(reader);
                }
                throw new Exception("Could not save user");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw; 
            }
        }

        private AppUser ReadUser(IDataReader reader)
        {
            int id = reader.GetInt32(reader.GetOrdinal("id"));
            string foundUsername = reader.GetString(reader.GetOrdinal("username"));
            string encryptedPassword = reader.GetString(reader.GetOrdinal("encrypted_password"));

            return new AppUser
            {
                Id = id,
                Username = foundUsername,
                EncryptedPassword = encryptedPassword
            };
        }
    }
   
}