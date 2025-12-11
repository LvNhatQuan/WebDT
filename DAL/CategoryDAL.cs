using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class CategoryDAL
    {
        private readonly DbConnect connect = new DbConnect();

        public List<CategoryMenu> GetAllWithCount()
        {
            connect.openConnection();
            var list = new List<CategoryMenu>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"
                    SELECT 
                        c.id,
                        c.name,
                        c.description,
                        COUNT(p.id) AS TotalProduct
                    FROM categories c
                    LEFT JOIN products p ON c.id = p.category_id
                    GROUP BY c.id, c.name, c.description
                    ORDER BY TotalProduct DESC";

                command.CommandText = query;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var category = new CategoryMenu
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["name"]?.ToString() ?? string.Empty,
                            Description = reader["description"]?.ToString() ?? string.Empty,
                            Count = Convert.ToInt32(reader["TotalProduct"])
                        };

                        list.Add(category);
                    }
                }
            }

            connect.closeConnection();
            return list;
        }
    }
}
