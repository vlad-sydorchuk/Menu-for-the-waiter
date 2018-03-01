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
using System.Timers;
using System.Windows.Threading;
using Npgsql;
using System.Data;
using Microsoft.Win32;
using System.IO;

namespace Menu
{
    /// <summary>
    /// Логика взаимодействия для TestingWindow.xaml
    /// </summary>
    public partial class TestingWindow : Window
    {
        private int flag = 0;
        private int time = 0;
        int lvl = 6;

        List<string> lstDishes = new List<string>();
        List<string> ingredientCurrDish = new List<string>();
        List<string> allIngredient = new List<string>();
        List<string> randomIngredient = new List<string>();
        List<string> randomDish = new List<string>();

        List<string> ansYes = new List<string>();
        List<string> ansNo = new List<string>();

        int cAllIngredient = 0;
        int cRightIngredient = 0;
        int cFalseIngredient = 0;
        int cRightDish = 0;

        string host = "127.0.0.1";
        string port = "5432";
        string user = "3B_user";
        string pass = "1111";
        string db = "3BCafe";

        int countCurrIngredient = 0;
        int countDish = 0;
        int tmpCountDish = 0;
        int tmpCountIngredient = 0;

        private DispatcherTimer tmr;

        public TestingWindow(string category)
        {
            InitializeComponent();

            this.Title = "Категория: ";
            switch (category)
            {
                case "Завтрак": 
                    this.Title += "Завтрак";
                    break;
                case "Детскоеменю":
                    this.Title += "Детское меню";
                    break;
                case "Закускикпиву":
                    this.Title += "Закуски к пиву";
                    break;
                case "Холодныезакуски":
                    this.Title += "Холодные закуски";
                    break;
                case "Стейки":
                    this.Title += "Стейки";
                    break;
                case "Салаты":
                    this.Title += "Салаты";
                    break;
                case "Бургеры":
                    this.Title += "Бургеры";
                    break;
                case "Антистейки":
                    this.Title += "Антистейки";
                    break;
                case "Первыеблюда":
                    this.Title += "Первые блюда";
                    break;
                case "Десерты":
                    this.Title += "Десерты";
                    break;
            }
        }

        private void fillListAllIngredient()
        {
            allIngredient.Clear();
            try
            {
                string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                           host, port, user, pass, db);

                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();

                NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select name from tbl_ingredient";

                NpgsqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                    allIngredient.Add(dr.GetValue(0).ToString());
                dr.Close();

                conn.Close();
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка при заполнении List'a всеми возможными ингредиентами. \nОригинальная ошибка: " + ex.ToString()); }
        }

        private void fillListDish()
        {
            lstDishes.Clear();
            try
            {
                string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                           host, port, user, pass, db);

                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();

                NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select name from tbl_dish where category='" + this.Title.Substring(this.Title.IndexOf(" ") + 1) + "'";

                NpgsqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                    lstDishes.Add(dr.GetValue(0).ToString());
                dr.Close();

                countDish = lstDishes.Count;

                conn.Close();
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка при заполнении List'a блюдами. \nОригинальная ошибка:\n" + ex.ToString()); }
        }

        private void randomFillDish()
        {
            int tmp = lstDishes.Count;
            int n;

            try
            {
                Random rand = new Random();
                while (randomDish.Count != tmp)
                {
                    n = rand.Next(lstDishes.Count);
                    randomDish.Add(lstDishes[n].ToString());
                    lstDishes.RemoveAt(n);
                }
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка при случайном заполнении List'a блюдами. \nОригинальная ошибка:\n" + ex.ToString()); }
        }

        private void fillCurrIngredient()
        {
            ingredientCurrDish.Clear();
            try
            {
                string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                           host, port, user, pass, db);

                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();

                NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select ingredients from tbl_dish where name='" + currentDish.Header.ToString() + "'";

                MessageBox.Show(cmd.CommandText.ToString());

                NpgsqlDataReader dr = cmd.ExecuteReader();

                dr.Read();
                string ing = dr.GetValue(0).ToString();
                MessageBox.Show(ing.ToString());
                ingredientCurrDish = ing.Split(',').ToList<string>();
                dr.Close();

                conn.Close();
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка при заполнении List'a ингредиентами текущего блюда. \nОригинальная ошибка:\n" + ex.ToString()); }
        
        }

        private void randomFillIngredient()
        {
            int tmp = ingredientCurrDish.Count;
            int n;

            try
            {
                Random rand = new Random();
                while (randomIngredient.Count != tmp)
                {
                    n = rand.Next(ingredientCurrDish.Count);
                    randomIngredient.Add(ingredientCurrDish[n].ToString());
                    ingredientCurrDish.RemoveAt(n);
                }

                if (tmp < 5)
                    tmp += 3;
                else if (tmp >= 5 && tmp <= 7)
                    tmp += 4;
                else
                    tmp += 5;

                while (randomIngredient.Count != tmp)
                {
                    n = rand.Next(allIngredient.Count);
                    if (randomIngredient.Contains(allIngredient[n]) == false)
                        randomIngredient.Add(allIngredient[n]);
                }
                countCurrIngredient = randomIngredient.Count;
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка при случайном заполнении List'a ингредиентами текущего блюда. \nОригинальная ошибка:\n" + ex.ToString()); }
        
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (time > 0)
            {

                if (time <= 10)
                {
                    if (time % 2 == 0)
                        forTimer.Foreground = Brushes.Red;
                    else
                        forTimer.Foreground = Brushes.White;
                    time--;
                    forTimer.Text = string.Format("0{0}:0{1}", time / 60, time % 60);
                }
                else
                {
                    time--;
                    forTimer.Text = string.Format("0{0}:{1}", time / 60, time % 60);
                }
            }
            else
            {
                tmr.Stop();
                flag = 0;
                
                txtCurrIngred.Text = "";
                imgCurrIngred.Source = null;
                currentDish.Header = "Тут будет название текущего блюда";
                txtHelp.Content = "Нажмите дважды тут, чтобы начать";
                MessageBox.Show("Время вышло");
            }
        }

        private void getRandIngredient()
        {
            int n;
            try
            {
                if (randomIngredient.Count > 0)
                {
                    Random rand = new Random();
                    n = rand.Next(randomIngredient.Count);

                    txtCurrIngred.Text = randomIngredient[n];

                    string mainPath = Directory.GetCurrentDirectory();
                    mainPath = mainPath.Substring(0, mainPath.IndexOf("\\bin"));
                    mainPath = mainPath + "\\ResourceFolder";
                    string ingredPath = mainPath + "\\Ingredient\\" + txtCurrIngred.Text + ".png";

                    if (File.Exists(ingredPath))
                        imgCurrIngred.Source = new BitmapImage(new Uri(ingredPath));
                    else
                        imgCurrIngred.Source = null;
                    randomIngredient.RemoveAt(n);
                    tmpCountIngredient++;
                    updDataIntxtHelp();
                }
                else
                {
                    txtCurrIngred.Text = "";
                    imgCurrIngred.Source = null;


                    //check all answers about this dish
                    checkRightIngredient();
                    getRandNextDish();
                }
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка при попытке получить следующий ингредиент для текущего блюда. \nОригинальная ошибка:\n" + ex.ToString()); }
        }

        private void getRandNextDish()
        {
            ansYes.Clear();
            ansNo.Clear();
            tmpCountIngredient = 0;
            tmpCountDish++;
            int n;
            try
            {
                if (randomDish.Count > 0)
                {
                    Random rand = new Random();
                    n = rand.Next(randomDish.Count);

                    currentDish.Header = randomDish[n];
                    fillCurrIngredient();
                    randomFillIngredient();
                    getRandIngredient();
                    time = lvl * countCurrIngredient;
                    randomDish.RemoveAt(n);
                }
                else
                {
                    tmr.Stop();
                    flag = 0;
                    txtCurrIngred.Text = "";
                    imgCurrIngred.Source = null;
                    currentDish.Header = "Тут будет название текущего блюда";
                    txtHelp.Content = "Нажмите дважды тут, чтобы начать";
                    MessageBox.Show("Больше блюд нет!");
                    
                }
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка при попытке получить следующее блюдо. \nОригинальная ошибка:\n" + ex.ToString()); }
        }

        private void updDataIntxtHelp()
        {
            txtHelp.Content = "Блюда: " + tmpCountDish.ToString() + "/" + countDish.ToString() +
        "\nИнгредиенты: " + tmpCountIngredient.ToString() + "/" + countCurrIngredient.ToString();
        }

        private void txtHelp_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (flag == 0)
            {
                flag = 1;

                /* Counters */
                tmpCountDish = 0;
                tmpCountIngredient = 0;
                cAllIngredient = 0;
                cRightIngredient = 0;
                cFalseIngredient = 0;
                cRightDish = 0;

                /* Lists */
                //lstDishes.Clear();
                //ingredientCurrDish.Clear();
                //allIngredient.Clear();
                randomIngredient.Clear();
                randomDish.Clear();

                /* Functions */
                fillListAllIngredient();
                fillListDish();
                randomFillDish();

                getRandNextDish();

                /* Timer */
                time = lvl * countCurrIngredient;
                tmr = new DispatcherTimer();
                tmr.Interval = new TimeSpan(0, 0, 1);
                tmr.Tick += Timer_Tick;
                tmr.Start();
            }
        }

        /* Checking list, if ingredientCurrDish contain some ingredient from ansYes -> cRightIngredient++ */
        private void checkRightIngredient()
        {
            int tmpCountRight = 0;
            try
            {
                cFalseIngredient = 0;
                ingredientCurrDish.Clear();
                fillCurrIngredient();

                cAllIngredient += ingredientCurrDish.Count;

                for (int i = 0; i < ansYes.Count; i++)
                {
                    if (ingredientCurrDish.Contains(ansYes[i]))
                    {
                        cRightIngredient++;
                        tmpCountRight++;
                    }
                    else
                        cFalseIngredient++;
                }

                if (ingredientCurrDish.Count == tmpCountRight && cFalseIngredient == 0)
                    cRightDish++;

                txtRightDish.Text = "Количество правильно определенных блюд: " + cRightDish.ToString() + "/" + countDish.ToString();
                txtRightIngredient.Text = "Количество правильно определенных ингредиентов: " + cRightIngredient.ToString() + "/" + cAllIngredient.ToString();
                txtFalseIngredient.Text = "Количество неправильно определенных ингредиентов: " + cFalseIngredient.ToString();

                /*
                MessageBox.Show("Правильно добавленные ингредиенты: " + cRightIngredient.ToString() + 
                    "\nНеправильно добавленные ингредиенты: " + cFalseIngredient.ToString() + 
                    "\nВсего правильных ингредиентов: " + cAllIngredient.ToString() + 
                    "\nКонечная оценка: " + (cRightIngredient - cFalseIngredient).ToString());
                */
            }
            catch (Exception ex)
            {
                txtRightDish.Text = "Количество правильно определенных блюд: ";
                txtRightIngredient.Text = "Количество правильно определенных ингредиентов: ";
                txtFalseIngredient.Text = "Количество неправильно определенных ингредиентов: ";
            }
        }

        /* Set how many seconds need for answer about ingredient (Example: "Eggs are part of the 'Eggs with bacon'") */
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            lvl = 7; // 7 seconds for answer (Begginer)
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            lvl = 5; // 5 seconds
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            lvl = 2; // 2 seconds (Like a God :D)
        }


        /* Here, i'm added ingredient for list with "yes" answers */
        private void btnYes_Click_1(object sender, RoutedEventArgs e)
        {
            ansYes.Add(txtCurrIngred.Text);
            getRandIngredient();
        }

        /* Same like an above, but for list with "no" */
        private void btnNo_Click_1(object sender, RoutedEventArgs e)
        {
            ansNo.Add(txtCurrIngred.Text);
            getRandIngredient();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                tmr.Stop();
            }
            catch (Exception ex) { }
        }

    }
}
