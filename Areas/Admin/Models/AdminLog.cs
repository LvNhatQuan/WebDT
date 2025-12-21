namespace WebDT.Areas.Admin.Models
{
    public class AdminLog
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Role { get; set; } = "";
        public string Action { get; set; } = "";
        public string Module { get; set; } = "";
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
