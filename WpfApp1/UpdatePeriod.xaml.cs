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
    /// Логика взаимодействия для UpdatePeriod.xaml
    /// </summary>
    public partial class UpdatePeriod : Window
    {
        int IdAuto;
        public UpdatePeriod(int idAuto)
        {
            InitializeComponent();
            IdAuto = idAuto;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Period.Text != "")
            {
                Operation.UpdateRentPeriod(User.Id, IdAuto, Int32.Parse(Cost.Text), Int32.Parse(Period.Text));
                MessageBox.Show("Аренда продлена");
                this.Close();
            }
            else
                MessageBox.Show("Ввведите срок аренды");
        }

        private void Period_TextChanged(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Int32.Parse(Period.Text) <= 5)
                    Cost.Text = (Int32.Parse(Period.Text) * 2000).ToString();
                else
                    Cost.Text = (((Int32.Parse(Period.Text) - 5) * 1000) + 5 * 2000).ToString();
            }
            catch
            {
                Cost.Text = "";
            }
        }
    }
}
