using Microsoft.Data.SqlClient;
using WebDT.Database;

namespace WebDT.Areas.Admin.DAL
{
    public class ProductStatisticDAL
    {
        private readonly DbConnect _db = new DbConnect();

        public List<dynamic> GetProductStatistics()
        {
            var list = new List<dynamic>();
            string sql = @"SELECT * FROM vw_product_sales";

            _db.openConnection();
            var cmd = new SqlCommand(sql, _db.getConnecttion());
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new
                {
                    ProductId = reader["product_id"],
                    ProductName = reader["product_name"].ToString(),
                    CategoryName = reader["category_name"].ToString(),
                    TotalOrders = reader["total_orders"] == DBNull.Value ? 0 : Convert.ToInt32(reader["total_orders"]),
                    TotalQuantity = reader["total_quantity_sold"] == DBNull.Value ? 0 : Convert.ToInt32(reader["total_quantity_sold"]),
                    Revenue = reader["total_revenue"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["total_revenue"]),
                    AvgRating = reader["average_rating"] == DBNull.Value ? 0 : Convert.ToDouble(reader["average_rating"])
                });
            }

            reader.Close();
            return list;
        }
    }
}
