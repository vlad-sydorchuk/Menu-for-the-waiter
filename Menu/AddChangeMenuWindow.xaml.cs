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
    /// Логика взаимодействия для AddChangeMenuWindow.xaml
    /// </summary>
    public partial class AddChangeMenuWindow : Window
    {
        List<string> category = new List<string>(){"Завтрак", "Детское меню", "Закуски к пиву", "Холодные закуски",
          "Стейки", "Салаты", "Бургеры", "Антистейки", "Первые блюда", "Десерты"};

        List<string> ingDish = new List<string>();
        List<string> addedIng = new List<string>();

        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();

        string host = "127.0.0.1";
        string port = "5432";
        string user = "3B_user";
        string pass = "1111";
        string db = "3BCafe";
        string sql = null;

        string pathImage = null;


        public AddChangeMenuWindow() 
        { 
            InitializeComponent();
            cmbBoxCategories.ItemsSource = category;
            updListBox();
        }

        public void updListBox()
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

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
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

        private void lstBoxAvailbleIngredient_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (currIngredient.Items.Contains(lstBoxAvailbleIngredient.SelectedItem.ToString()))
                    MessageBox.Show("Этот ингредиент уже есть! :)");
                else
                {
                    currIngredient.Items.Add(lstBoxAvailbleIngredient.SelectedItem.ToString());
                    currIngredient.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Нажимайте по ингредиентам :)");
            }
        }

        private void currIngredient_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            currIngredient.Items.Remove(currIngredient.SelectedItem);
        }

        private void AddNewIngredient_Click(object sender, RoutedEventArgs e)
        {
            AddNewIngredientWindow addNewIng = new AddNewIngredientWindow();
            addNewIng.Owner = this;
            addNewIng.ShowDialog();
            updListBox();
        }

        private void AddNewDish_Click_1(object sender, RoutedEventArgs e)
        {
            string tmpIngredient = null;
            try
            {
                if (makeNameMoreBeautiful() != 1)
                {
                    string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                           host, port, user, pass, db);

                    NpgsqlConnection conn = new NpgsqlConnection(connstring);
                    conn.Open();

                    NpgsqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select count(*) from tbl_dish where name='" + newNameDish.Text + "'";

                    //ingredient exist? if n > 0 - yes
                    int n = Convert.ToInt32(cmd.ExecuteScalar());

                    if (n > 0)
                        MessageBox.Show("Блюдо уже есть в базе!");
                    else
                    {
                        // create str with all ingredient for current dish
                        for (int i = 0; i < currIngredient.Items.Count; i++)
                        {
                            if (i == currIngredient.Items.Count - 1)
                                tmpIngredient += currIngredient.Items[i].ToString();
                            else
                                tmpIngredient += currIngredient.Items[i].ToString() + ",";
                        }

                        sql = "insert into tbl_dish (name, ingredients, category) values('" + newNameDish.Text + "', '" + tmpIngredient + 
                            "', '" + cmbBoxCategories.SelectedItem.ToString() + "')";
                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                        da.Fill(ds);
                    }
                    conn.Close();

                    string mainPath = Directory.GetCurrentDirectory();
                    mainPath = mainPath.Substring(0, mainPath.IndexOf("\\bin"));
                    mainPath = mainPath + "\\ResourceFolder";
                    string dishPath = mainPath + "\\Dishes";

                    // Copy file in inner folder with name ingredient
                    if (File.Exists(dishPath + "\\" + newNameDish.Text + ".png") && pathImage != null)
                        File.Delete(dishPath + "\\" + newNameDish.Text + ".png");

                    if (pathImage != null)
                        File.Copy(pathImage, dishPath + "\\" + newNameDish.Text + ".png");
                    MessageBox.Show("Блюдо добавлено.");

                    newNameDish.Text = null;
                    newImgForDish.Source = null;
                    cmbBoxCategories.SelectedIndex = -1;
                    // clear before reusing lstbox
                    currIngredient.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Упс! Что то пошло не так.\nOriginal msg:\n" + ex.ToString());

                newNameDish.Text = null;
                newImgForDish.Source = null;
                cmbBoxCategories.SelectedIndex = -1;
                // clear before reusing lstbox
                currIngredient.Items.Clear();
            }
            
        }

        private int makeNameMoreBeautiful()
        {
            string tmp = "";

            if (newNameDish.Text.Length == 0)
                tmp += "Вы не ввели название блюда.\n";
            if (cmbBoxCategories.SelectedIndex == -1)
                tmp += "Вы не выбрали категорию.\n";
            if (currIngredient.Items.Count == 0)
                tmp += "Вы не выбрали ингредиенты для блюда.";
            if (tmp.Length > 0)
            {
                MessageBox.Show(tmp);
                return 1;
            }
            
            newNameDish.Text = newNameDish.Text.Substring(0, 1).ToUpper() +
                    newNameDish.Text.Substring(1, (newNameDish.Text.Length - 1)).ToLower();

            return 0;
        }

        private void addNewDishPhoto_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Изображения (*.png, *jpg)|*.png;*.jpg";

            Nullable<bool> res = dlg.ShowDialog();
            if (res == true)
            {
                newImgForDish.Source = new BitmapImage(new Uri(dlg.FileName));
                pathImage = dlg.FileName;
            }
        }

    }
}
