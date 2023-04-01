using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    class Advert
    {
        public static List<UIElement> GetCars(List<Auto> autos, bool myCars = false)
        {
            List<UIElement> adverts = new List<UIElement>();
            foreach(Auto auto in autos)
            {
                DockPanel advert = new DockPanel()
                {
                    Height = 100,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(10)
                };
                Border border = new Border()
                {
                    Margin = new Thickness(10),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                };

                Label titleLb = new Label()
                {
                    Content = String.Format("{0} {1} {2}", auto.Model.Brand.Name.Trim(), auto.Model.Name.Trim(), auto.Release.Year) +"\nОбем двигателя: " + auto.Potencia + "См^3\n КПП: "+auto.Type.Name,
                    Margin = new Thickness(10, 10, 0, 0),
                    FontSize = 15,
                    FontWeight = FontWeight.FromOpenTypeWeight(500),
                };
                border.Child = advert;
                StackPanel statusPanel = new StackPanel();
                Label statusLb = new Label()
                {
                    Content = "Статус:",
                    FontSize = 15
                };
                statusPanel.Children.Add(statusLb);
                Label lb = new Label()
                {
                    FontSize = 15,
                    FontWeight = FontWeight.FromOpenTypeWeight(500),
                };
               


                Image img = new Image
                {
                    Source = auto.Image,
                    Stretch = Stretch.Fill,
                    Height = 90,
                    Width = 135
                };
                advert.Children.Add(img);
                advert.Children.Add(titleLb);
                if (auto.UserId == null)
                {
                    lb.Content = "Свободен";
                    lb.Foreground = Brushes.Green;
                    statusPanel.Children.Add(lb);
                    advert.Children.Add(statusPanel);
                }
                else
                {
                    lb.Content = "Занят";
                    lb.Foreground = Brushes.Red;
                    statusPanel.Children.Add(lb);
                    var expDate = (DateTime)auto.ExpDate;
                    Label l1 = new Label()
                    {
                        Content = "до " + expDate.ToString("dd:MM:yyyy"),
                        FontSize = 15,
                        FontWeight = FontWeight.FromOpenTypeWeight(500),
                    };
                    statusPanel.Children.Add(l1);
                    advert.Children.Add(statusPanel);

                    if (auto.UserId == User.Id && myCars)
                    {
                        Button del = new Button
                        {
                            Content = "Удалить",
                            Name = "del",
                            Tag = auto.Id,
                            Height = 28,
                            Width = 100
                        };
                        Button edit = new Button
                        {
                            Content = "Редактировать",
                            Name = "edit",
                            Tag = auto.Id,
                            Height = 28,
                            Width = 100,
                        };
                        edit.Click += Operation.UpdatePeriodFormShow;
                        del.Click += Operation.RemoveFromMyCarFormShow;
                        advert.Children.Add(del);
                        advert.Children.Add(edit);

                    }
                }
                adverts.Add(border);
            }
            return adverts;
        }
        
        public static List<UIElement> GetCarsWithFilter(List<Auto> allAuto, Brand brand)
        {
            List<Auto> autous = new List<Auto>();
            foreach (var item in allAuto)
                if (item.Model.Brand.Id == brand.Id)
                    autous.Add(item);

            return GetCars(autous);
        }

        public static List<UIElement> GetMyCars(List<Auto> allAuto)
        {
            List<Auto> autous = new List<Auto>();
            foreach (var item in allAuto)
                if (item.UserId == User.Id)
                    autous.Add(item);

            return GetCars(autous, true);
        }
    }
}
