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
using System.Resources;
using Npgsql;
using System.Data;
using Microsoft.Win32;
using System.IO;


namespace Menu
{
    /// <summary>
    /// Логика взаимодействия для AddNewIngredientWindow.xaml
    /// </summary>
    public partial class AddNewIngredientWindow : Window
    {
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();
        

        string host = "127.0.0.1";
        string port = "5432";
        string user = "3B_user";
        string pass = "1111";
        string db = "3BCafe";
        string sql = null;

        string pathImage = null;

        public AddNewIngredientWindow()
        {
            InitializeComponent();
        }

        private void AddNewIngredient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (makeNameMoreBeautiful() != 1)
                {

                    string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                           host, port, user, pass, db);

                    NpgsqlConnection conn = new NpgsqlConnection(connstring);
                    conn.Open();

                    NpgsqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select count(*) from tbl_ingredient where name='" + newNameIngredient.Text + "'";

                    //ingredient exist? if n > 0 - yes
                    int n = Convert.ToInt32(cmd.ExecuteScalar());

                    if (n > 0)
                        MessageBox.Show("Ингредиент уже есть в базе!");
                    else
                    {
                        sql = "insert into tbl_ingredient (name) values('" + newNameIngredient.Text + "')";
                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                        da.Fill(ds);
                        MessageBox.Show("Ингредиент добавлен.");

                        /* Copy file in inner folder with name ingredient */
                        if (pathImage != null)
                        {
                            string mainPath = Directory.GetCurrentDirectory();
                            mainPath = mainPath.Substring(0, mainPath.IndexOf("\\bin"));
                            mainPath = mainPath + "\\ResourceFolder";
                            string ingredPath = mainPath + "\\Ingredient";

                            if (File.Exists(ingredPath + "\\" + newNameIngredient.Text + ".png"))
                                File.Delete(ingredPath + "\\" + newNameIngredient.Text + ".png");
                            File.Copy(pathImage, ingredPath + "\\" + newNameIngredient.Text + ".png");
                        }

                    }
                    conn.Close();
                }
                else
                    MessageBox.Show("Введите название ингредиента.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Упс! Что то пошло не так.\nOriginal msg:\n" + ex.ToString());
            }
            newNameIngredient.Text = null;
            newImgForIngredient.Source = null;
         }

        private void btnAddNewIngredientPhoto_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Изображения (*.png, *jpg)|*.png;*.jpg";

            Nullable<bool> res = dlg.ShowDialog();
            if (res == true)
            {
                newImgForIngredient.Source = new BitmapImage(new Uri(dlg.FileName));
                pathImage = dlg.FileName;
            }
        }

        /* Make text more beautiful. Examlpe: vlad sydorchuk (student) --> Vlad Sydorchuk (student)  */
        private int makeNameMoreBeautiful()
        {
            if (newNameIngredient.Text.Length == 0)
                return 1;

            newNameIngredient.Text = newNameIngredient.Text.Substring(0, 1).ToUpper() +
                    newNameIngredient.Text.Substring(1, (newNameIngredient.Text.Length - 1));

            for (int i = 1; i < newNameIngredient.Text.Length; i++)
            {
                if (newNameIngredient.Text[i - 1] == ' ')
                    newNameIngredient.Text =
                        newNameIngredient.Text.Substring(0, i) +
                        newNameIngredient.Text.Substring(i, 1).ToUpper() +
                        newNameIngredient.Text.Substring(i + 1, newNameIngredient.Text.Length - i - 1);
            }
            return 0;
        }
    }
}
