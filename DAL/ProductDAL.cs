using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class ProductDAL
    {
        private readonly DbConnect _db = new DbConnect();

        // -------------------------------
        // MAP PRODUCT (Full Data Mapping)
        // -------------------------------
        private Product Map(SqlDataReader r)
        {
            return new Product
            {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                CategoryId = r.IsDBNull(r.GetOrdinal("CategoryId")) ? null : r.GetInt32(r.GetOrdinal("CategoryId")),
                CategoryName = r.IsDBNull(r.GetOrdinal("CategoryName")) ? "" : r.GetString(r.GetOrdinal("CategoryName")),
                Name = r.GetString(r.GetOrdinal("Name")),
                Description = r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description")),
                Price = r.GetDecimal(r.GetOrdinal("Price")),
                StockQuantity = r.IsDBNull(r.GetOrdinal("stock_quantity")) ? 0 : r.GetInt32(r.GetOrdinal("stock_quantity")),
                ImageUrl = r.IsDBNull(6) ? "" : r.GetString(6),
                IsActive = r.GetBoolean(r.GetOrdinal("is_active")),
                CreatedAt = r.GetDateTime(r.GetOrdinal("created_at")),
                Discount = r.IsDBNull(r.GetOrdinal("Discount"))
                    ? 0
                    : Convert.ToDecimal(r["Discount"])

            };
        }

        // -------------------------------
        // BASE QUERY FOR ALL PRODUCT LOAD
        // -------------------------------
        private string BaseSelectQuery = @"
            SELECT 
                p.id AS Id,
                p.category_id AS CategoryId,
                p.name AS Name,
                p.description AS Description,
                p.price AS Price,
                p.stock_quantity,
                p.image_url,
                p.is_active,
                p.created_at,
                c.name AS CategoryName,
                ISNULL(cp.discount_value, 0) AS Discount
            FROM products p
            LEFT JOIN categories c ON p.category_id = c.id
            LEFT JOIN coupons cp ON cp.id = p.coupon_id 
                AND cp.is_active = 1 
                AND GETDATE() BETWEEN cp.start_date AND cp.end_date
        ";

        // ===============================
        // GET ALL PRODUCTS
        // ===============================
        public List<Product> GetAllProducts()
        {
            List<Product> list = new();
            string sql = BaseSelectQuery + " WHERE p.is_active = 1 ORDER BY p.created_at DESC";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(Map(r));
            _db.CloseConnection();
            return list;
        }

        // ===============================
        // GET PRODUCT BY ID
        // ===============================
        public Product? GetProductById(int id)
        {
            Product? p = null;

            string sql = BaseSelectQuery + " WHERE p.id = @id";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();

            if (r.Read()) p = Map(r);
            _db.CloseConnection();

            return p;
        }

        // ===============================
        // GET PRODUCT BY CATEGORY
        // ===============================
        public List<Product> GetProductsByCategory(int categoryId)
        {
            List<Product> list = new();

            string sql = BaseSelectQuery + @"
                WHERE p.category_id = @cat AND p.is_active = 1
                ORDER BY p.created_at DESC";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            cmd.Parameters.AddWithValue("@cat", categoryId);
            using var r = cmd.ExecuteReader();

            while (r.Read()) list.Add(Map(r));
            _db.CloseConnection();
            return list;
        }

        // ===============================
        // GET FEATURED PRODUCTS
        // ===============================
        public List<Product> GetFeaturedProducts(int limit = 4)
        {
            List<Product> list = new();

            string sql = BaseSelectQuery + $@"
                WHERE p.is_active = 1
                ORDER BY p.created_at DESC
                OFFSET 0 ROWS FETCH NEXT {limit} ROWS ONLY";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            using var r = cmd.ExecuteReader();

            while (r.Read()) list.Add(Map(r));
            _db.CloseConnection();
            return list;
        }

        // ===============================
        // GET BEST SELLER
        // ===============================
        public List<Product> GetBestSellerProducts(int limit = 4)
        {
            List<Product> list = new();

            string sql = @"
                SELECT TOP (@limit)
                    p.id AS Id,
                    p.category_id AS CategoryId,
                    p.name AS Name,
                    p.description AS Description,
                    p.price AS Price,
                    p.stock_quantity,
                    p.image_url,
                    p.is_active,
                    p.created_at,
                    c.name AS CategoryName,
                    ISNULL(cp.discount_value, 0) AS Discount,
                    SUM(oi.quantity) AS TotalSold
                FROM products p
                LEFT JOIN order_items oi ON p.id = oi.product_id
                LEFT JOIN categories c ON p.category_id = c.id
                LEFT JOIN coupons cp ON cp.id = p.coupon_id 
                    AND cp.is_active = 1 
                    AND GETDATE() BETWEEN cp.start_date AND cp.end_date
                WHERE p.is_active = 1
                GROUP BY p.id, p.category_id, p.name, p.description, p.price,
                         p.stock_quantity, p.image_url, p.is_active, 
                         p.created_at, c.name, cp.discount_value
                ORDER BY TotalSold DESC, created_at DESC";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            cmd.Parameters.AddWithValue("@limit", limit);
            using var r = cmd.ExecuteReader();

            while (r.Read()) list.Add(Map(r));
            _db.CloseConnection();
            return list;
        }

        // ===============================
        // RELATED PRODUCTS
        // ===============================
        public List<Product> GetRelatedProducts(int productId, int limit = 4)
        {
            List<Product> list = new();

            string sql = BaseSelectQuery + @"
                WHERE p.category_id = (SELECT category_id FROM products WHERE id = @id)
                  AND p.id <> @id
                ORDER BY NEWID()
                OFFSET 0 ROWS FETCH NEXT @limit ROWS ONLY";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            cmd.Parameters.AddWithValue("@id", productId);
            cmd.Parameters.AddWithValue("@limit", limit);
            using var r = cmd.ExecuteReader();

            while (r.Read()) list.Add(Map(r));
            _db.CloseConnection();

            return list;
        }

        // ===============================
        // SEARCH
        // ===============================
        public List<Product> SearchProducts(string keyword)
        {
            List<Product> list = new();

            string sql = BaseSelectQuery + @"
                WHERE p.is_active = 1
                AND (p.name LIKE @kw OR p.description LIKE @kw)
                ORDER BY p.created_at DESC";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
            using var r = cmd.ExecuteReader();

            while (r.Read()) list.Add(Map(r));
            _db.CloseConnection();
            return list;
        }

        // ===============================
        // PAGINATION
        // ===============================
        public List<Product> GetProducts_Pagination(int page, int pageSize, int? categoryId, string sort)
        {
            List<Product> list = new();
            int skip = (page - 1) * pageSize;

            string sortQuery = sort switch
            {
                "price_asc" => " ORDER BY p.price ASC ",
                "price_desc" => " ORDER BY p.price DESC ",
                "name_asc" => " ORDER BY p.name ASC ",
                "name_desc" => " ORDER BY p.name DESC ",
                _ => " ORDER BY p.created_at DESC "
            };

            string sql = BaseSelectQuery + @"
                WHERE p.is_active = 1 "
                + (categoryId.HasValue ? " AND p.category_id = @cat " : "") +
                sortQuery +
                $" OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());

            if (categoryId.HasValue)
                cmd.Parameters.AddWithValue("@cat", categoryId.Value);

            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(Map(r));

            _db.CloseConnection();
            return list;
        }

        public int GetTotalProducts(int? categoryId)
        {
            string sql = "SELECT COUNT(*) FROM products WHERE is_active = 1";

            if (categoryId.HasValue)
                sql += " AND category_id = @cat";

            _db.OpenConnection();
            using var cmd = new SqlCommand(sql, _db.GetConnection());
            if (categoryId.HasValue)
                cmd.Parameters.AddWithValue("@cat", categoryId);

            int total = (int)cmd.ExecuteScalar();
            _db.CloseConnection();

            return total;
        }
    }
}
