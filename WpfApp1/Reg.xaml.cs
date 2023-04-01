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
    /// Логика взаимодействия для Reg.xaml
    /// </summary>
    public partial class Reg : Window
    {
        public Reg()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (login.Text.Trim().Length < 2 || login.Text.Trim().Length > 20)
                MessageBox.Show("Поле логин должно содержать от 2 до 20 смволов");
            else
                if (pass.Text.Trim().Length < 2 || pass.Text.Trim().Length > 20)
                MessageBox.Show("Поле пароль должно содержать от 2 до 20 смволов");
            else
                if (name.Text.Trim().Length < 2 || name.Text.Trim().Length > 20)
                MessageBox.Show("Поле Имя должно содержать от 2 до 20 смволов");
            else
                if (lName.Text.Trim().Length < 2 || lName.Text.Trim().Length > 20)
                MessageBox.Show("Поле фамилия должно содержать от 2 до 20 смволов");
            else
                if (adr.Text.Trim().Length < 2 || adr.Text.Trim().Length > 100)
                MessageBox.Show("Поле логин должно содержать от 2 до 100 смволов");
            else
                if (phone.Text.Trim().Length < 7 || phone.Text.Trim().Length > 12)
                MessageBox.Show("Поле телефо должно содержать от 7 до 12 смволов");
            else
            {
                User.Reg(login.Text, pass.Text, name.Text, lName.Text, adr.Text, phone.Text);
                this.Close();
            }
        }
    }
}
