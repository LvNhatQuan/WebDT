using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class MenuDAL
    {
        private readonly DbConnect connect = new DbConnect();

        public List<MenuItem> GetAllMenu()
        {
            connect.openConnection();
            var list = new List<MenuItem>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"SELECT * FROM menu ORDER BY menuIndex";
                command.CommandText = query;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var menuItem = new MenuItem
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Title = reader["Title"]?.ToString() ?? string.Empty,
                            ParentId = reader["ParentId"] != DBNull.Value
                                            ? Convert.ToInt32(reader["ParentId"])
                                            : (int?)null,
                            MenuUrl = reader["MenuUrl"]?.ToString(),
                            MenuIndex = Convert.ToInt32(reader["MenuIndex"]),
                            isVisible = Convert.ToInt32(reader["isVisible"]) == 1
                        };

                        list.Add(menuItem);
                    }
                }
            }

            connect.closeConnection();
            return list;
        }
    }
}
