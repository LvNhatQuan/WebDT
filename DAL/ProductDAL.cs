using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class ProductDAL
    {
        private readonly DbConnect db = new DbConnect();

        // ============================
        // Map Product từ SQL
        // ============================
        private Product MapProduct(SqlDataReader r)
        {
            return new Product
            {
                Id = Convert.ToInt32(r["id"]),
                CategoryId = r["category_id"] != DBNull.Value ? Convert.ToInt32(r["category_id"]) : 0,
                Name = r["name"].ToString(),
                Description = r["description"].ToString(),
                Price = Convert.ToInt32(r["price"]),
                Stock_quantity = Convert.ToInt32(r["stock_quantity"]),
                Image_url = r["image_url"].ToString(),
                Is_active = Convert.ToBoolean(r["is_active"]),
                Created_at = Convert.ToDateTime(r["created_at"])
            };
        }

        // ============================
        // 1) Lấy tất cả sản phẩm (dùng cho Admin)
        // ============================
        public List<Product> GetAll()
        {
            db.openConnection();
            List<Product> list = new List<Product>();

            using var cmd = new SqlCommand("SELECT * FROM products ORDER BY id DESC", db.getConnecttion());
            using var r = cmd.ExecuteReader();

            while (r.Read())
                list.Add(MapProduct(r));

            db.closeConnection();
            return list;
        }

        // ============================
        // 2) Lấy sản phẩm theo ID
        // ============================
        public Product? GetById(int id)
        {
            db.openConnection();
            Product? product = null;

            using var cmd = new SqlCommand("SELECT * FROM products WHERE id=@id AND is_active=1", db.getConnecttion());
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (r.Read()) product = MapProduct(r);

            db.closeConnection();
            return product;
        }

        // ============================
        // 3) Lấy sản phẩm liên quan
        // ============================
        public List<Product> GetRelated(int excludeId, int categoryId, int limit = 6)
        {
            db.openConnection();
            List<Product> list = new();

            using var cmd = new SqlCommand(@"
                SELECT TOP(@limit) * 
                FROM products 
                WHERE category_id=@cid AND id<>@id AND is_active=1
                ORDER BY created_at DESC", db.getConnecttion());

            cmd.Parameters.AddWithValue("@limit", limit);
            cmd.Parameters.AddWithValue("@cid", categoryId);
            cmd.Parameters.AddWithValue("@id", excludeId);

            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapProduct(r));

            db.closeConnection();
            return list;
        }

        // ============================
        // 4) Lấy sản phẩm nổi bật (dùng cho trang chủ)
        // ============================
        public List<Product> GetFeatured(int limit = 8)
        {
            db.openConnection();
            List<Product> list = new();

            using var cmd = new SqlCommand(@"
                SELECT TOP(@limit) * 
                FROM products 
                WHERE is_active=1
                ORDER BY created_at DESC",
                db.getConnecttion());

            cmd.Parameters.AddWithValue("@limit", limit);

            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapProduct(r));

            db.closeConnection();
            return list;
        }

        // ============================
        // 5) Lấy sản phẩm bán chạy (dựa vào order_items)
        // ============================
        public List<Product> GetBestSeller(int limit = 8)
        {
            db.openConnection();
            List<Product> list = new();

            using var cmd = new SqlCommand(@"
                SELECT TOP(@limit) p.*, SUM(oi.quantity) AS total_sold
                FROM order_items oi
                JOIN products p ON p.id = oi.product_id
                GROUP BY p.id, p.category_id, p.name, p.description, p.price, 
                         p.stock_quantity, p.image_url, p.is_active, p.created_at
                ORDER BY total_sold DESC",
                db.getConnecttion());

            cmd.Parameters.AddWithValue("@limit", limit);

            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapProduct(r));

            db.closeConnection();
            return list;
        }

        // ============================
        // 6) Đếm số sản phẩm (dùng cho phân trang)
        // ============================
        public int CountProducts(int? categoryId)
        {
            db.openConnection();

            string query = categoryId == null ?
                "SELECT COUNT(*) FROM products WHERE is_active=1" :
                "SELECT COUNT(*) FROM products WHERE is_active=1 AND category_id=@cid";

            using var cmd = new SqlCommand(query, db.getConnecttion());

            if (categoryId != null)
                cmd.Parameters.AddWithValue("@cid", categoryId);

            int total = (int)cmd.ExecuteScalar();
            db.closeConnection();

            return total;
        }

        // ============================
        // 7) Lấy danh sách theo phân trang + filter + sort
        // ============================
        public List<Product> GetPaged(
            int pageIndex,
            int pageSize,
            int? categoryId,
            string sort)
        {
            db.openConnection();
            List<Product> list = new();

            string orderBy = sort switch
            {
                "price_asc" => "price ASC",
                "price_desc" => "price DESC",
                "name_asc" => "name ASC",
                "name_desc" => "name DESC",
                _ => "created_at DESC"
            };

            string baseQuery = @"
                SELECT * FROM (
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY {0}) AS RowNum,
                        *
                    FROM products
                    WHERE is_active=1 {1}
                ) AS T
                WHERE RowNum BETWEEN @start AND @end";

            string filter = categoryId != null ? "AND category_id=@cid" : "";

            string finalQuery = string.Format(baseQuery, orderBy, filter);

            using var cmd = new SqlCommand(finalQuery, db.getConnecttion());

            cmd.Parameters.AddWithValue("@start", (pageIndex - 1) * pageSize + 1);
            cmd.Parameters.AddWithValue("@end", pageIndex * pageSize);

            if (categoryId != null)
                cmd.Parameters.AddWithValue("@cid", categoryId);

            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapProduct(r));

            db.closeConnection();
            return list;
        }

        // ============================
        // 8) Tìm kiếm sản phẩm
        // ============================
        public List<Product> Search(string keyword)
        {
            db.openConnection();
            List<Product> list = new();

            using var cmd = new SqlCommand(
                "SELECT * FROM products WHERE name LIKE @kw AND is_active=1",
                db.getConnecttion());

            cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapProduct(r));

            db.closeConnection();
            return list;
        }
    }
}
