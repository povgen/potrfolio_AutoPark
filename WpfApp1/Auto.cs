using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Windows;
using System.Data;
using System.IO;

namespace WpfApp1
{
    class Auto
    {
        public int Id              { get; }
        public Type Type           { get; set; }
        public Model Model         { get; set; }
        public int? UserId         { get; set; }
        public DateTime? ExpDate   { get; set; }
        public DateTime Release    { get; set; }
        public int Potencia        { get; set; }
        public BitmapImage Image   { get; set; }

        Auto(int id, Type type, Model model, int? userId, DateTime? expDate, DateTime release, int potencia, byte[] image)
        {
            this.Id = id;
            this.Type = type;
            this.Model = model;
            this.UserId = userId;
            this.ExpDate = expDate;
            this.Release = release;
            this.Potencia = potencia;
            this.Image = new BitmapImage();
            this.Image.BeginInit();
            this.Image.StreamSource = new System.IO.MemoryStream(image);
            this.Image.EndInit();
            this.Image.Freeze();
        }


        Auto(int id, Type type, Model model, DateTime release, int potencia, byte[] image)
        {
            this.Id = id;
            this.Type = type;
            this.Model = model;
            this.Release = release;
            this.Image = new BitmapImage();
            this.Potencia = potencia;
            this.Image.BeginInit();
            this.Image.StreamSource = new System.IO.MemoryStream(image);
            this.Image.EndInit();
            this.Image.Freeze();
        }

        public static Auto SaveToDb(Type type, Model model, DateTime release, int potencia, byte[] imageData)
        {
            SqlCommand command = new SqlCommand()
            {
                Connection = Connection.Connect,
                CommandText = "INSERT INTO Auto VALUES (@TypeId, @ModelId, Null, Null, @Release, @Potencia, @Image)"
            };
            command.Parameters.AddWithValue("@TypeId", type.Id);
            command.Parameters.AddWithValue("@ModelId", model.Id);
            command.Parameters.AddWithValue("@Release", release);
            command.Parameters.AddWithValue("@Potencia", potencia);
            command.Parameters.AddWithValue("@Image", imageData);

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


            return new Auto(GetLastId(), type, model, release, potencia, imageData);
        }

        public static List<Auto> LoadFromDb()
        {
            CheckRentPeriod();
            List<Auto> auto = new List<Auto>();

            SqlCommand command = new SqlCommand("SELECT Auto.Id, Auto.TypeId, Type.Name, Auto.ModelId, Model.Name, Model.BrandId, Brand.Name, Auto.UserId, Auto.ExpDate, Auto.Release, Auto.Potencia, Auto.Image FROM Auto, Type, Model, Brand WHERE Auto.TypeId = Type.Id AND Auto.ModelId = Model.Id AND Model.BrandId = Brand.Id", Connection.Connect);
            Connection.Connect.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                object sqlUserId = reader[7];
                int? userId = (sqlUserId == System.DBNull.Value)
                    ? (int?)null
                    : Convert.ToInt32(sqlUserId);

                object sqlExpDate = reader[8];
                DateTime? expDate = (sqlExpDate == System.DBNull.Value)
                    ? (DateTime?)null
                    : Convert.ToDateTime(sqlExpDate);

                auto.Add(new Auto(
                   reader.GetInt32(0),                 //Id            | int
                   Type.GetType(reader.GetInt32(1),    //TypeId        | int
                   reader.GetString(2)),               //Type.Name     | string
                   Model.GetModel(reader.GetInt32(3),  //ModelId       | int
                   reader.GetString(4),                //Model.Name    | string
                   reader.GetInt32(5),                 //Model.BrandId | int
                   reader.GetString(6)),               //Brand.Name    | string
                   userId,                             //UserId        | int
                   expDate,                            //ExpDate       | date
                   reader.GetDateTime(9),              //Release       | date
                   reader.GetInt32(10),                //Potencia      | int
                   (byte[])reader.GetValue(11)         //Image         | binary(1024)
               ));
            }
            reader.Close();
            Connection.Connect.Close();

            return auto;
        }

        static void CheckRentPeriod()
        {
            DateTime now = DateTime.Now;
            SqlCommand cm = new SqlCommand("UPDATE Auto SET UserId = null, ExpDate = null WHERE ExpDate < GETDATE ()", Connection.Connect);
            Connection.Connect.Open();
            cm.ExecuteNonQuery();
            Connection.Connect.Close();
        }

        public void Update(Type type, Model model, DateTime release, int potencia, byte[] imageData)
        {
            SqlCommand command = new SqlCommand("UPDATE Auto SET TypeId = @TypeId, ModelId = @ModelId, Release = @Release, Potencia = @Potencia, Image = @Image WHERE Id = @Id", Connection.Connect);
            command.Parameters.AddWithValue("@TypeId", type.Id);
            command.Parameters.AddWithValue("@ModelId", model.Id);
            command.Parameters.AddWithValue("@Release", release);
            command.Parameters.AddWithValue("@Potencia", potencia);
            command.Parameters.AddWithValue("@Image", imageData);
            command.Parameters.AddWithValue("@Id", this.Id);
            try
            {
                Connection.Connect.Open();
                command.ExecuteNonQuery();
                Connection.Connect.Close();
                Type = type;
                Model = model;
                Release = release;
                Potencia = potencia;
                Image = new BitmapImage();
                Image.BeginInit();
                Image.StreamSource = new System.IO.MemoryStream(imageData);
                Image.EndInit();
                Image.Freeze();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Connection.Connect.Close();
            }
        }

        public void Delete()
        {
            SqlCommand command = new SqlCommand("DELETE FROM Auto WHERE Id = @Id", Connection.Connect);
            command.Parameters.AddWithValue("Id", Id);
            try
            {
                Connection.Connect.Open();
                command.ExecuteNonQuery();
                Connection.Connect.Close();
            } catch (Exception ex)
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
                CommandText = "SELECT TOP 1 id FROM Auto ORDER BY ID DESC"
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
