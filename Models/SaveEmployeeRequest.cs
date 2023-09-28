namespace encrypt_server.Models
{
    public class SaveEmployeeRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int MonthlySalaryUSD { get; set; }
        public Address Address { get; set; }
    }
}
