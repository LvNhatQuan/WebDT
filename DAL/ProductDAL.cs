using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;
namespace WebDT.DAL
{
    public class ProductDAL
    {
        DbConnect connect = new DbConnect();

        public List<Product> GetListProduct(int? categoryId)
        {
            connect.openConnection();
            List<Product> list = new List<Product>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"SELECT 
                    p.id AS Id,
                    p.category_id AS CategoryId,
                    p.name AS Name,
                    p.description AS Description,
                    p.price AS Price,
                    p.stock_quantity AS Stock_quantity,
                    p.image_url AS Image_url,
                    p.is_active AS Is_active,
                    p.created_at AS Created_at,
                    c.name AS CategoryName
                    FROM products p
                    LEFT JOIN categories c ON p.category_id = c.id
                    WHERE p.is_active = 1";

                // Thêm điều kiện WHERE nếu có categoryId
                if (categoryId.HasValue)
                {
                    query += " AND p.category_id = @CategoryId";
                    command.Parameters.AddWithValue("@CategoryId", categoryId.Value);
                }

                query += " ORDER BY p.created_at DESC";

                command.CommandText = query;

                // Đọc dữ liệu từ cơ sở dữ liệu
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            Name = reader["Name"].ToString() ?? "",
                            Description = reader["Description"].ToString() ?? "",
                            Price = Convert.ToInt32(reader["Price"]),
                            Stock_quantity = Convert.ToInt32(reader["Stock_quantity"]),
                            Image_url = reader["Image_url"].ToString() ?? "",
                            Is_active = Convert.ToBoolean(reader["Is_active"]),
                            Created_at = DateTime.Parse(reader["Created_at"]?.ToString() ?? DateTime.Now.ToString())
                        };
                        list.Add(product);
                    }
                }
            }
            connect.closeConnection();
            return list;
        }

        public List<Product> GetProducts_Pagination(int? categoryId, int pageIndex, int pageSize, string sortOrder)
        {
            connect.openConnection();
            List<Product> list = new List<Product>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                // Xây dựng truy vấn sắp xếp
                string sortQuery = " ORDER BY p.created_at DESC ";
                if (!string.IsNullOrEmpty(sortOrder))
                {
                    switch (sortOrder.ToLower())
                    {
                        case "price_asc":
                            sortQuery = " ORDER BY p.price ASC ";
                            break;
                        case "price_desc":
                            sortQuery = " ORDER BY p.price DESC ";
                            break;
                        case "name_asc":
                            sortQuery = " ORDER BY p.name ASC ";
                            break;
                        case "name_desc":
                            sortQuery = " ORDER BY p.name DESC ";
                            break;
                        case "newest":
                            sortQuery = " ORDER BY p.created_at DESC ";
                            break;
                        case "oldest":
                            sortQuery = " ORDER BY p.created_at ASC ";
                            break;
                        default:
                            sortQuery = " ORDER BY p.created_at DESC ";
                            break;
                    }
                }

                // Xây dựng truy vấn chính
                string query = @"SELECT * FROM (
                            SELECT ROW_NUMBER() OVER (" + sortQuery + @") AS RowNumber,
                                   p.id AS Id,
                                   p.category_id AS CategoryId,
                                   p.name AS Name,
                                   p.description AS Description,
                                   p.price AS Price,
                                   p.stock_quantity AS Stock_quantity,
                                   p.image_url AS Image_url,
                                   p.is_active AS Is_active,
                                   p.created_at AS Created_at,
                                   c.name AS CategoryName
                            FROM products p
                            LEFT JOIN categories c ON p.category_id = c.id
                            WHERE p.is_active = 1";

                // Thêm điều kiện lọc category
                if (categoryId.HasValue)
                {
                    query += " AND p.category_id = @CategoryId";
                    command.Parameters.AddWithValue("@CategoryId", categoryId.Value);
                }

                // Thêm phân trang
                query += @") AS TableResult
                   WHERE TableResult.RowNumber BETWEEN (@PageIndex - 1) * @PageSize + 1
                   AND @PageIndex * @PageSize";

                command.CommandText = query;
                command.Parameters.AddWithValue("@PageIndex", pageIndex);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                // Thực thi truy vấn và đọc dữ liệu
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Product
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            Name = reader["Name"].ToString() ?? "",
                            Description = reader["Description"].ToString() ?? "",
                            Price = Convert.ToInt32(reader["Price"]),
                            Stock_quantity = Convert.ToInt32(reader["Stock_quantity"]),
                            Image_url = reader["Image_url"].ToString() ?? "",
                            Is_active = Convert.ToBoolean(reader["Is_active"]),
                            Created_at = DateTime.Parse(reader["Created_at"]?.ToString() ?? DateTime.Now.ToString())
                        });
                    }
                }
            }
            connect.closeConnection();
            return list;
        }

        public Product GetProductById(int id)
        {
            connect.openConnection();
            Product product = new Product();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"SELECT 
                    p.id AS Id,
                    p.category_id AS CategoryId,
                    p.name AS Name,
                    p.description AS Description,
                    p.price AS Price,
                    p.stock_quantity AS Stock_quantity,
                    p.image_url AS Image_url,
                    p.is_active AS Is_active,
                    p.created_at AS Created_at,
                    c.name AS CategoryName
                    FROM products p
                    LEFT JOIN categories c ON p.category_id = c.id
                    WHERE p.id = @Id";

                command.CommandText = query;
                command.Parameters.AddWithValue("@Id", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            Name = reader["Name"].ToString() ?? "",
                            Description = reader["Description"].ToString() ?? "",
                            Price = Convert.ToInt32(reader["Price"]),
                            Stock_quantity = Convert.ToInt32(reader["Stock_quantity"]),
                            Image_url = reader["Image_url"].ToString() ?? "",
                            Is_active = Convert.ToBoolean(reader["Is_active"]),
                            Created_at = DateTime.Parse(reader["Created_at"]?.ToString() ?? DateTime.Now.ToString())
                        };
                    }
                }
            }
            connect.closeConnection();
            return product;
        }

        public List<Product> GetFeaturedProducts(int limitProduct)
        {
            connect.openConnection();
            List<Product> list = new List<Product>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                // Lấy sản phẩm bán chạy nhất dựa trên số lượng đã bán
                string query = @"SELECT TOP (@Limit) 
                    p.id AS Id,
                    p.category_id AS CategoryId,
                    p.name AS Name,
                    p.description AS Description,
                    p.price AS Price,
                    p.stock_quantity AS Stock_quantity,
                    p.image_url AS Image_url,
                    p.is_active AS Is_active,
                    p.created_at AS Created_at,
                    c.name AS CategoryName,
                    COALESCE(SUM(oi.quantity), 0) AS TotalSold
                    FROM products p
                    LEFT JOIN categories c ON p.category_id = c.id
                    LEFT JOIN order_items oi ON p.id = oi.product_id
                    WHERE p.is_active = 1
                    GROUP BY p.id, p.category_id, p.name, p.description, p.price, 
                             p.stock_quantity, p.image_url, p.is_active, p.created_at, c.name
                    ORDER BY TotalSold DESC, p.created_at DESC";

                command.CommandText = query;
                command.Parameters.AddWithValue("@Limit", limitProduct);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            Name = reader["Name"].ToString() ?? "",
                            Description = reader["Description"].ToString() ?? "",
                            Price = Convert.ToInt32(reader["Price"]),
                            Stock_quantity = Convert.ToInt32(reader["Stock_quantity"]),
                            Image_url = reader["Image_url"].ToString() ?? "",
                            Is_active = Convert.ToBoolean(reader["Is_active"]),
                            Created_at = DateTime.Parse(reader["Created_at"]?.ToString() ?? DateTime.Now.ToString())
                        };
                        list.Add(product);
                    }
                }
            }
            connect.closeConnection();
            return list;
        }

        public List<Product> GetBestSellerProducts(int topCount)
        {
            connect.openConnection();
            List<Product> list = new List<Product>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"SELECT TOP (@TopCount) 
                    p.id AS Id,
                    p.category_id AS CategoryId,
                    p.name AS Name,
                    p.description AS Description,
                    p.price AS Price,
                    p.stock_quantity AS Stock_quantity,
                    p.image_url AS Image_url,
                    p.is_active AS Is_active,
                    p.created_at AS Created_at,
                    c.name AS CategoryName,
                    SUM(oi.quantity) AS TotalSold,
                    SUM(oi.total_price) AS TotalRevenue
                    FROM products p
                    LEFT JOIN categories c ON p.category_id = c.id
                    LEFT JOIN order_items oi ON p.id = oi.product_id
                    WHERE p.is_active = 1
                    GROUP BY p.id, p.category_id, p.name, p.description, p.price, 
                             p.stock_quantity, p.image_url, p.is_active, p.created_at, c.name
                    HAVING SUM(oi.quantity) > 0
                    ORDER BY TotalSold DESC";

                command.CommandText = query;
                command.Parameters.AddWithValue("@TopCount", topCount);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            Name = reader["Name"].ToString() ?? "",
                            Description = reader["Description"].ToString() ?? "",
                            Price = Convert.ToInt32(reader["Price"]),
                            Stock_quantity = Convert.ToInt32(reader["Stock_quantity"]),
                            Image_url = reader["Image_url"].ToString() ?? "",
                            Is_active = Convert.ToBoolean(reader["Is_active"]),
                            Created_at = DateTime.Parse(reader["Created_at"]?.ToString() ?? DateTime.Now.ToString())
                        };
                        list.Add(product);
                    }
                }
            }
            connect.closeConnection();
            return list;
        }

        public List<Product> SearchProducts(string keyword)
        {
            connect.openConnection();
            List<Product> list = new List<Product>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                // Xây dựng câu truy vấn tìm kiếm
                string query = @"
                    SELECT p.id AS Id,
                           p.category_id AS CategoryId,
                           p.name AS Name,
                           p.description AS Description,
                           p.price AS Price,
                           p.stock_quantity AS Stock_quantity,
                           p.image_url AS Image_url,
                           p.is_active AS Is_active,
                           p.created_at AS Created_at,
                           c.name AS CategoryName
                    FROM products p
                    LEFT JOIN categories c ON p.category_id = c.id
                    WHERE p.is_active = 1 
                    AND (p.name LIKE @Keyword OR p.description LIKE @Keyword)";

                command.CommandText = query;
                command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            Name = reader["Name"].ToString() ?? "",
                            Description = reader["Description"].ToString() ?? "",
                            Price = Convert.ToInt32(reader["Price"]),
                            Stock_quantity = Convert.ToInt32(reader["Stock_quantity"]),
                            Image_url = reader["Image_url"].ToString() ?? "",
                            Is_active = Convert.ToBoolean(reader["Is_active"]),
                            Created_at = DateTime.Parse(reader["Created_at"]?.ToString() ?? DateTime.Now.ToString())
                        };
                        list.Add(product);
                    }
                }
            }

            connect.closeConnection();
            return list;
        }

        // Thêm phương thức để lấy tổng số sản phẩm (cho phân trang)
        public int GetTotalProducts(int? categoryId)
        {
            connect.openConnection();
            int total = 0;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"SELECT COUNT(*) FROM products WHERE is_active = 1";

                if (categoryId.HasValue)
                {
                    query += " AND category_id = @CategoryId";
                    command.Parameters.AddWithValue("@CategoryId", categoryId.Value);
                }

                command.CommandText = query;
                total = Convert.ToInt32(command.ExecuteScalar());
            }

            connect.closeConnection();
            return total;
        }

        public List<Product> GetRelatedProducts(int productId, int limit)
        {
            connect.openConnection();
            var list = new List<Product>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"
                    SELECT TOP (@Limit)
                        p.id AS Id,
                        p.category_id AS CategoryId,
                        p.name AS Name,
                        p.description AS Description,
                        p.price AS Price,
                        p.stock_quantity AS Stock_quantity,
                        p.image_url AS Image_url,
                        p.is_active AS Is_active,
                        p.created_at AS Created_at
                    FROM products p
                    INNER JOIN products currentP ON currentP.category_id = p.category_id
                    WHERE currentP.id = @ProductId
                      AND p.id <> @ProductId
                      AND p.is_active = 1
                    ORDER BY p.created_at DESC;";

                command.CommandText = query;
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@Limit", limit);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new Product
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            CategoryId = Convert.ToInt32(reader["CategoryId"]),
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Description = reader["Description"]?.ToString() ?? string.Empty,
                            Price = Convert.ToInt32(reader["Price"]),
                            Stock_quantity = Convert.ToInt32(reader["Stock_quantity"]),
                            Image_url = reader["Image_url"]?.ToString() ?? string.Empty,
                            Is_active = Convert.ToBoolean(reader["Is_active"]),
                            Created_at = DateTime.Parse(reader["Created_at"]?.ToString() ?? DateTime.Now.ToString())
                        };

                        list.Add(product);
                    }
                }
            }

            connect.closeConnection();
            return list;
        }
    }
}
