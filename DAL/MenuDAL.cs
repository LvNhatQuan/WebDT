using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class MenuDAL
    {
        private readonly DbConnect db = new DbConnect();

        // ============================
        // MAP MENU ITEM TỪ SQL
        // ============================
        private MenuItem Map(SqlDataReader r)
        {
            return new MenuItem
            {
                Id = Convert.ToInt32(r["id"]),
                Title = r["title"].ToString(),
                ParentId = r["parent_id"] != DBNull.Value ? Convert.ToInt32(r["parent_id"]) : (int?)null,
                MenuUrl = r["menu_url"].ToString(),
                MenuIndex = Convert.ToInt32(r["menu_index"]),
                isVisible = Convert.ToBoolean(r["isVisible"])
            };
        }

        // ============================
        // 1) Lấy tất cả menu (không phân cấp)
        // ============================
        public List<MenuItem> GetAll()
        {
            List<MenuItem> list = new();
            db.openConnection();

            using var cmd = new SqlCommand("SELECT * FROM menu ORDER BY menu_index ASC", db.getConnecttion());
            using var r = cmd.ExecuteReader();

            while (r.Read())
                list.Add(Map(r));

            db.closeConnection();
            return list;
        }

        // ============================
        // 2) Lấy menu cha
        // ============================
        public List<MenuItem> GetRootMenus()
        {
            List<MenuItem> menus = new();
            db.openConnection();

            using var cmd = new SqlCommand(@"
                SELECT * 
                FROM menu 
                WHERE parent_id IS NULL AND isVisible = 1
                ORDER BY menu_index ASC",
                db.getConnecttion());

            using var r = cmd.ExecuteReader();
            while (r.Read())
                menus.Add(Map(r));

            db.closeConnection();
            return menus;
        }

        // ============================
        // 3) Lấy menu con theo parent_id
        // ============================
        public List<MenuItem> GetChildren(int parentId)
        {
            List<MenuItem> menus = new();
            db.openConnection();

            using var cmd = new SqlCommand(@"
                SELECT * 
                FROM menu 
                WHERE parent_id = @pid AND isVisible = 1
                ORDER BY menu_index ASC",
                db.getConnecttion());

            cmd.Parameters.AddWithValue("@pid", parentId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
                menus.Add(Map(r));

            db.closeConnection();
            return menus;
        }

        // ============================
        // 4) Tạo menu tree cho navbar (đa cấp)
        // ============================
        public List<NavbarItem> GetNavbarMenu()
        {
            List<NavbarItem> result = new();
            db.openConnection();

            // Lấy menu cha
            using var cmd = new SqlCommand(@"
                SELECT * 
                FROM menu 
                WHERE parent_id IS NULL AND isVisible = 1
                ORDER BY menu_index ASC",
                db.getConnecttion());

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new NavbarItem
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Title = reader["title"].ToString(),
                    MenuUrl = reader["menu_url"].ToString(),
                    MenuIndex = Convert.ToInt32(reader["menu_index"]),
                    isVisible = Convert.ToBoolean(reader["isVisible"])
                });
            }

            reader.Close();

            // Lấy menu con cho từng menu cha
            foreach (var parent in result)
            {
                using var childCmd = new SqlCommand(@"
                    SELECT * 
                    FROM menu 
                    WHERE parent_id = @pid AND isVisible = 1
                    ORDER BY menu_index ASC",
                    db.getConnecttion());

                childCmd.Parameters.AddWithValue("@pid", parent.Id);

                using var cr = childCmd.ExecuteReader();
                List<NavbarItem> children = new();

                while (cr.Read())
                {
                    children.Add(new NavbarItem
                    {
                        Id = Convert.ToInt32(cr["id"]),
                        Title = cr["title"].ToString(),
                        MenuUrl = cr["menu_url"].ToString(),
                        MenuIndex = Convert.ToInt32(cr["menu_index"]),
                        isVisible = Convert.ToBoolean(cr["isVisible"])
                    });
                }

                parent.subItems = children;
            }

            db.closeConnection();
            return result;
        }
    }
}
