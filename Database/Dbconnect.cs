using Microsoft.Data.SqlClient;

namespace WebDT.Database
{
    public class DbConnect
    {
        SqlConnection connect = new SqlConnection(
            "Data Source=NhatQuan\\SQLEXPRESS;Initial Catalog=DT;Integrated Security=True;TrustServerCertificate=False;Encrypt=false"
        );

        public SqlConnection getConnecttion()
        {
            return connect;
        }

        public void openConnection()
        {
            if (connect.State == System.Data.ConnectionState.Closed)
                connect.Open();
        }

        public void closeConnection()
        {
            if (connect.State == System.Data.ConnectionState.Open)
                connect.Close();
        }

        // Alias mới
        public SqlConnection GetConnection() => connect;
        public void OpenConnection() => openConnection();
        public void CloseConnection() => closeConnection();

        // ⭐ Thêm hàm này để sửa lỗi CartDAL
        public string GetConnectionString()
        {
            return connect.ConnectionString;
        }

        // Cũng có thể dùng property
        public string ConnectionString => connect.ConnectionString;
    }
}
