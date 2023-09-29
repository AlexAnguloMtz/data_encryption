using Npgsql;
using encrypt_server.Models;
using System.Data;

namespace encrypt_server.Repositories
{
    public class EmployeeRepository
    {
        private readonly string connectionString;
        private readonly string encryptionSecret;


        public EmployeeRepository(string connectionString, string encryptionSecret)
        {
            this.connectionString = connectionString;
            this.encryptionSecret = encryptionSecret;
        }

        public async Task<List<Employee>> FindAll()
        {
            List<Employee> employees = new();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using var command = new NpgsqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText = @"
                    SELECT 
                        id,
                        pgp_sym_decrypt(full_name::bytea, @Secret) AS full_name,
                        pgp_sym_decrypt(email::bytea, @Secret) AS email,
                        pgp_sym_decrypt(phone::bytea, @Secret) AS phone,
                        monthly_salary_usd,
                        pgp_sym_decrypt(city::bytea, @Secret) AS city,
                        pgp_sym_decrypt(street_name::bytea, @Secret) AS street_name,
                        pgp_sym_decrypt(street_number::bytea, @Secret) AS street_number
                    FROM employee
                    INNER JOIN address ON employee.address_id = address.address_id
                ";

                command.Parameters.AddWithValue("@Secret", encryptionSecret);

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
                    addressCommand.CommandText = @"
                    INSERT INTO address (city, street_name, street_number)
                    VALUES (
                        pgp_sym_encrypt(@City, @Secret),
                        pgp_sym_encrypt(@StreetName, @Secret),
                        pgp_sym_encrypt(@StreetNumber, @Secret)
                    )
                    RETURNING address_id";

                    addressCommand.Parameters.AddWithValue("@City", employee.Address.City);
                    addressCommand.Parameters.AddWithValue("@StreetName", employee.Address.StreetName);
                    addressCommand.Parameters.AddWithValue("@StreetNumber", employee.Address.StreetNumber);
                    addressCommand.Parameters.AddWithValue("@Secret", encryptionSecret);

                    var addressId = await addressCommand.ExecuteScalarAsync();

                    using var employeeCommand = new NpgsqlCommand();
                    employeeCommand.Connection = connection;
                    employeeCommand.Transaction = transaction;
                    employeeCommand.CommandType = CommandType.Text;
                    employeeCommand.CommandText = @"
                    INSERT INTO employee (full_name, email, phone, address_id, monthly_salary_usd) 
                    VALUES (
                        pgp_sym_encrypt(@FullName, @Secret),
                        pgp_sym_encrypt(@Email, @Secret), 
                        pgp_sym_encrypt(@Phone, @Secret),
                        @AddressId, 
                        @MonthlySalaryUSD
                    )";

                    employeeCommand.Parameters.AddWithValue("@FullName", employee.FullName);
                    employeeCommand.Parameters.AddWithValue("@Email", employee.Email);
                    employeeCommand.Parameters.AddWithValue("@Phone", employee.Phone);
                    employeeCommand.Parameters.AddWithValue("@AddressId", addressId);
                    employeeCommand.Parameters.AddWithValue("@MonthlySalaryUSD", employee.MonthlySalaryUSD);
                    employeeCommand.Parameters.AddWithValue("@Secret", encryptionSecret);

                    await employeeCommand.ExecuteNonQueryAsync();
                }
                else
                {
                    using (var addressCommand = new NpgsqlCommand())
                    {
                        addressCommand.Connection = connection;
                        addressCommand.Transaction = transaction;
                        addressCommand.CommandType = CommandType.Text;
                        addressCommand.CommandText = @"
                        UPDATE address 
                        SET 
                            city = pgp_sym_encrypt(@City, @Secret), 
                            street_name = pgp_sym_encrypt(@StreetName, @Secret), 
                            street_number = pgp_sym_encrypt(@StreetNumber, @Secret)
                            WHERE address_id = (SELECT address_id FROM employee WHERE id = @Id)";

                        addressCommand.Parameters.AddWithValue("@Id", employee.Id);
                        addressCommand.Parameters.AddWithValue("@City", employee.Address.City);
                        addressCommand.Parameters.AddWithValue("@StreetName", employee.Address.StreetName);
                        addressCommand.Parameters.AddWithValue("@StreetNumber", employee.Address.StreetNumber);
                        addressCommand.Parameters.AddWithValue("@Secret", encryptionSecret);


                        await addressCommand.ExecuteNonQueryAsync();
                    }

                    using var employeeCommand = new NpgsqlCommand();
                    employeeCommand.Connection = connection;
                    employeeCommand.Transaction = transaction;
                    employeeCommand.CommandType = CommandType.Text;
                    employeeCommand.CommandText = @"
                    UPDATE employee
                    SET
                        full_name = pgp_sym_encrypt(@FullName, @Secret),
                        email = pgp_sym_encrypt(@Email, @Secret),
                        phone = pgp_sym_encrypt(@Phone, @Secret),
                        monthly_salary_usd = @MonthlySalaryUSD
                    WHERE id = @Id";

                    employeeCommand.Parameters.AddWithValue("@Id", employee.Id);
                    employeeCommand.Parameters.AddWithValue("@FullName", employee.FullName);
                    employeeCommand.Parameters.AddWithValue("@Email", employee.Email);
                    employeeCommand.Parameters.AddWithValue("@Phone", employee.Phone);
                    employeeCommand.Parameters.AddWithValue("@MonthlySalaryUSD", employee.MonthlySalaryUSD);
                    employeeCommand.Parameters.AddWithValue("@Secret", encryptionSecret);

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
            command.CommandText = @"
                SELECT 
                    id,
                    pgp_sym_decrypt(full_name::bytea, @Secret) AS full_name,
                    pgp_sym_decrypt(email::bytea, @Secret) AS email,
                    pgp_sym_decrypt(phone::bytea, @Secret) AS phone,
                    monthly_salary_usd,
                    pgp_sym_decrypt(city::bytea, @Secret) AS city,
                    pgp_sym_decrypt(street_name::bytea, @Secret) AS street_name,
                    pgp_sym_decrypt(street_number::bytea, @Secret) AS street_number
                FROM employee
                INNER JOIN address ON employee.address_id = address.address_id WHERE employee.id = @Id
            ";

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Secret", encryptionSecret);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadEmployee(reader);
            }

            return null;
        }

        public async Task DeleteById(int id)
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using var deleteEmployeeCommand = new NpgsqlCommand();
                deleteEmployeeCommand.Connection = connection;
                deleteEmployeeCommand.Transaction = transaction;
                deleteEmployeeCommand.CommandType = CommandType.Text;
                deleteEmployeeCommand.CommandText = "DELETE FROM employee WHERE id = @Id RETURNING address_id";

                deleteEmployeeCommand.Parameters.AddWithValue("@Id", id);

                var addressId = await GetAddressIdAsync(deleteEmployeeCommand);

                if (addressId.HasValue)
                {
                    using var deleteAddressCommand = new NpgsqlCommand();
                    deleteAddressCommand.Connection = connection;
                    deleteAddressCommand.Transaction = transaction;
                    deleteAddressCommand.CommandType = CommandType.Text;
                    deleteAddressCommand.CommandText = "DELETE FROM address WHERE address_id = @AddressId";

                    deleteAddressCommand.Parameters.AddWithValue("@AddressId", addressId);

                    await deleteAddressCommand.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task<int?> GetAddressIdAsync(NpgsqlCommand command)
        {
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32(reader.GetOrdinal("address_id"));
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