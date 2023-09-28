using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using encrypt_server.Models;
using System.Data;

namespace encrypt_server.Repositories
{
    public class EmployeeRepository
    {
        private readonly string connectionString;

        public EmployeeRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<List<Employee>> FindAll()
        {
            List<Employee> employees = new();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using var command = new NpgsqlCommand("SELECT * FROM employee INNER JOIN address ON employee.address_id = address.address_id", connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    employees.Add(ReadEmployee(reader));
                }
            }

            return employees;
        }

        public async Task Save(Employee employee)
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                if (employee.Id == null)
                {
                    using var addressCommand = new NpgsqlCommand();
                    addressCommand.Connection = connection;
                    addressCommand.Transaction = transaction;
                    addressCommand.CommandType = CommandType.Text;
                    addressCommand.CommandText = "INSERT INTO address (city, street_name, street_number) VALUES (@City, @StreetName, @StreetNumber) RETURNING address_id";

                    addressCommand.Parameters.AddWithValue("@City", employee.Address.City);
                    addressCommand.Parameters.AddWithValue("@StreetName", employee.Address.StreetName);
                    addressCommand.Parameters.AddWithValue("@StreetNumber", employee.Address.StreetNumber);

                    var addressId = await addressCommand.ExecuteScalarAsync();

                    using var employeeCommand = new NpgsqlCommand();
                    employeeCommand.Connection = connection;
                    employeeCommand.Transaction = transaction;
                    employeeCommand.CommandType = CommandType.Text;
                    employeeCommand.CommandText = "INSERT INTO employee (full_name, email, phone, address_id, monthly_salary_usd) VALUES (@FullName, @Email, @Phone, @AddressId, @MonthlySalaryUSD)";

                    employeeCommand.Parameters.AddWithValue("@FullName", employee.FullName);
                    employeeCommand.Parameters.AddWithValue("@Email", employee.Email);
                    employeeCommand.Parameters.AddWithValue("@Phone", employee.Phone);
                    employeeCommand.Parameters.AddWithValue("@AddressId", addressId);
                    employeeCommand.Parameters.AddWithValue("@MonthlySalaryUSD", employee.MonthlySalaryUSD);

                    await employeeCommand.ExecuteNonQueryAsync();
                }
                else
                {
                    using (var addressCommand = new NpgsqlCommand())
                    {
                        addressCommand.Connection = connection;
                        addressCommand.Transaction = transaction;
                        addressCommand.CommandType = CommandType.Text;
                        addressCommand.CommandText = "UPDATE address SET city = @City, street_name = @StreetName, street_number = @StreetNumber WHERE address_id = (SELECT address_id FROM employee WHERE id = @Id)";

                        addressCommand.Parameters.AddWithValue("@Id", employee.Id);
                        addressCommand.Parameters.AddWithValue("@City", employee.Address.City);
                        addressCommand.Parameters.AddWithValue("@StreetName", employee.Address.StreetName);
                        addressCommand.Parameters.AddWithValue("@StreetNumber", employee.Address.StreetNumber);

                        await addressCommand.ExecuteNonQueryAsync();
                    }

                    using var employeeCommand = new NpgsqlCommand();
                    employeeCommand.Connection = connection;
                    employeeCommand.Transaction = transaction;
                    employeeCommand.CommandType = CommandType.Text;
                    employeeCommand.CommandText = "UPDATE employee SET full_name = @FullName, email = @Email, phone = @Phone, monthly_salary_usd = @MonthlySalaryUSD WHERE id = @Id";

                    employeeCommand.Parameters.AddWithValue("@Id", employee.Id);
                    employeeCommand.Parameters.AddWithValue("@FullName", employee.FullName);
                    employeeCommand.Parameters.AddWithValue("@Email", employee.Email);
                    employeeCommand.Parameters.AddWithValue("@Phone", employee.Phone);
                    employeeCommand.Parameters.AddWithValue("@MonthlySalaryUSD", employee.MonthlySalaryUSD);

                    await employeeCommand.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<Employee?> FindById(int id)
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM employee INNER JOIN address ON employee.address_id = address.address_id WHERE employee.id = @Id";

            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadEmployee(reader);
            }

            return null;
        }

        private Employee ReadEmployee(NpgsqlDataReader reader)
        {
            return new Employee
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                FullName = reader.GetString(reader.GetOrdinal("full_name")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Phone = reader.GetString(reader.GetOrdinal("phone")),
                MonthlySalaryUSD = reader.GetInt32(reader.GetOrdinal("monthly_salary_usd")),
                Address = ReadAddress(reader),
            };
        }

        private Address ReadAddress(NpgsqlDataReader reader)
        {
            return new Address
            {
                City = reader.GetString(reader.GetOrdinal("city")),
                StreetName = reader.GetString(reader.GetOrdinal("street_name")),
                StreetNumber = reader.GetString(reader.GetOrdinal("street_number")),
            };
        }
    }
}