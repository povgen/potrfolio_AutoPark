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
    class Model
    {
        public int Id { get; }
        public string Name;
        public Brand Brand;

        Model(int id, string name, Brand brand)
        {
            Id = id;
            Name = name;
            Brand = brand;
        }

        Model(int id, string modelName, int brandId, string brandName)
        {
            Id = id;
            Name = modelName;
            
            Brand = Brand.GetBrand(brandId, brandName);
        }

        public static Model SaveToDb(string modelName, Brand brand)
        {
            SqlCommand command = new SqlCommand()
            {
                Connection = Connection.Connect,
                CommandText = "INSERT INTO Model VALUES (@Name, @BrandId)"
            };
            command.Parameters.Add("@Name", SqlDbType.NVarChar, 20);
            command.Parameters.Add("@BrandId", SqlDbType.Int);
            command.Parameters["@Name"].Value = modelName;
            command.Parameters["@BrandId"].Value = brand.Id;

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
                Connection.Connect.Close();
            }


            return new Model(GetLastId(), modelName, brand);
        }

        public static Model GetModel(int modelId, string modelName, int brandId, string brandName)
        {
            return new Model(modelId, modelName, Brand.GetBrand(brandId, brandName));
        }

        public static List<Model> LoadFromDb()
        {
            SqlCommand command = new SqlCommand("SELECT Model.Id, Model.Name, Model.BrandId, Brand.Name FROM Model, Brand WHERE Model.BrandId = Brand.Id", Connection.Connect);

            List<Model> models = new List<Model>();

            try
            {
                Connection.Connect.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    models.Add(new Model(
                            reader.GetInt32(0), // Id.Model         | int
                            reader.GetString(1),// Name.Model       | string
                            reader.GetInt32(2), // BrandId.Model    | int
                            reader.GetString(3) // Name.Brand       | string
                        ));

                Connection.Connect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Connection.Connect.Close();
            }

            return models;
        }

        public void Update(string newName, Brand newBrand)
        {
            this.Name = newName;
            SqlCommand command = new SqlCommand()
            {
                Connection = Connection.Connect,
                CommandText = "UPDATE Model SET Name = @Name, BrandId = @BrandId WHERE Id = @Id"
            };
            command.Parameters.AddWithValue("@Name", this.Name);
            command.Parameters.AddWithValue("@BrandId", newBrand.Id);
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
                Connection.Connect.Close();
            }

        }

        public void Delete()
        {
            SqlCommand command = new SqlCommand("DELETE FROM Model WHERE Id = @Id", Connection.Connect);
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
                Connection.Connect.Close();
            }
        }

        static int GetLastId()
        {
            SqlCommand command = new SqlCommand()
            {
                Connection = Connection.Connect,
                CommandText = "SELECT TOP 1 id FROM Model ORDER BY ID DESC"
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
                Connection.Connect.Close();
                return -1;
            }
        }
    }
}
