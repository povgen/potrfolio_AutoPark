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
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для addAdvert.xaml
    /// </summary>
    public partial class AdvertEditor : Window
    {
        private List<Type>  types  = Type.LoadFromDb();
        private List<Brand> brands = Brand.LoadFromDb();
        private List<Model> models = Model.LoadFromDb();
        private List<Auto> autos = Auto.LoadFromDb();

        public AdvertEditor()
        {   
            InitializeComponent();

            foreach (Type type in types)
            {
                AutoType.Items.Add(type.Name);
                TypeId.Items.Add(String.Format("{0,-3}| {1}", type.Id, type.Name));
            }
            TypeId.Items.Add("Добавить");


            foreach (Brand brand in brands)
            {
                BrandId.Items.Add(String.Format("{0,-3}| {1}", brand.Id, brand.Name));
                ModelBrand.Items.Add(String.Format("{0,-3}| {1}", brand.Id, brand.Name));
            }
            BrandId.Items.Add("Добавить");


            foreach (Model model in models)
            {
                AutoModel.Items.Add(String.Format("{0} {1}",model.Brand.Name.Trim(), model.Name.Trim()));
                ModelId.Items.Add(String.Format("{0,-3}| {1}", model.Id, model.Name));
            }
            ModelId.Items.Add("Добавить");


            foreach (Auto auto in autos)
                AutoId.Items.Add(auto.Id);
            AutoId.Items.Add("Добавить");
        }

    // работа с таблицей Auto (машины)
        private void ImageSelect(object sender, RoutedEventArgs e) //Выбор изображения
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            dialog.FilterIndex = 1;  // ставим фильтр Image 

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
                AutoImageRef.Text = dialog.FileName;
        }

        private void SaveAutoToDb(object sender, RoutedEventArgs e)
        {
            if (AutoId.SelectedIndex == -1)
                MessageBox.Show("Выбеите № Машины");
            else
                if (AutoType.SelectedIndex == -1 || AutoModel.SelectedIndex == -1)
                    MessageBox.Show("Выберете тип КПП и модель");
            else
                if (AutoRelease.SelectedDate == null)
                    MessageBox.Show("Выберите дату релиза");
            else
                if (AutoId.SelectedIndex == AutoId.Items.Count - 1) //save | Добавть
            {
                try
                {
                    byte[] imageData;
                    using (System.IO.FileStream fs = new System.IO.FileStream(AutoImageRef.Text, FileMode.Open))
                    {
                        imageData = new byte[fs.Length];
                        fs.Read(imageData, 0, imageData.Length);
                    }
                    autos.Add(Auto.SaveToDb(types[AutoType.SelectedIndex], models[AutoModel.SelectedIndex], (DateTime)AutoRelease.SelectedDate, int.Parse(AutoPotencia.Text), imageData));
                    AutoId.Items[AutoId.SelectedIndex] = autos[AutoId.SelectedIndex].Id;
                    AutoId.Items.Add("Добавить");
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            } 
            else //Update
            {
                byte[] imageData;
                using (System.IO.FileStream fs = new System.IO.FileStream(AutoImageRef.Text, FileMode.Open))
                {
                    imageData = new byte[fs.Length];
                    fs.Read(imageData, 0, imageData.Length);
                }
                autos[AutoId.SelectedIndex].Update(types[AutoType.SelectedIndex], models[AutoModel.SelectedIndex], (DateTime)AutoRelease.SelectedDate, Int32.Parse(AutoPotencia.Text), imageData);
                MessageBox.Show("Запись обновлена");
            }
            
        }

        private void DeletAuto(object sender, RoutedEventArgs e)
        {
            if (AutoId.SelectedIndex != -1)
            {
                autos[AutoId.SelectedIndex].Delete();
                AutoId.Items.RemoveAt(AutoId.SelectedIndex);
                MessageBox.Show("Запись Удалена");
            }
            else
                MessageBox.Show("Выберете № Машины");
        }

        void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void AutoId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AutoId.Items.Count - 1 == AutoId.SelectedIndex || AutoId.SelectedIndex == -1)
            {
                AutoType.SelectedIndex = -1;
                AutoModel.SelectedIndex = -1;
                AutoImageRef.Text = "";
                AutoPotencia.Text = "";
                AutoRelease.Text = "";
                AutoImage.Source = null;
            }
            else
            {
                for (int i = 0; i < types.Count; i++)
                    if (types[i].Id == autos[AutoId.SelectedIndex].Type.Id)
                        AutoType.SelectedIndex = i;

                for (int i = 0; i < models.Count; i++)
                    if (models[i].Id == autos[AutoId.SelectedIndex].Model.Id)
                        AutoModel.SelectedIndex = i;

                AutoPotencia.Text = autos[AutoId.SelectedIndex].Potencia.ToString();
                AutoRelease.SelectedDate = autos[AutoId.SelectedIndex].Release;
                AutoImage.Source = autos[AutoId.SelectedIndex].Image;
            }
        }
        private void UploadImage(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] imageData;
                using (System.IO.FileStream fs = new System.IO.FileStream(AutoImageRef.Text, FileMode.Open))
                {
                    imageData = new byte[fs.Length];
                    fs.Read(imageData, 0, imageData.Length);
                }
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new System.IO.MemoryStream(imageData);
                image.EndInit();
                image.Freeze();
                AutoImage.Source = image;
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        // работа с таблицей Model (модель)

        private void SaveModelToDb(object sender, RoutedEventArgs e)
        {
            int id = ModelId.SelectedIndex;
            if (ModelId.SelectedIndex == -1)
                MessageBox.Show("№ Модели не выбран");
            else if (ModelBrand.SelectedIndex == -1)
                MessageBox.Show("Не выбрана марка");
            else if (ModelId.Items.Count - 1 == id)
            {
                models.Add(Model.SaveToDb(ModelName.Text, brands[ModelBrand.SelectedIndex]));
                ModelId.Items[id] = String.Format("{0,-3}| {1}", models[id].Id, models[id].Name);
                ModelId.Items.Add("Добавить");
                AutoModel.Items.Add(String.Format("{0} {1}", models[id].Brand.Name.Trim(), models[id].Name.Trim()));
            }
            else
            {
                models[id].Update(ModelName.Text, brands[ModelBrand.SelectedIndex]);
                ModelId.Items[id] = String.Format("{0,-3}| {1}", models[id].Id, models[id].Name);
                AutoModel.Items[id] = String.Format("{0,-15}| {1}", models[id].Name, models[id].Brand.Name);
                ModelId.SelectedIndex = id;
            }
        }

        private void ModelId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelId.Items.Count - 1 == ModelId.SelectedIndex || ModelId.SelectedIndex == -1)
            {
                ModelName.Text = "";
                ModelBrand.SelectedIndex = -1;
            }
            else
            {
                ModelName.Text = models[ModelId.SelectedIndex].Name.Trim();
                for (int i = 0; i < brands.Count; i++)
                    if (brands[i].Id == models[ModelId.SelectedIndex].Brand.Id)
                        ModelBrand.SelectedIndex = i;

            }
        }

        private void DeleteModelFromDb(object sender, RoutedEventArgs e)
        {
            if (ModelId.SelectedIndex == -1)
                MessageBox.Show("№ Модели не выбран");
            else
            {
                models[ModelId.SelectedIndex].Delete();
                models.RemoveAt(ModelId.SelectedIndex);
                AutoModel.Items.RemoveAt(ModelId.SelectedIndex);
                ModelId.Items.RemoveAt(ModelId.SelectedIndex);
            }
        }



    // работа с таблицей Brand (марка)

        private void SaveBrandToDb(object sender, RoutedEventArgs e)
        {
            int id = BrandId.SelectedIndex;
            if (BrandId.SelectedIndex == -1)
                MessageBox.Show("№ Марки не выбран");
            else
                if (BrandId.Items.Count - 1 == id)
            {
                brands.Add(Brand.SaveToDb(BrandName.Text));
                BrandId.Items[id] = String.Format("{0,-3}| {1}", brands[id].Id, brands[id].Name);
                BrandId.Items.Add("Добавить");
                ModelBrand.Items.Add(brands[id].Name);
            }
            else
            {
                brands[id].Update(BrandName.Text);
                BrandId.Items[id] = String.Format("{0,-3}| {1}", brands[id].Id, brands[id].Name);
                ModelBrand.Items[id] = brands[id].Name;
                BrandId.SelectedIndex = id;
            }
        }

        private void BrandId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrandId.Items.Count - 1 == BrandId.SelectedIndex || BrandId.SelectedIndex == -1)
                BrandName.Text = "";
            else
                BrandName.Text = brands[BrandId.SelectedIndex].Name.Trim();
        }

        private void DeleteBrandFromDb(object sender, RoutedEventArgs e)
        {
            if (BrandId.SelectedIndex == -1)
                MessageBox.Show("№ Марки не выбран");
            else
            {
                brands[BrandId.SelectedIndex].Delete();
                brands.RemoveAt(BrandId.SelectedIndex);
                ModelBrand.Items.RemoveAt(BrandId.SelectedIndex);
                BrandId.Items.RemoveAt(BrandId.SelectedIndex);
            }
        }



    // работа с таблицей Type (тип КПП)

        private void TypeId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeId.Items.Count - 1 == TypeId.SelectedIndex || TypeId.SelectedIndex == -1)
                TypeName.Text = "";
            else
                TypeName.Text = types[TypeId.SelectedIndex].Name.Trim();

        }

        private void SaveTypeToDb(object sender, RoutedEventArgs e)
        {
            int id = TypeId.SelectedIndex;
            if (TypeId.SelectedIndex == -1)
                MessageBox.Show("№ типа КПП не выбран");
            else
                if (TypeId.Items.Count - 1 == id)
                {
                    types.Add(Type.SaveToDb(TypeName.Text));
                    TypeId.Items[id] = String.Format("{0,-3}| {1}", types[id].Id, types[id].Name);
                    TypeId.Items.Add("Добавить");   
                    AutoType.Items.Add(types[id].Name);
                }
                else
                {
                    types[id].Update(TypeName.Text);
                    TypeId.Items[id] = String.Format("{0,-3}| {1}", types[id].Id, types[id].Name);
                    AutoType.Items[id] = String.Format(types[id].Name);
                    TypeId.SelectedIndex = id;
                }
        }

        private void DeleteTypeFromDb(object sender, RoutedEventArgs e)
        {
            if (TypeId.SelectedIndex == -1)
                MessageBox.Show("№ типа КПП не выбран");
            else
            {
                types[TypeId.SelectedIndex].Delete();
                types.RemoveAt(TypeId.SelectedIndex);
                AutoType.Items.RemoveAt(TypeId.SelectedIndex);
                TypeId.Items.RemoveAt(TypeId.SelectedIndex);
            }
        }
    }
}
