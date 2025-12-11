using Microsoft.Data.SqlClient;
using WebDT.Database;

namespace WebDT.DAL
{
    public class CouponDAL
    {
        private readonly DbConnect db = new DbConnect();

        public decimal Apply(int couponId, decimal subtotal)
        {
            db.openConnection();

            using var cmd = new SqlCommand(
                "SELECT discount_value FROM coupons WHERE id=@id AND is_active=1",
                db.getConnecttion());

            cmd.Parameters.AddWithValue("@id", couponId);

            var result = cmd.ExecuteScalar();
            db.closeConnection();

            if (result == null) return 0;

            decimal percent = Convert.ToDecimal(result);
            return subtotal * percent / 100m;
        }
    }
}
