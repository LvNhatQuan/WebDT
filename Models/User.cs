namespace WebDT.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;

        public string Role { get; set; } = "customer";

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsLocked { get; set; } = false;
    }
}
