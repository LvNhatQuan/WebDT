using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Areas.Admin.Models;

namespace WebDT.Areas.Admin.DAL
{
    public class ReviewAdminDAL
    {
        DbConnect db = new DbConnect();

        // ===============================
        // DANH SÁCH ĐÁNH GIÁ
        // ===============================
        public List<ReviewAdmin> GetAll()
        {
            var list = new List<ReviewAdmin>();

            string sql = @"
SELECT r.id, r.rating, r.comment, r.created_at,
       p.name AS product_name,
       u.full_name AS customer_name
FROM reviews r
JOIN products p ON r.product_id = p.id
JOIN users u ON r.user_id = u.id
ORDER BY r.created_at DESC";

            using SqlConnection con =
                new SqlConnection(db.getConnecttion().ConnectionString);
            using SqlCommand cmd = new SqlCommand(sql, con);

            con.Open();
            using SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                list.Add(new ReviewAdmin
                {
                    Id = (int)rd["id"],
                    ProductName = rd["product_name"].ToString()!,
                    CustomerName = rd["customer_name"].ToString()!,
                    Rating = Convert.ToInt32(rd["rating"]),
                    Comment = rd["comment"]?.ToString(),
                    CreatedAt = Convert.ToDateTime(rd["created_at"])
                });
            }

            return list;
        }

        // ===============================
        // XÓA ĐÁNH GIÁ
        // ===============================
        public bool Delete(int id)
        {
            string sql = "DELETE FROM reviews WHERE id = @id";

            using SqlConnection con =
                new SqlConnection(db.getConnecttion().ConnectionString);
            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@id", id);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

    }
}
