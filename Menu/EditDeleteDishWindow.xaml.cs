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
    /// Логика взаимодействия для EditDeleteDishWindow.xaml
    /// </summary>
    public partial class EditDeleteDishWindow : Window
    {
        List<string> category = new List<string>(){"Все блюда", "Завтрак", "Детское меню", "Закуски к пиву", "Холодные закуски",
          "Стейки", "Салаты", "Бургеры", "Антистейки", "Первые блюда", "Десерты"};

        List<string> lstFillcmb = new List<string>();
        List<string> lstFilltempIng = new List<string>();

        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();

        string host = "127.0.0.1";
        string port = "5432";
        string user = "3B_user";
        string pass = "1111";
        string db = "3BCafe";
        string sql = null;

        string mainPath = null;
        string dishPath = null;

        public EditDeleteDishWindow()
        {
            InitializeComponent();
            cmbCategory.ItemsSource = category;
            cmbCategory.SelectedIndex = 0;
            updList(cmbCategory.SelectedItem.ToString());

            this.Title = "Изменить/Удалить блюдо";
            deleteDish.Visibility = Visibility.Visible;
            changeIng.Visibility = Visibility.Visible;
        }

        public EditDeleteDishWindow(int k)
        {
            InitializeComponent();
            cmbCategory.ItemsSource = category;
            cmbCategory.SelectedIndex = 0;
            updList(cmbCategory.SelectedItem.ToString());

            this.Title = "Доступные блюда";
            deleteDish.Visibility = Visibility.Collapsed;
            changeIng.Visibility = Visibility.Collapsed;
        }

        private void updList(string category)
        {
            //MessageBox.Show(category);
            try
            {
                lstFillcmb.Clear();
                string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                       host, port, user, pass, db);

                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();

                NpgsqlCommand cmd = conn.CreateCommand();
                NpgsqlDataReader dr;

                if (category.Equals(cmbCategory.Items[0].ToString()))
                {
                    //MessageBox.Show(category);
                    cmd.CommandText = "select name from tbl_dish";
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                        lstFillcmb.Add(dr.GetValue(0).ToString());
                }
                else
                {
                    //MessageBox.Show(category);
                    cmd.CommandText = "select name from tbl_dish where category='" + category + "'";
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                        lstFillcmb.Add(dr.GetValue(0).ToString());
                }

                lstBoxAllDish.ItemsSource = lstFillcmb;
                lstBoxAllDish.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                conn.Close();
            }
            catch (Exception ex) 
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        private void deleteDish_Click_1(object sender, RoutedEventArgs e)
        {
            if (lstBoxAllDish.SelectedIndex != -1)
            {

                string message = "Вы уверенны, что хотите удалить блюдо: " + lstBoxAllDish.SelectedItem.ToString();
                string caption = "Удаление ингредиента";
                var result = MessageBox.Show(message, caption, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                           host, port, user, pass, db);

                    NpgsqlConnection conn = new NpgsqlConnection(connstring);
                    conn.Open();

                    NpgsqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select id from tbl_dish where name='" + lstBoxAllDish.SelectedItem.ToString() + "'";

                    int id_dish = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.CommandText = "delete from tbl_dish where id='" + id_dish + "'";
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    updList(cmbCategory.SelectedItem.ToString());

                    lstFilltempIng.Clear();
                    lstBoxAvailbleIngredient.ItemsSource = lstFilltempIng;

                    imgDish.Source = null;
                }
            }
            else
                MessageBox.Show("Выюерите блюдо, чтобы удалить его!");
        }

        private void changeIng_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция в разработке");
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("Change");
            updList(cmbCategory.SelectedItem.ToString());
        }

        private void lstBoxAllDish_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                mainPath = Directory.GetCurrentDirectory();
                mainPath = mainPath.Substring(0, mainPath.IndexOf("\\bin"));
                mainPath = mainPath + "\\ResourceFolder";
                dishPath = mainPath + "\\Dishes\\" + lstBoxAllDish.SelectedItem + ".png";
                if (File.Exists(dishPath))
                    imgDish.Source = new BitmapImage(new Uri(dishPath));
                else
                    imgDish.Source = null;

                lstFilltempIng.Clear();
                string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", host, port, user, pass, db);
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();

                sql = "select ingredients from tbl_dish where name='" + lstBoxAllDish.SelectedItem.ToString() + "'";

                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();

                dr.Read();
                string ing = dr.GetValue(0).ToString();
                lstFilltempIng = ing.Split(',').ToList<string>();
                lstBoxAvailbleIngredient.ItemsSource = lstFilltempIng;
                dr.Close();

                conn.Close();
            }
            catch (Exception ex)
            {
                lstBoxAllDish.SelectedIndex = -1; 
            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            List<String> tmpList = new List<String>();

            if (findDish.Text == "")
                lstBoxAllDish.ItemsSource = lstFillcmb;
            else
            {
                for (int i = 0; i < lstFillcmb.Count(); i++)
                {
                    if (lstFillcmb[i].ToLower().Contains(findDish.Text.ToLower()))
                        tmpList.Add(lstFillcmb[i]);
                }
                lstBoxAllDish.ItemsSource = tmpList;
            }
            lstBoxAllDish.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
        }
    }
}
