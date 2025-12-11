using Microsoft.Data.SqlClient;
using WebDT.Areas.Admin.Models;
using WebDT.Database;

namespace WebDT.Areas.DAL
{
    public class CategoryAdminDAL
    {
        DbConnect connect = new DbConnect();

        public List<CategoryAdmin> getAll()
        {
            connect.openConnection();

            List<CategoryAdmin> list = new List<CategoryAdmin>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"SELECT id, name, description FROM categories";

                command.CommandText = query;

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    CategoryAdmin category = new CategoryAdmin()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"]?.ToString() ?? string.Empty,
                        Description = reader["description"]?.ToString()
                    };

                    list.Add(category);
                }
            }

            connect.closeConnection();
            return list;
        }

        public bool AddNew(CategoryAdmin categoryAdd)
        {
            connect.openConnection();

            int rows = 0;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"INSERT INTO categories(name, description)
                         VALUES(@name, @description);";

                command.CommandText = query;

                command.Parameters.AddWithValue("@name", categoryAdd.Name);
                
                command.Parameters.AddWithValue("@description",
                    string.IsNullOrEmpty(categoryAdd.Description)
                        ? DBNull.Value
                        : categoryAdd.Description);

                Console.WriteLine("command Insert Category: " + command.CommandText);

                rows = command.ExecuteNonQuery();
            }

            connect.closeConnection();
            return rows > 0;
        }

        public CategoryAdmin getCategoryById(int id)
        {
            connect.openConnection();

            CategoryAdmin category = new CategoryAdmin();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"SELECT id, name, description 
                         FROM categories 
                         WHERE id = @id";

                command.CommandText = query;

                command.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    category.Id = Convert.ToInt32(reader["id"]);
                    category.Name = reader["name"]?.ToString() ?? "";
                    category.Description = reader["description"]?.ToString();
                }
            }

            connect.closeConnection();
            return category;
        }

        public bool updateCategoryById(int id, CategoryAdmin categoryUpdate)
        {
            connect.openConnection();

            int isSuccess = 0;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"
            UPDATE categories
            SET name = @name,
                description = @description
            WHERE id = @id";

                command.CommandText = query;

                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", categoryUpdate.Name);

                if (string.IsNullOrEmpty(categoryUpdate.Description))
                    command.Parameters.AddWithValue("@description", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@description", categoryUpdate.Description);

                Console.WriteLine("command update Category: " + command.CommandText);

                isSuccess = command.ExecuteNonQuery();
            }

            connect.closeConnection();
            return isSuccess > 0;
        }

        public bool deleteCategoryById(int id)
        {
            connect.openConnection();

            int isSuccess = 0;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                string query = @"DELETE FROM categories WHERE id = @id;";

                command.CommandText = query;

                command.Parameters.AddWithValue("@id", id);

                Console.WriteLine("command delete Category: " + command.CommandText);

                isSuccess = command.ExecuteNonQuery();
            }

            connect.closeConnection();
            return isSuccess > 0;
        }

    }
}
