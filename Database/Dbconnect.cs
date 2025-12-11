using Microsoft.Data.SqlClient;

namespace WebDT.Database
{
    public class DbConnect
    {
        // Kết nối SQL Server (giữ nguyên đúng connection string của bạn)
        private readonly SqlConnection _connection =
            new SqlConnection("Data Source=NhatQuan\\SQLEXPRESS;Initial Catalog=DT;Integrated Security=True;TrustServerCertificate=False;Encrypt=false");

        // Lấy connection
        public SqlConnection getConnecttion()
        {
            return _connection;
        }

        // Mở kết nối
        public void openConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        // Đóng kết nối
        public void closeConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }
    }
}
