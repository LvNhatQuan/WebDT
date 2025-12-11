using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class MenuDAL
    {
        private readonly DbConnect _db = new DbConnect();

        public List<NavbarItem> GetNavbarMenu()
        {
            List<NavbarItem> list = new();

            string sql = @"SELECT id, title, parent_id, menu_url, menu_index, isVisible 
                           FROM menu
                           WHERE isVisible = 1
                           ORDER BY menu_index ASC";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new NavbarItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    ParentId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    MenuUrl = reader.IsDBNull(3) ? "#" : reader.GetString(3),
                    MenuIndex = reader.GetInt32(4),
                    IsVisible = reader.GetBoolean(5)
                });
            }

            _db.CloseConnection();

            // Build cây menu
            var root = list.Where(x => x.ParentId == null).ToList();
            foreach (var parent in root)
            {
                parent.SubItems = list.Where(x => x.ParentId == parent.Id).ToList();
            }

            return root;
        }
    }
}
