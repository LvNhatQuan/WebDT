using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class ProductDAL
    {
        private readonly DbConnect _db = new DbConnect();

        // Lấy toàn bộ sản phẩm
        public List<Product> GetAllProducts()
        {
            List<Product> list = new();
            string sql = @"SELECT id, category_id, name, description, price, stock_quantity, image_url, is_active, created_at
                           FROM products
                           ORDER BY created_at DESC";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            using var reader = cmd.ExecuteReader();

            while (reader.Read()) list.Add(MapToProduct(reader));

            _db.CloseConnection();
            return list;
        }

        // Lấy sản phẩm theo ID
        public Product? GetProductById(int id)
        {
            string sql = @"SELECT id, category_id, name, description, price, stock_quantity, image_url, is_active, created_at
                           FROM products
                           WHERE id = @id";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();

            Product? product = null;
            if (reader.Read()) product = MapToProduct(reader);

            _db.CloseConnection();
            return product;
        }

        // Lấy sản phẩm theo danh mục
        public List<Product> GetProductsByCategory(int categoryId)
        {
            List<Product> list = new();
            string sql = @"SELECT id, category_id, name, description, price, stock_quantity, image_url, is_active, created_at
                           FROM products
                           WHERE category_id = @cat
                           ORDER BY created_at DESC";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            cmd.Parameters.AddWithValue("@cat", categoryId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read()) list.Add(MapToProduct(reader));

            _db.CloseConnection();
            return list;
        }

        // =========================
        // FEATURED PRODUCTS
        // =========================
        public List<Product> GetFeaturedProducts(int limit = 4)
        {
            List<Product> list = new();

            string sql = @$"
                SELECT TOP ({limit}) id, category_id, name, description, price, stock_quantity, image_url, is_active, created_at
                FROM products
                WHERE is_active = 1
                ORDER BY created_at DESC";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            using var reader = cmd.ExecuteReader();

            while (reader.Read()) list.Add(MapToProduct(reader));

            _db.CloseConnection();
            return list;
        }

        // =========================
        // BEST SELLER PRODUCTS
        // =========================
        public List<Product> GetBestSellerProducts(int limit = 4)
        {
            List<Product> list = new();

            string sql = @$"
                SELECT TOP ({limit}) p.id, p.category_id, p.name, p.description, p.price, 
                       p.stock_quantity, p.image_url, p.is_active, p.created_at
                FROM products p
                JOIN order_items oi ON oi.product_id = p.id
                GROUP BY p.id, p.category_id, p.name, p.description, p.price, 
                         p.stock_quantity, p.image_url, p.is_active, p.created_at
                ORDER BY SUM(oi.quantity) DESC";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            using var reader = cmd.ExecuteReader();

            while (reader.Read()) list.Add(MapToProduct(reader));

            _db.CloseConnection();
            return list;
        }

        // =========================
        // RELATED PRODUCTS
        // =========================
        public List<Product> GetRelatedProducts(int productId, int limit = 4)
        {
            List<Product> list = new();

            string sql = @$"
                SELECT TOP ({limit}) id, category_id, name, description, price,
                       stock_quantity, image_url, is_active, created_at
                FROM products
                WHERE category_id = (SELECT category_id FROM products WHERE id = @id)
                  AND id <> @id
                ORDER BY NEWID()";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            cmd.Parameters.AddWithValue("@id", productId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read()) list.Add(MapToProduct(reader));

            _db.CloseConnection();
            return list;
        }

        // =========================
        // SEARCH
        // =========================
        public List<Product> SearchProducts(string keyword)
        {
            List<Product> list = new();

            string sql = @"SELECT id, category_id, name, description, price, stock_quantity, image_url, is_active, created_at
                           FROM products
                           WHERE name LIKE '%' + @kw + '%'";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            cmd.Parameters.AddWithValue("@kw", keyword);
            using var reader = cmd.ExecuteReader();

            while (reader.Read()) list.Add(MapToProduct(reader));

            _db.CloseConnection();
            return list;
        }

        // =========================
        // PAGINATION
        // =========================
        public List<Product> GetProducts_Pagination(int page, int pageSize)
        {
            List<Product> list = new();
            int skip = (page - 1) * pageSize;

            string sql = @$"
                SELECT id, category_id, name, description, price, stock_quantity, image_url, is_active, created_at
                FROM products
                ORDER BY created_at DESC
                OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            using var reader = cmd.ExecuteReader();

            while (reader.Read()) list.Add(MapToProduct(reader));

            _db.CloseConnection();
            return list;
        }

        public int GetTotalProducts()
        {
            string sql = "SELECT COUNT(*) FROM products";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            int total = (int)cmd.ExecuteScalar();
            _db.CloseConnection();

            return total;
        }

        // =========================
        // MAPPING FUNCTION
        // =========================
        private Product MapToProduct(SqlDataReader r)
        {
            return new Product
            {
                Id = r.GetInt32(0),
                CategoryId = r.IsDBNull(1) ? null : r.GetInt32(1),
                Name = r.GetString(2),
                Description = r.IsDBNull(3) ? null : r.GetString(3),
                Price = r.GetDecimal(4),
                StockQuantity = r.IsDBNull(5) ? 0 : r.GetInt32(5),
                ImageUrl = r.IsDBNull(6) ? null : r.GetString(6),
                IsActive = r.GetBoolean(7),
                CreatedAt = r.GetDateTime(8)
            };
        }
    }
}
