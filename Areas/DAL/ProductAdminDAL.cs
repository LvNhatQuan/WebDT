using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using WebDT.Areas.Admin.Models;
using WebDT.Database; 

namespace WebDT.Areas.Admin.DAL
{
    public class ProductAdminDAL
    {
        DbConnect connect = new DbConnect();

        // ==================== LẤY TẤT CẢ SẢN PHẨM ====================
        public List<ProductAdmin> getAll()
        {
            connect.openConnection();

            List<ProductAdmin> list = new List<ProductAdmin>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"
                    SELECT 
                        a.id              AS ProductId,
                        a.name            AS ProductName,
                        a.description     AS ProductDescription,
                        a.image_url       AS ProductImageUrl,
                        a.price           AS ProductPrice,
                        a.stock_quantity  AS ProductStockQuantity,
                        a.is_active       AS ProductIsActive,
                        a.created_at      AS ProductCreatedAt,
                        b.id              AS CategoryId,
                        b.name            AS CategoryName
                    FROM products a
                    LEFT JOIN categories b ON a.category_id = b.id";

                command.CommandText = query;

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var product = new ProductAdmin()
                    {
                        Id = Convert.ToInt32(reader["ProductId"]),
                        Name = reader["ProductName"]?.ToString() ?? string.Empty,
                        Description = reader["ProductDescription"]?.ToString(),
                        ImageUrl = reader["ProductImageUrl"]?.ToString(),

                        Price = reader["ProductPrice"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["ProductPrice"])
                                    : 0m,

                        StockQuantity = reader["ProductStockQuantity"] != DBNull.Value
                                    ? Convert.ToInt32(reader["ProductStockQuantity"])
                                    : 0,

                        IsActive = reader["ProductIsActive"] != DBNull.Value
                                    && Convert.ToBoolean(reader["ProductIsActive"]),

                        CreatedAt = reader["ProductCreatedAt"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["ProductCreatedAt"])
                                    : DateTime.MinValue,

                        CategoryId = reader["CategoryId"] != DBNull.Value
                                    ? Convert.ToInt32(reader["CategoryId"])
                                    : 0,

                        CategoryName = reader["CategoryName"]?.ToString()
                    };

                    list.Add(product);
                }
            }

            connect.closeConnection();
            return list;
        }

        // ==================== THÊM MỚI SẢN PHẨM ====================
        public bool AddNew(ProductFormAdmin productNew)
        {
            connect.openConnection();

            int rowsAffected = 0;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"
                    INSERT INTO products
                        (name, description, image_url, price, stock_quantity, is_active, created_at, category_id)
                    VALUES
                        (@name, @description, @image_url, @price, @stock_quantity, @is_active, @created_at, @category_id);";

                command.CommandText = query;

                command.Parameters.AddWithValue("@name", productNew.Name);

                command.Parameters.AddWithValue("@description",
                    string.IsNullOrEmpty(productNew.Description)
                        ? (object)DBNull.Value
                        : productNew.Description);

                command.Parameters.AddWithValue("@image_url",
                    string.IsNullOrEmpty(productNew.ImageUrl)
                        ? (object)DBNull.Value
                        : productNew.ImageUrl);

                command.Parameters.AddWithValue("@price", productNew.Price);
                command.Parameters.AddWithValue("@stock_quantity", productNew.StockQuantity);
                command.Parameters.AddWithValue("@is_active", productNew.IsActive);

                var createdAt = productNew.CreatedAt == default(DateTime)
                    ? DateTime.Now
                    : productNew.CreatedAt;
                command.Parameters.AddWithValue("@created_at", createdAt);

                command.Parameters.AddWithValue("@category_id", productNew.CategoryId);

                Console.WriteLine("command Insert Product: " + command.CommandText);

                rowsAffected = command.ExecuteNonQuery();
            }

            connect.closeConnection();
            return rowsAffected > 0;
        }

        // ==================== LẤY SẢN PHẨM THEO ID ====================
        public ProductAdmin GetProductById(int id)
        {
            connect.openConnection();

            ProductAdmin? product = null;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"
                    SELECT 
                        a.id             AS ProductId,
                        a.name           AS ProductName,
                        a.description    AS ProductDescription,
                        a.image_url      AS ProductImageUrl,
                        a.price          AS ProductPrice,
                        a.stock_quantity AS ProductStockQuantity,
                        a.is_active      AS ProductIsActive,
                        a.created_at     AS ProductCreatedAt,
                        b.id             AS CategoryId,
                        b.name           AS CategoryName
                    FROM products a
                    LEFT JOIN categories b ON a.category_id = b.id
                    WHERE a.id = @Id";

                command.CommandText = query;
                command.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    product = new ProductAdmin()
                    {
                        Id = Convert.ToInt32(reader["ProductId"]),
                        Name = reader["ProductName"]?.ToString() ?? string.Empty,
                        Description = reader["ProductDescription"]?.ToString(),
                        ImageUrl = reader["ProductImageUrl"]?.ToString(),

                        Price = reader["ProductPrice"] != DBNull.Value
                                    ? Convert.ToDecimal(reader["ProductPrice"])
                                    : 0m,

                        StockQuantity = reader["ProductStockQuantity"] != DBNull.Value
                                    ? Convert.ToInt32(reader["ProductStockQuantity"])
                                    : 0,

                        IsActive = reader["ProductIsActive"] != DBNull.Value
                                    && Convert.ToBoolean(reader["ProductIsActive"]),

                        CreatedAt = reader["ProductCreatedAt"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["ProductCreatedAt"])
                                    : DateTime.MinValue,

                        CategoryId = reader["CategoryId"] != DBNull.Value
                                    ? Convert.ToInt32(reader["CategoryId"])
                                    : 0,

                        CategoryName = reader["CategoryName"]?.ToString()
                    };
                }
            }

            connect.closeConnection();
            return product ?? new ProductAdmin();
        }

        public bool UpdateProduct(ProductFormAdmin productNew, int Id)
        {
            connect.openConnection();
            int isSuccess = 0;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"
            UPDATE products
            SET 
                name           = @name,
                description    = @description,
                image_url      = @image_url,
                price          = @price,
                stock_quantity = @stock_quantity,
                is_active      = @is_active,
                category_id    = @category_id
            WHERE id = @id";

                command.CommandText = query;

                command.Parameters.AddWithValue("@id", Id);
                command.Parameters.AddWithValue("@name", productNew.Name);

                command.Parameters.AddWithValue("@description",
                    string.IsNullOrEmpty(productNew.Description)
                        ? (object)DBNull.Value
                        : productNew.Description);

                command.Parameters.AddWithValue("@image_url",
                    string.IsNullOrEmpty(productNew.ImageUrl)
                        ? (object)DBNull.Value
                        : productNew.ImageUrl);

                command.Parameters.AddWithValue("@price", productNew.Price);
                command.Parameters.AddWithValue("@stock_quantity", productNew.StockQuantity);
                command.Parameters.AddWithValue("@is_active", productNew.IsActive);
                command.Parameters.AddWithValue("@category_id", productNew.CategoryId);

                isSuccess = command.ExecuteNonQuery();
            }

            connect.closeConnection();
            return isSuccess > 0;
        }
        public bool DeleteProduct(int Id)
        {
            connect.openConnection();

            int isSuccess = 0;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"DELETE FROM products 
                         WHERE id = @id";

                command.CommandText = query;
                command.Parameters.AddWithValue("@id", Id);

                isSuccess = command.ExecuteNonQuery();
            }

            connect.closeConnection();
            return isSuccess > 0;
        }

    }
}
