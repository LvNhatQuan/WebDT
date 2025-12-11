using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class CategoryDAL
    {
        private readonly DbConnect db = new DbConnect();

        // =============================
        // MAP CATEGORY BASIC
        // =============================
        private CategoryMenu Map(SqlDataReader r)
        {
            return new CategoryMenu
            {
                Id = Convert.ToInt32(r["id"]),
                Name = r["name"].ToString(),
                Description = r["description"] != DBNull.Value ? r["description"].ToString() : "",
                Count = r["Count"] != DBNull.Value ? Convert.ToInt32(r["Count"]) : 0
            };
        }

        // =============================
        // 1. Lấy toàn bộ danh mục
        // =============================
        public List<CategoryMenu> GetAll()
        {
            List<CategoryMenu> categories = new();
            db.openConnection();

            using var cmd = new SqlCommand("SELECT * FROM categories ORDER BY id ASC", db.getConnecttion());
            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                categories.Add(new CategoryMenu
                {
                    Id = Convert.ToInt32(r["id"]),
                    Name = r["name"].ToString(),
                    Description = r["description"].ToString(),
                    Count = 0 // không có count
                });
            }

            db.closeConnection();
            return categories;
        }

        // ================================================
        // 2. Lấy danh mục + số lượng sản phẩm trong danh mục
        // ================================================
        public List<CategoryMenu> GetAllWithCount()
        {
            List<CategoryMenu> list = new();
            db.openConnection();

            string query = @"
                SELECT 
                    c.id, c.name, c.description,
                    COUNT(p.id) AS Count
                FROM categories c
                LEFT JOIN products p ON p.category_id = c.id AND p.is_active = 1
                GROUP BY c.id, c.name, c.description
                ORDER BY c.id ASC";

            using var cmd = new SqlCommand(query, db.getConnecttion());
            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(Map(r));
            }

            db.closeConnection();
            return list;
        }

        // =============================
        // 3. Lấy danh mục theo ID
        // =============================
        public CategoryMenu? GetById(int id)
        {
            db.openConnection();
            CategoryMenu? category = null;

            using var cmd = new SqlCommand("SELECT * FROM categories WHERE id=@id", db.getConnecttion());
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                category = new CategoryMenu
                {
                    Id = Convert.ToInt32(r["id"]),
                    Name = r["name"].ToString(),
                    Description = r["description"]?.ToString(),
                    Count = 0
                };
            }

            db.closeConnection();
            return category;
        }
    }
}
