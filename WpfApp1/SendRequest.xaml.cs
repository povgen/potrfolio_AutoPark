using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для SenddRequest.xaml
    /// </summary>
    public partial class SendRequest : Window
    {
        List<Brand> brands = Brand.LoadFromDb(); 
        List<Model> models = Model.LoadFromDb(); 
        List<Auto> autous = Auto.LoadFromDb(); 
        public SendRequest()
        {
            InitializeComponent();
            foreach (var item in brands)
                Brands.Items.Add(item.Name);

            Brands.Items.Add("Все");

            foreach (var item in models)
                Models.Items.Add(item.Name);
        }

        private void Period_TextChanged(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void Brands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Model> modelsWithFilter = new List<Model>();
            if (Brands.SelectedIndex != -1)
            {
                Models.Items.Clear();
                if (Brands.SelectedIndex == Brands.Items.Count - 1)
                    foreach (var item in models)
                        Models.Items.Add(item.Name);
                else
                    foreach (var item in models)
                        if (item.Brand.Id == brands[Brands.SelectedIndex].Id)
                            Models.Items.Add(item.Name);
            }
        }

        private void Period_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Int32.Parse(period.Text) <= 5)
                    Cost.Text = (Int32.Parse(period.Text) * 2000).ToString();
                else
                    Cost.Text = (((Int32.Parse(period.Text) - 5) * 1000) + 5 * 2000).ToString();
            } catch
            {
                Cost.Text = "";    }
                
        }

        private void Models_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Models.SelectedIndex != -1)
            {
                Autos.Items.Clear();
                foreach (var item in autous)
                    if (item.Model.Name == Models.SelectedItem.ToString())
                        Autos.Items.Add(item.Id);
            }
                
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Autos.SelectedIndex != -1 && period.Text !="")
            {
                Operation.ToRent(User.Id, Int32.Parse(Autos.SelectedItem.ToString()), Int32.Parse(Cost.Text), Int32.Parse(period.Text));
                Close();
            }
            else
                MessageBox.Show("Выберите машину и ввведите срок аренды");
        }
    }
}
