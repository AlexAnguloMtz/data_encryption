using Npgsql;

namespace encrypt_server.Repositories
{
    public class StateRepository
    {

        private readonly string connectionString;

        public StateRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<List<State>> FindAll()
        {
            var query = "SELECT * FROM state";
            var models = new List<State>();

            try
            {
                await using var dataSource = NpgsqlDataSource.Create(this.connectionString);
                await using var command = dataSource.CreateCommand(query);
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var state = new State
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