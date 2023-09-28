using encrypt_server.Models;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int MonthlySalaryUSD { get; set; }
    public Address Address { get; set; }
}