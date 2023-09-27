using Npgsql;

namespace encrypt_server.Repositories
{
    public class BusinessTypeRepository
    {

        private readonly string connectionString;

        public BusinessTypeRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task<List<BusinessType>> FindAll()
        {
            var query = "SELECT * FROM business_type";
            var models = new List<BusinessType>();

            try
            {
                await using var dataSource = NpgsqlDataSource.Create(this.connectionString);
                await using var command = dataSource.CreateCommand(query);
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var state = new BusinessType
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name"))
                    };
                    models.Add(state);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return models;
        }

    }

}