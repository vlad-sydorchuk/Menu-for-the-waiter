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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;
using System.Data;
using System.IO;

namespace Menu
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckFolderResource();
        }

        /* Create folders for future images if they not exist */
        private void CheckFolderResource()
        {
            string mainPath = Directory.GetCurrentDirectory();
            string dishPath = null;
            string ingredPath = null;
            
            mainPath = mainPath.Substring(0, mainPath.IndexOf("\\bin"));
            mainPath = mainPath + "\\ResourceFolder";
            dishPath = mainPath + "\\Dishes";
            ingredPath = mainPath + "\\Ingredient";

            if (Directory.Exists(mainPath))
            {
                if (!(Directory.Exists(dishPath)))
                    Directory.CreateDirectory(dishPath);
                if (!(Directory.Exists(ingredPath)))
                    Directory.CreateDirectory(ingredPath);
            }
            else
            {
                Directory.CreateDirectory(mainPath);
                Directory.CreateDirectory(dishPath);
                Directory.CreateDirectory(ingredPath);
            }
        }

        private void ShowAllDish_Click(object sender, RoutedEventArgs e)
        {
            EditDeleteDishWindow eddw = new EditDeleteDishWindow(1);
            eddw.Owner = this;
            eddw.ShowDialog();
        }

        private void AddNewDish_Click(object sender, RoutedEventArgs e)
        {
            AddChangeMenuWindow changeDish = new AddChangeMenuWindow();
            changeDish.Owner = this;
            changeDish.ShowDialog();
        }

        private void ChangeDish_Click(object sender, RoutedEventArgs e)
        {
            EditDeleteDishWindow eddw = new EditDeleteDishWindow();
            eddw.Owner = this;
            eddw.ShowDialog();
        }

        private void ShowAllIngredient_Click_1(object sender, RoutedEventArgs e)
        {
            EditDeleteIngredientWindow ediw = new EditDeleteIngredientWindow(1);
            ediw.Owner = this;
            ediw.ShowDialog();
        }

        private void AddNewIngredient_Click_1(object sender, RoutedEventArgs e)
        {
            AddNewIngredientWindow aniw = new AddNewIngredientWindow();
            aniw.Owner = this;
            aniw.ShowDialog();
        }

        private void ChangeIngredient_Click_1(object sender, RoutedEventArgs e)
        {
            EditDeleteIngredientWindow ediw = new EditDeleteIngredientWindow();
            ediw.Owner = this;
            ediw.ShowDialog();
        }

        private void testing_category(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            TestingWindow tw = new TestingWindow((string)btn.Name);
            tw.Owner = this;
            tw.ShowDialog();
        }
    }
}
