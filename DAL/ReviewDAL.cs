using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class ReviewDAL
    {
        private readonly DbConnect _db = new DbConnect();

        // =============================
        // GET REVIEWS BY PRODUCT
        // =============================
        public List<Review> GetByProductId(int productId)
        {
            var list = new List<Review>();

            string sql = @"
SELECT
    r.id,
    r.product_id,
    r.user_id,
    u.username,
    r.rating,
    r.comment,
    r.created_at
FROM reviews r
JOIN users u ON r.user_id = u.id
WHERE r.product_id = @pid
ORDER BY r.created_at DESC
";

            _db.openConnection();
            using var cmd = new SqlCommand(sql, _db.getConnecttion());
            cmd.Parameters.AddWithValue("@pid", productId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Review
                {
                    Id = reader.GetInt32(0),
                    ProductId = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    UserName = reader.GetString(3),
                    Rating = reader.GetByte(4),
                    Comment = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    CreatedAt = reader.IsDBNull(6) ? DateTime.Now : reader.GetDateTime(6)
                });
            }

            _db.closeConnection();
            return list;
        }

        // =============================
        // COUNT REVIEW
        // =============================
        public int CountByProductId(int productId)
        {
            string sql = "SELECT COUNT(*) FROM reviews WHERE product_id = @pid";

            _db.openConnection();
            using var cmd = new SqlCommand(sql, _db.getConnecttion());
            cmd.Parameters.AddWithValue("@pid", productId);

            int count = (int)cmd.ExecuteScalar();
            _db.closeConnection();
            return count;
        }

        // =============================
        // AVG RATING
        // =============================
        public double AvgRatingByProductId(int productId)
        {
            string sql = @"
SELECT ISNULL(AVG(CAST(rating AS FLOAT)), 0)
FROM reviews
WHERE product_id = @pid
";

            _db.openConnection();
            using var cmd = new SqlCommand(sql, _db.getConnecttion());
            cmd.Parameters.AddWithValue("@pid", productId);

            double avg = Convert.ToDouble(cmd.ExecuteScalar());
            _db.closeConnection();
            return avg;
        }
    }
}
