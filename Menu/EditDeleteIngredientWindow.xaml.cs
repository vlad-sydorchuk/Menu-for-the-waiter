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
using Npgsql;
using System.Data;
using Microsoft.Win32;
using System.IO;

namespace Menu
{
    /// <summary>
    /// Логика взаимодействия для EditDeleteIngredientWindow.xaml
    /// </summary>
    public partial class EditDeleteIngredientWindow : Window
    {
        List<string> category = new List<string>(){"Завтраки", "Детское меню", "Закуски к пиву", "Холодные закуски",
          "Стейки", "Салаты", "Бургеры", "Антистейки", "Первые блюда", "Десерты"};

        List<string> ingDish = new List<string>();

        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();

        string host = "127.0.0.1";
        string port = "5432";
        string user = "3B_user";
        string pass = "1111";
        string db = "3BCafe";
        string sql = null;

        string newPathImage = null;
        string mainPath = null;
        string ingPath = null;

        public EditDeleteIngredientWindow()
        {
            InitializeComponent();
            updListBox();

            deleteIngredient.Visibility = Visibility.Visible;
        }

        public EditDeleteIngredientWindow(int k)
        {
            InitializeComponent();
            updListBox();

            deleteIngredient.Visibility = Visibility.Collapsed;
        
        }

        private void updListBox()
        {
            ingDish.Clear();
            try
            {
                string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                       host, port, user, pass, db);

                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();

                sql = "select name from tbl_ingredient";

                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();

                /* May be i can upgrade this, and don't read all data every run (just add last item?) */
                while (dr.Read())
                    ingDish.Add(dr.GetValue(0).ToString());

                lstBoxAvailbleIngredient.ItemsSource = ingDish;
                lstBoxAvailbleIngredient.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                conn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Упс! Что то пошло не так.\nOriginal msg:\n" + ex.ToString());
            }
        }


        private void deleteIngredient_Click_1(object sender, RoutedEventArgs e)
        {
            if (nameIngredient.Text.Length == 0)
                return;

            //imgIngredietn.Source = new BitmapImage(new Uri(mainPath + "\\Ingredient\\Авокадо.png"));
            string message = "Вы уверенны, что хотите удалить ингредиент: " + lstBoxAvailbleIngredient.SelectedItem.ToString();
            string caption = "Удаление ингредиента";
            var result = MessageBox.Show(message, caption, MessageBoxButton.OKCancel);
            
            if (result == MessageBoxResult.OK)
            {
                string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                       host, port, user, pass, db);

                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();

                NpgsqlCommand cmd = conn.CreateCommand();
                //MessageBox.Show(lstBoxAvailbleIngredient.SelectedItem.ToString());
                cmd.CommandText = "select id from tbl_ingredient where name='" + lstBoxAvailbleIngredient.SelectedItem.ToString() + "'";
                
                int id_ing = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = "delete from tbl_ingredient where id='" + id_ing + "'";
                cmd.ExecuteNonQuery();

                conn.Close();
                imgIngredient.Source = null;
                nameIngredient.Text = null;
                findIngredient.Text = null;
                updListBox();
                //File.Delete(mainPath + "\\Ingredient\\" + lstBoxAvailbleIngredient.SelectedItem.ToString() + ".png");
            }
        }

        private void findIngredient_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            List<String> tmpList = new List<String>();

            if (findIngredient.Text == "")
                lstBoxAvailbleIngredient.ItemsSource = ingDish;
            else
            {
                for (int i = 0; i < ingDish.Count(); i++)
                {
                    if (ingDish[i].ToLower().Contains(findIngredient.Text.ToLower()))
                        tmpList.Add(ingDish[i]);
                }
                lstBoxAvailbleIngredient.ItemsSource = tmpList;
            }
            lstBoxAvailbleIngredient.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void lstBoxAvailbleIngredient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                mainPath = Directory.GetCurrentDirectory();
                mainPath = mainPath.Substring(0, mainPath.IndexOf("\\bin"));
                mainPath = mainPath + "\\ResourceFolder";
                ingPath = mainPath + "\\Ingredient\\" + lstBoxAvailbleIngredient.SelectedItem + ".png";
                imgIngredient.Source = new BitmapImage(new Uri(ingPath));

            }
            catch (Exception ex) 
            {
                imgIngredient.Source = null;
            }

            try
            { 
                //MessageBox.Show(lstBoxAvailbleIngredient.SelectedItem.ToString());
                nameIngredient.Text = lstBoxAvailbleIngredient.SelectedItem.ToString();
            }
            catch (Exception ms)
            { lstBoxAvailbleIngredient.SelectedIndex = -1; }
        }

        /*
        private void newPhoto_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Изображения (*.png, *jpg)|*.png;*.jpg";

            Nullable<bool> res = dlg.ShowDialog();
            if (res == true)
            {
                imgIngredient.Source = new BitmapImage(new Uri(dlg.FileName));
                newPathImage = dlg.FileName;
            }
        }

        
        private void changeIngredient_Click_1(object sender, RoutedEventArgs e)
        {
            string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                       host, port, user, pass, db);

            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            NpgsqlCommand cmd = conn.CreateCommand();
            
            cmd.CommandText = "select id from tbl_ingredient where name='" + lstBoxAvailbleIngredient.SelectedItem.ToString() + "'";
            int id_ing = Convert.ToInt32(cmd.ExecuteScalar());

            cmd.CommandText = "update tbl_ingredient set name='" + nameIngredient.Text + "' where id=" + id_ing;
            cmd.ExecuteNonQuery();

            if (newPathImage == null)
            { 
                mainPath = Directory.GetCurrentDirectory();
                mainPath = mainPath.Substring(0, mainPath.IndexOf("\\bin"));
                mainPath = mainPath + "\\ResourceFolder";
                ingPath = mainPath + "\\Ingredient\\" + lstBoxAvailbleIngredient.SelectedItem + ".png";
                string oldIm, newIm;
                oldIm = mainPath + "\\Ingredient\\" + lstBoxAvailbleIngredient.SelectedItem + ".png";
                newIm = mainPath + "\\Ingredient\\" + nameIngredient.Text + ".png";
                //imgIngredient.Source = new BitmapImage(new Uri(ingPath));
                //File.Copy(oldIm, newIm);
                File.Replace(oldIm, newIm, "back.png");

            }

        }
        */
    }
}
