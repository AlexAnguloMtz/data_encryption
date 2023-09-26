namespace encrypt_server.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string EncryptedPassword { get; set; }
    }
}
