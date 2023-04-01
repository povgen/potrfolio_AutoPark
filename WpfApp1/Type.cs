﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    class Type
    {
        public string Name;
        public int Id { get;  }

        Type(int id, string name)
        {
            Name = name;
            Id = id;
        }

        public static Type GetType(int id, string name)
        {
            return new Type(id, name);
        }

        public static Type SaveToDb(string typeName)
        {
            SqlCommand command = new SqlCommand()
            {
                Connection = Connection.Connect,
                CommandText = "INSERT INTO Type VALUES (@Name)"
            };
            command.Parameters.Add("@Name", SqlDbType.NVarChar, 20);
            command.Parameters["@Name"].Value = typeName;
            
            try
            {
                Connection.Connect.Open();
                command.ExecuteNonQuery();
                Connection.Connect.Close();
                MessageBox.Show("Запись добавлена");
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Connection.Connect.Close();
            }
            

            return new Type(GetLastId(), typeName);
        }

        public static List<Type> LoadFromDb()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM Type", Connection.Connect);

            List<Type> types = new List<Type>();

            try
            {
                Connection.Connect.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    types.Add(new Type(
                            reader.GetInt32(0), // Id   | int
                            reader.GetString(1) // Name | string
                        ));

                Connection.Connect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Connection.Connect.Close();
            }

            return types;
        }

        public void Update(string newName)
        {
            this.Name = newName;
            SqlCommand command = new SqlCommand()
            {
                Connection = Connection.Connect,
                CommandText = "UPDATE Type SET Name = @Name WHERE Id = @Id"
            };
            command.Parameters.AddWithValue("@Name", this.Name); 
            command.Parameters.AddWithValue("@Id", this.Id);

            try
            {
                Connection.Connect.Open();
                command.ExecuteNonQuery();
                Connection.Connect.Close();
                MessageBox.Show("Запись обновлена");
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Connection.Connect.Close();
            }
            
        } 

        public void Delete()
        {
            SqlCommand command = new SqlCommand("DELETE FROM Type WHERE Id = @Id", Connection.Connect);
            try
            {
                Connection.Connect.Open();
                command.Parameters.AddWithValue("@Id", this.Id);
                command.ExecuteNonQuery();
                Connection.Connect.Close();
                MessageBox.Show("Запись Удалена");
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
                CommandText = "SELECT TOP 1 id FROM Type ORDER BY ID DESC"
            };
            try
            {
                Connection.Connect.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                int id = reader.GetInt32(0);
                Connection.Connect.Close();
                return id;
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Connection.Connect.Close();
                return -1;
            }
        }
    }
}
