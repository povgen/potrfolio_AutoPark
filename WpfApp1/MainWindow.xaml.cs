using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Brand> brands = Brand.LoadFromDb();
        public MainWindow()
        {
            InitializeComponent();
            var Adverts = Advert.GetCars(Auto.LoadFromDb());
            foreach (UIElement advert in Adverts)
                stackPanel.Children.Add(advert);

            foreach (var item in brands)
                filter.Items.Add(item.Name);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (User.Id == -1)
            {
                if (User.Auth(login.Text, pass.Password))//"11198151", "3QjGHM")) 
                {
                    btnAllAuto.IsEnabled = true;
                    btnMyAuto.IsEnabled = true;
                    btnSendReq.IsEnabled = true;
                    btnHistory.IsEnabled = true;
                    btnSetting.IsEnabled = true;
                    filter.IsEnabled = true;
                    signButton.Content = "Выйти";
                }
            }
            else
            {
                signButton.Content = "Войти";
                btnAllAuto.IsEnabled = false;
                btnMyAuto.IsEnabled = false;
                btnSendReq.IsEnabled = false;
                btnHistory.IsEnabled = false;
                btnSetting.IsEnabled = false;
                filter.IsEnabled = false;
                login.Clear();
                pass.Clear();
                User.LogOut();
            }


        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AdvertEditor form = new AdvertEditor();
            form.Show();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filter.SelectedIndex != -1)
            {
                stackPanel.Children.Clear();
                var adverts = Advert.GetCarsWithFilter(Auto.LoadFromDb(), brands[filter.SelectedIndex]);
                foreach (var item in adverts)
                    stackPanel.Children.Add(item);

            }

        }

        private void BtnAllAuto_Click(object sender, RoutedEventArgs e)
        {
            var Adverts = Advert.GetCars(Auto.LoadFromDb());
            stackPanel.Children.Clear();
            foreach (UIElement advert in Adverts)
                stackPanel.Children.Add(advert);
        }

        private void BtnMyAuto_Click(object sender, RoutedEventArgs e)
        {
            stackPanel.Children.Clear();
            var adverts = Advert.GetMyCars(Auto.LoadFromDb());
            foreach (var item in adverts)
                stackPanel.Children.Add(item);
        }

        private void BtnSendReq_Click(object sender, RoutedEventArgs e)
        {
            SendRequest form = new SendRequest();
            form.Show();
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            History f = new History();
            f.Show();
        }

        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            Settings f = new Settings();
            f.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Reg f = new Reg();
            f.Show();
        }
    }
}
