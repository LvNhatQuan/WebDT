using Microsoft.Data.SqlClient;
using WebDT.Database;
using WebDT.Models;

namespace WebDT.DAL
{
    public class AddressDAL
    {
        DbConnect connect = new DbConnect();

        // Lấy danh sách địa chỉ theo UserId
        public List<Address> GetAddressesByUserId(int userId)
        {
            connect.openConnection();
            var addresses = new List<Address>();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"
                    SELECT * FROM addresses 
                    WHERE user_id = @userId 
                    ORDER BY is_default DESC, id DESC";
                command.Parameters.AddWithValue("@userId", userId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        addresses.Add(new Address
                        {
                            id = Convert.ToInt32(reader["id"]),
                            user_id = Convert.ToInt32(reader["user_id"]),
                            address_line = reader["address_line"].ToString(),
                            city = reader["city"]?.ToString() ?? "",
                            district = reader["district"]?.ToString() ?? "",
                            phone_receiver = reader["phone_receiver"]?.ToString() ?? "",
                            is_default = Convert.ToBoolean(reader["is_default"])
                        });
                    }
                }
            }

            connect.closeConnection();
            return addresses;
        }

        // Lấy địa chỉ theo Id
        public Address? GetAddressById(int id)
        {
            connect.openConnection();
            Address? address = null;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT * FROM addresses WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        address = new Address
                        {
                            id = Convert.ToInt32(reader["id"]),
                            user_id = Convert.ToInt32(reader["user_id"]),
                            address_line = reader["address_line"].ToString(),
                            city = reader["city"]?.ToString() ?? "",
                            district = reader["district"]?.ToString() ?? "",
                            phone_receiver = reader["phone_receiver"]?.ToString() ?? "",
                            is_default = Convert.ToBoolean(reader["is_default"])
                        };
                    }
                }
            }

            connect.closeConnection();
            return address;
        }

        // Thêm địa chỉ mới
        public bool AddAddress(Address address)
        {
            connect.openConnection();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = @"
                    INSERT INTO addresses (user_id, address_line, city, district, 
                    phone_receiver, is_default) 
                    VALUES (@user_id, @address_line, @city, @district, 
                    @phone_receiver, @is_default)";

                command.Parameters.AddWithValue("@user_id", address.user_id);
                command.Parameters.AddWithValue("@address_line", address.address_line);
                command.Parameters.AddWithValue("@city", address.city ?? "");
                command.Parameters.AddWithValue("@district", address.district ?? "");
                command.Parameters.AddWithValue("@phone_receiver", address.phone_receiver ?? "");
                command.Parameters.AddWithValue("@is_default", address.is_default);

                int numberOfRows = command.ExecuteNonQuery();
                connect.closeConnection();
                return numberOfRows > 0;
            }
        }

        // Cập nhật địa chỉ
        public bool UpdateAddress(Address address)
        {
            connect.openConnection();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = @"
                    UPDATE addresses 
                    SET address_line = @address_line, 
                        city = @city, 
                        district = @district, 
                        phone_receiver = @phone_receiver,
                        is_default = @is_default
                    WHERE id = @id AND user_id = @user_id";

                command.Parameters.AddWithValue("@id", address.id);
                command.Parameters.AddWithValue("@user_id", address.user_id);
                command.Parameters.AddWithValue("@address_line", address.address_line);
                command.Parameters.AddWithValue("@city", address.city ?? "");
                command.Parameters.AddWithValue("@district", address.district ?? "");
                command.Parameters.AddWithValue("@phone_receiver", address.phone_receiver ?? "");
                command.Parameters.AddWithValue("@is_default", address.is_default);

                int numberOfRows = command.ExecuteNonQuery();
                connect.closeConnection();
                return numberOfRows > 0;
            }
        }

        // Xóa địa chỉ
        public bool DeleteAddress(int id)
        {
            connect.openConnection();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = "DELETE FROM addresses WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);

                int numberOfRows = command.ExecuteNonQuery();
                connect.closeConnection();
                return numberOfRows > 0;
            }
        }

        // Đặt địa chỉ làm mặc định
        public bool SetDefaultAddress(int userId, int addressId)
        {
            connect.openConnection();

            using (SqlTransaction transaction = connect.getConnecttion().BeginTransaction())
            {
                try
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connect.getConnecttion();
                        command.Transaction = transaction;

                        // Bỏ mặc định tất cả địa chỉ của user
                        command.CommandText = "UPDATE addresses SET is_default = 0 WHERE user_id = @user_id";
                        command.Parameters.AddWithValue("@user_id", userId);
                        command.ExecuteNonQuery();

                        // Đặt địa chỉ mới làm mặc định
                        command.Parameters.Clear();
                        command.CommandText = "UPDATE addresses SET is_default = 1 WHERE id = @id AND user_id = @user_id";
                        command.Parameters.AddWithValue("@id", addressId);
                        command.Parameters.AddWithValue("@user_id", userId);
                        int numberOfRows = command.ExecuteNonQuery();

                        transaction.Commit();
                        connect.closeConnection();
                        return numberOfRows > 0;
                    }
                }
                catch
                {
                    transaction.Rollback();
                    connect.closeConnection();
                    throw;
                }
            }
        }

        // Lấy địa chỉ mặc định của user
        public Address? GetDefaultAddress(int userId)
        {
            connect.openConnection();
            Address? address = null;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"
                    SELECT TOP 1 * FROM addresses 
                    WHERE user_id = @user_id AND is_default = 1";
                command.Parameters.AddWithValue("@user_id", userId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        address = new Address
                        {
                            id = Convert.ToInt32(reader["id"]),
                            user_id = Convert.ToInt32(reader["user_id"]),
                            address_line = reader["address_line"].ToString(),
                            city = reader["city"]?.ToString() ?? "",
                            district = reader["district"]?.ToString() ?? "",
                            phone_receiver = reader["phone_receiver"]?.ToString() ?? "",
                            is_default = true
                        };
                    }
                }
            }

            connect.closeConnection();
            return address;
        }

        // Lấy địa chỉ đầy đủ (cho checkout)
        public string GetDefaultAddressString(int userId)
        {
            connect.openConnection();
            string result = "Chưa có địa chỉ giao hàng";

            // Đầu tiên kiểm tra trong bảng addresses
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"
                    SELECT TOP 1 address_line, city, district, phone_receiver 
                    FROM addresses 
                    WHERE user_id = @user_id AND is_default = 1";
                command.Parameters.AddWithValue("@user_id", userId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string addressLine = reader["address_line"].ToString() ?? "";
                        string city = reader["city"]?.ToString() ?? "";
                        string district = reader["district"]?.ToString() ?? "";
                        string phone = reader["phone_receiver"]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(addressLine))
                        {
                            result = $"{addressLine}, {district}, {city}. ĐT: {phone}";
                        }
                    }
                }
            }

            // Nếu không có trong addresses, kiểm tra trong bảng users
            if (result == "Chưa có địa chỉ giao hàng")
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connect.getConnecttion();
                    command.CommandType = System.Data.CommandType.Text;

                    command.CommandText = "SELECT address FROM users WHERE id = @user_id";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@user_id", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string address = reader["address"]?.ToString();
                            if (!string.IsNullOrEmpty(address))
                            {
                                result = address;
                            }
                        }
                    }
                }
            }

            connect.closeConnection();
            return result;
        }
    }
}