using Microsoft.Data.SqlClient;
using WebDT.Models;
using WebDT.Database;
using System.Linq;
using WebDT.Areas.Admin.Models;

namespace WebDT.Areas.Admin.DAL
{
    public class CouponAdminDAL
    {
        DbConnect connect = new DbConnect();

        // GET ALL COUPONS
        public List<CouponAdmin> GetAll()
        {
            connect.openConnection();
            List<CouponAdmin> list = new();

            using var cmd = new SqlCommand("SELECT * FROM coupons ORDER BY id DESC", connect.getConnecttion());
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new CouponAdmin
                {
                    Id = (int)reader["id"],
                    EventName = reader["event_name"].ToString()!,
                    DiscountValue = (int)reader["discount_value"],
                    StartDate = (DateTime)reader["start_date"],
                    EndDate = (DateTime)reader["end_date"],
                    IsActive = reader["is_active"] != DBNull.Value && (bool)reader["is_active"]
                });
            }

            connect.closeConnection();
            return list;
        }

        // GET ACTIVE COUPONS
        public List<CouponAdmin> GetActiveCoupons()
        {
            connect.openConnection();
            List<CouponAdmin> list = new();

            using var cmd = new SqlCommand("SELECT * FROM coupons WHERE is_active = 1 AND GETDATE() BETWEEN start_date AND end_date ORDER BY id DESC", connect.getConnecttion());
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new CouponAdmin
                {
                    Id = (int)reader["id"],
                    EventName = reader["event_name"].ToString()!,
                    DiscountValue = (int)reader["discount_value"],
                    StartDate = (DateTime)reader["start_date"],
                    EndDate = (DateTime)reader["end_date"],
                    IsActive = (bool)reader["is_active"]
                });
            }

            connect.closeConnection();
            return list;
        }

        // GET COUPON BY ID
        public CouponAdmin? GetById(int id)
        {
            connect.openConnection();
            CouponAdmin? coupon = null;

            using var cmd = new SqlCommand("SELECT * FROM coupons WHERE id = @id", connect.getConnecttion());
            cmd.Parameters.AddWithValue("@id", id);

            var r = cmd.ExecuteReader();
            if (r.Read())
            {
                coupon = new CouponAdmin
                {
                    Id = (int)r["id"],
                    EventName = r["event_name"].ToString()!,
                    DiscountValue = (int)r["discount_value"],
                    StartDate = (DateTime)r["start_date"],
                    EndDate = (DateTime)r["end_date"],
                    IsActive = r["is_active"] != DBNull.Value && (bool)r["is_active"]
                };
            }

            connect.closeConnection();
            return coupon;
        }

        // CREATE COUPON
        public bool Create(CouponAdmin coupon)
        {
            connect.openConnection();

            using var cmd = new SqlCommand(@"
                INSERT INTO coupons (event_name, discount_value, start_date, end_date, is_active)
                VALUES (@EventName, @DiscountValue, @StartDate, @EndDate, @IsActive)",
                connect.getConnecttion());

            cmd.Parameters.AddWithValue("@EventName", coupon.EventName);
            cmd.Parameters.AddWithValue("@DiscountValue", coupon.DiscountValue);
            cmd.Parameters.AddWithValue("@StartDate", coupon.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", coupon.EndDate);
            cmd.Parameters.AddWithValue("@IsActive", coupon.IsActive);

            int result = cmd.ExecuteNonQuery();
            connect.closeConnection();

            return result > 0;
        }

        // UPDATE COUPON
        public bool Update(CouponAdmin coupon)
        {
            connect.openConnection();

            using var cmd = new SqlCommand(@"
                UPDATE coupons SET
                    event_name = @EventName,
                    discount_value = @DiscountValue,
                    start_date = @StartDate,
                    end_date = @EndDate,
                    is_active = @IsActive
                WHERE id = @Id",
                connect.getConnecttion());

            cmd.Parameters.AddWithValue("@Id", coupon.Id);
            cmd.Parameters.AddWithValue("@EventName", coupon.EventName);
            cmd.Parameters.AddWithValue("@DiscountValue", coupon.DiscountValue);
            cmd.Parameters.AddWithValue("@StartDate", coupon.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", coupon.EndDate);
            cmd.Parameters.AddWithValue("@IsActive", coupon.IsActive);

            int result = cmd.ExecuteNonQuery();
            connect.closeConnection();

            return result > 0;
        }

        // UPDATE STATUS
        public bool UpdateStatus(int id, bool isActive)
        {
            connect.openConnection();

            using var cmd = new SqlCommand(@"
                UPDATE coupons SET is_active = @IsActive
                WHERE id = @Id",
                connect.getConnecttion());

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@IsActive", isActive);

            int result = cmd.ExecuteNonQuery();
            connect.closeConnection();

            return result > 0;
        }

        // DELETE COUPON
        public bool Delete(int id)
        {
            connect.openConnection();

            // Kiểm tra xem coupon có đang được sử dụng trong các bảng khác không
            using var checkCmd = new SqlCommand(
                "SELECT COUNT(*) FROM products WHERE coupon_id = @id",
                connect.getConnecttion());
            checkCmd.Parameters.AddWithValue("@id", id);
            int productCount = (int)checkCmd.ExecuteScalar();

            if (productCount > 0)
            {
                // Nếu coupon đang được sử dụng trong products, chỉ cập nhật thành không hoạt động
                using var updateCmd = new SqlCommand(
                    "UPDATE coupons SET is_active = 0 WHERE id = @id",
                    connect.getConnecttion());
                updateCmd.Parameters.AddWithValue("@id", id);
                int updateResult = updateCmd.ExecuteNonQuery();
                connect.closeConnection();
                return updateResult > 0;
            }
            else
            {
                // Nếu không có ràng buộc, xóa coupon
                using var deleteCmd = new SqlCommand(
                    "DELETE FROM coupons WHERE id = @id",
                    connect.getConnecttion());
                deleteCmd.Parameters.AddWithValue("@id", id);
                int deleteResult = deleteCmd.ExecuteNonQuery();
                connect.closeConnection();
                return deleteResult > 0;
            }
        }

        // GET PRODUCTS BY COUPON ID
        public List<ProductAdmin> GetProductsByCouponId(int couponId)
        {
            connect.openConnection();
            List<ProductAdmin> list = new();

            using var cmd = new SqlCommand(@"
                SELECT p.id, p.name, p.description, p.image_url, p.price, 
                       p.stock_quantity, p.is_active, p.created_at,
                       c.name as CategoryName
                FROM products p
                LEFT JOIN categories c ON p.category_id = c.id
                WHERE p.coupon_id = @couponId
                ORDER BY p.name", connect.getConnecttion());

            cmd.Parameters.AddWithValue("@couponId", couponId);

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ProductAdmin
                {
                    Id = (int)reader["id"],
                    Name = reader["name"].ToString()!,
                    Description = reader["description"]?.ToString(),
                    ImageUrl = reader["image_url"]?.ToString(),
                    Price = (decimal)reader["price"],
                    StockQuantity = (int)reader["stock_quantity"],
                    IsActive = (bool)reader["is_active"],
                    CreatedAt = (DateTime)reader["created_at"],
                    CategoryName = reader["CategoryName"]?.ToString()
                });
            }

            connect.closeConnection();
            return list;
        }

        // ASSIGN COUPON TO PRODUCTS
        public bool AssignCouponToProducts(int couponId, List<int> productIds)
        {
            connect.openConnection();

            using var transaction = connect.getConnecttion().BeginTransaction();

            try
            {
                // Đầu tiên, xóa coupon khỏi tất cả sản phẩm
                using var clearCmd = new SqlCommand(
                    "UPDATE products SET coupon_id = NULL WHERE coupon_id = @couponId",
                    connect.getConnecttion(),
                    transaction);
                clearCmd.Parameters.AddWithValue("@couponId", couponId);
                clearCmd.ExecuteNonQuery();

                // Sau đó, thêm coupon cho các sản phẩm được chọn
                foreach (var productId in productIds)
                {
                    using var cmd = new SqlCommand(
                        "UPDATE products SET coupon_id = @couponId WHERE id = @productId",
                        connect.getConnecttion(),
                        transaction);

                    cmd.Parameters.AddWithValue("@couponId", couponId);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                connect.closeConnection();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                connect.closeConnection();
                return false;
            }
        }

    }
}
