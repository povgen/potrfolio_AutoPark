using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    static class User
    {

        public static int Id { get; private set; } = -1;
        public static string Password { get; private set; }
        public static string Login { get; private set; }
        public static string Name { get; private set; }
        public static string LastName { get; private set; }
        public static string Adress { get; private set; }
        public static string Phone { get; private set; }

        public static bool Auth(string login, string pass)
        {
            SqlCommand command = new SqlCommand("Select * FROM \"User\" WHERE Login = '" + login +"'", Connection.Connect);
            try
            {
                Connection.Connect.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader.GetString(2).Trim() == pass)
                {
                    Id = reader.GetInt32(0);
                    Login = login;
                    Password = reader.GetString(2).Trim();
                    Name = reader.GetString(3).Trim();
                    LastName = reader.GetString(4).Trim();
                    Adress = reader.GetString(5).Trim();
                    Phone = reader.GetString(6).Trim();
                    return true;
                }
                MessageBox.Show("Неверный пароль");
            } catch(Exception ex)
            {
                MessageBox.Show("Пользователь с таким логином не найден");
            }
            finally
            {
                Connection.Connect.Close();
            }


            return false;
        }

        public static void Update(string login, string pass, string name, string lastName, string adress, string phone)
        {
            SqlCommand cm2 = new SqlCommand("SELECT * FROM \"User\" WHERE Login = '" + login + "'", Connection.Connect);

            SqlCommand cm = new SqlCommand(String.Format("UPDATE \"User\" SET Login = N'{0}', Password = N'{1}', Name = N'{2}', LastName = N'{3}', Adress = N'{4}', Phone = N'{5}' WHERE Id = {6}", 
                login, pass, name, lastName, adress, phone, User.Id), Connection.Connect);

            Login = login;
            Password = pass;
            Name = name;
            LastName = lastName;
            Adress = adress;
            Phone = phone;
            try
            {
                Connection.Connect.Open();
                if (cm2.ExecuteScalar() == null || cm2.ExecuteScalar().ToString() == User.Id.ToString())
                {
                    cm.ExecuteNonQuery();
                    MessageBox.Show("Данные успешно изменены");
                }
                else
                    MessageBox.Show("Пользователь с таким логином уже существует");
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            } finally
            {
                Connection.Connect.Close();
            }
        }

        public static void Reg(string login, string pass, string name, string lastName, string adress, string phone)
        {
            SqlCommand cm2 = new SqlCommand("SELECT * FROM \"User\" WHERE Login = '" + login + "'", Connection.Connect);

            SqlCommand cm = new SqlCommand(String.Format("INSERT INTO \"User\" VALUES(N'{0}', N'{1}', N'{2}', N'{3}', N'{4}', N'{5}')",
                login, pass, name, lastName, adress, phone), Connection.Connect);

            try
            {
                Connection.Connect.Open();
                if (cm2.ExecuteScalar() == null)
                {
                    cm.ExecuteNonQuery();
                    MessageBox.Show("Вы успешно зарегестрированы");
                }
                else
                    MessageBox.Show("Пользователь с таким логином уже существует");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Connect.Close();
            }
        }
        public static void LogOut()
        {
            Id = -1;
            Login = "";
            Password = "";
            Name = "";
            LastName = "";
            Adress = "";
            Phone = "";
        }
    }
}
