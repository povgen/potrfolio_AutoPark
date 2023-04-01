using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    class Operation
    {
        public int UserId { get; private set; }
        public int Money { get; private set; }
        public DateTime Date { get; private set; }

        public static void ToRent(int userId, int autoId, int money, int period)
        {
            DateTime now = DateTime.Now;
            DateTime expDate = now.AddDays(period);

            SqlCommand insertOperation = new SqlCommand("INSERT INTO Operation VALUES(@UserId, @Money, @Date)", Connection.Connect);
            insertOperation.Parameters.AddWithValue("@UserId", userId);
            insertOperation.Parameters.AddWithValue("@Money", money);
            insertOperation.Parameters.AddWithValue("@Date", now);

            SqlCommand updateAuto = new SqlCommand("UPDATE Auto SET UserId = @UserId, ExpDate = @ExpDate Where Id = @Id",Connection.Connect);
            updateAuto.Parameters.AddWithValue("@UserId", userId);
            updateAuto.Parameters.AddWithValue("@ExpDate", expDate);
            updateAuto.Parameters.AddWithValue("@Id", autoId);

            SqlCommand getUserId = new SqlCommand("SELECT Auto.UserId FROM Auto WHERE Id = @Id", Connection.Connect);
            getUserId.Parameters.AddWithValue("@Id", autoId);

            try
            {
                Connection.Connect.Open();
                if (getUserId.ExecuteScalar().ToString() == "")
                {
                    insertOperation.ExecuteNonQuery();
                    updateAuto.ExecuteNonQuery();
                    MessageBox.Show("Машина арендована");
                }
                else
                    MessageBox.Show("Машина уже занята");
                
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } finally
            {
                Connection.Connect.Close();
            }
        }

        public static void UpdateRentPeriod(int userId, int autoId, int money, int period)
        {
            DateTime now = DateTime.Now;

            SqlCommand insertOperation = new SqlCommand("INSERT INTO Operation VALUES(@UserId, @Money, @Date)", Connection.Connect);
            insertOperation.Parameters.AddWithValue("@UserId", userId);
            insertOperation.Parameters.AddWithValue("@Money", money);
            insertOperation.Parameters.AddWithValue("@Date", now);

            SqlCommand updateAuto = new SqlCommand("UPDATE Auto SET UserId = @UserId, ExpDate = @ExpDate Where Id = @Id", Connection.Connect);
            updateAuto.Parameters.AddWithValue("@UserId", userId);
            updateAuto.Parameters.AddWithValue("@Id", autoId);

            SqlCommand getExpDate = new SqlCommand("SELECT Auto.ExpDate FROM Auto WHERE Id = @Id", Connection.Connect);
            getExpDate.Parameters.AddWithValue("@Id", autoId);

            try
            {
                Connection.Connect.Open();
                DateTime expDate = (DateTime)getExpDate.ExecuteScalar();
                expDate = expDate.AddDays(period);
                updateAuto.Parameters.AddWithValue("@ExpDate", expDate);
                updateAuto.ExecuteNonQuery();
                insertOperation.ExecuteNonQuery();
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


        public static List<UIElement> Get(int userId)
        {
            var operations = new List<UIElement>();
            DockPanel dp1 = new DockPanel()
            {
                Margin = new Thickness(10),
                Height = 30
            };
            Label money1 = new Label()
            {
                FontSize = 15,
                FontWeight = FontWeight.FromOpenTypeWeight(500),
                Content = "Сумма (р)"
            };
            dp1.Children.Add(money1);
            Label date1 = new Label()
            {
                FontSize = 15,
                FontWeight = FontWeight.FromOpenTypeWeight(500),
                Content = "Дата"
            };
            dp1.Children.Add(date1);
            operations.Add(dp1);

            SqlCommand comm = new SqlCommand("SELECT * FROM Operation WHERE UserId = " + userId, Connection.Connect);
            try
            {
                Connection.Connect.Open();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                  
                    DockPanel dp = new DockPanel()
                    {
                        Margin = new Thickness(10),
                        Height = 30
                    };
                    Label money = new Label()
                    {
                        FontSize = 15,
                        FontWeight = FontWeight.FromOpenTypeWeight(500),
                        Content = reader.GetInt32(2) + "    "
                    };
                    dp.Children.Add(money);
                    Label date = new Label()
                    {
                        FontSize = 15,
                        FontWeight = FontWeight.FromOpenTypeWeight(500),
                        Content = reader.GetDateTime(3).ToString("dd:MM:yyyy")
                    };
                    dp.Children.Add(date);
                    operations.Add(dp);
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            } finally
            {
                Connection.Connect.Close();
            }

            return operations;
        } 

        static public void RemoveFromMyCar(int Id)
        {
            SqlCommand cm = new SqlCommand("UPDATE Auto SET UserId = null, ExpDate = null WHERE Id = " + Id, Connection.Connect);
            Connection.Connect.Open();
            cm.ExecuteNonQuery();
            Connection.Connect.Close();
        }
        static public void RemoveFromMyCarFormShow(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            RemoveFromMyCar f = new RemoveFromMyCar((Int32) btn.Tag);
            f.Show();

        }
        static public void UpdatePeriodFormShow(object sender, RoutedEventArgs e)
        {

            var btn = sender as Button;
            UpdatePeriod f = new UpdatePeriod(Int32.Parse(btn.Tag.ToString()));
            f.Show();
        }
    }
}
