using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    class Brand
    {
        public string Name;
        public int Id { get; }

        Brand(int id, string name)
        {
            Name = name;
            Id = id;
        }

        public static Brand SaveToDb(string brandName)
        {
            SqlCommand command = new SqlCommand()
            {
                Connection = Connection.Connect,
                CommandText = "INSERT INTO Brand VALUES (@Name)"
            };
            command.Parameters.Add("@Name", SqlDbType.NVarChar, 20);
            command.Parameters["@Name"].Value = brandName;

            try
            {
                Connection.Connect.Open();
                command.ExecuteNonQuery();
                Connection.Connect.Close();
                MessageBox.Show("Запись добавлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            return new Brand(GetLastId(), brandName);
        }

        public static List<Brand> LoadFromDb()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM Brand", Connection.Connect);

            List<Brand> brands = new List<Brand>();

            try
            {
                Connection.Connect.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    brands.Add(new Brand(
                            reader.GetInt32(0), // Id   | int
                            reader.GetString(1) // Name | string
                        ));

                Connection.Connect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return brands;
        }

        public static Brand GetBrand(int id, string name)
        {
                return new Brand(id, name);
        }

        public void Update(string newName)
        {
            this.Name = newName;
            SqlCommand command = new SqlCommand()
            {
                Connection = Connection.Connect,
                CommandText = "UPDATE Brand SET Name = @Name WHERE Id = @Id"
            };
            command.Parameters.AddWithValue("@Name", this.Name);
            command.Parameters.AddWithValue("@Id", this.Id);

            try
            {
                Connection.Connect.Open();
                command.ExecuteNonQuery();
                Connection.Connect.Close();
                MessageBox.Show("Запись обновлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void Delete()
        {
            SqlCommand command = new SqlCommand("DELETE FROM Brand WHERE Id = @Id", Connection.Connect);
            try
            {
                Connection.Connect.Open();
                command.Parameters.AddWithValue("@Id", this.Id);
                command.ExecuteNonQuery();
                Connection.Connect.Close();
                MessageBox.Show("Запись Удалена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static int GetLastId()
        {
            SqlCommand command = new SqlCommand()
            {
                Connection = Connection.Connect,
                CommandText = "SELECT TOP 1 id FROM Brand ORDER BY ID DESC"
            };
            try
            {
                Connection.Connect.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                int id = reader.GetInt32(0);
                Connection.Connect.Close();
                return id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return -1;
            }
        }
    }
}
