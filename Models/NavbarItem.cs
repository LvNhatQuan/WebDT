namespace WebDT.Models
{
    public class NavbarItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? ParentId { get; set; }
        public string MenuUrl { get; set; }
        public int MenuIndex { get; set; }
        public bool IsVisible { get; set; }

        // Danh sách menu con
        public List<NavbarItem> SubItems { get; set; } = new();
    }
}
