using Microsoft.Data.SqlClient;

namespace WebDT.Database
{
    public class DbConnect
    {
        SqlConnection connect = new SqlConnection(
            "Data Source=NhatQuan\\SQLEXPRESS;Initial Catalog=DT;Integrated Security=True;TrustServerCertificate=False;Encrypt=false"
        );

        // === HÀM GỐC CỦA BẠN (KHÔNG XOÁ) ===
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

        // === HÀM ALIAS MỚI THÊM VÀO CHO DAL ===
        public SqlConnection GetConnection()
        {
            return getConnecttion();
        }

        public void OpenConnection()
        {
            openConnection();
        }

        public void CloseConnection()
        {
            closeConnection();
        }
    }
}
