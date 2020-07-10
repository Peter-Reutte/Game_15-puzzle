using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;

namespace Fifteen
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        List<Button> ListOfButtons = new List<Button>();
        bool btnMixIsPressed = false;

        // окно "Победа", в котором выдается результат
        Window wndResult = new Window();
        // в окне "Победа" кнопка "Начать новую игру"
        Button btnNewGame = new Button();
        // в окне "Победа" кнопка "Закрыть игру"
        Button btnCloseGame = new Button();

        // таймер для игры
        DispatcherTimer timer = new DispatcherTimer();
        // время, за которое игра будет пройдена
        int myTime = 0;
        // Label, в котором будет выдан результат
        Label lblTimeResult = new Label();

        string strRules; //текст с правилами игры

        public MainWindow()
        {
            InitializeComponent();
            int y = 1;
            //создаем игровое поле с 4х4 с кнопками
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    ListOfButtons.Add(new Button()); //добавляем новую кнопку
                    ListOfButtons[ListOfButtons.Count - 1].Name = "b" + y.ToString(); //задаем имя кнопки
                    y++; //увеличиваем значение счетчика на 1
                    //устанавливаем позицию кнопки
                    ListOfButtons[ListOfButtons.Count - 1].Margin = new Thickness(50 * i, 50 * j, 50 * (3 - i), 50 * (3 - j));
                    gridMain.Children.Add(ListOfButtons[ListOfButtons.Count - 1]); //добавляем кнопку в поле gridMain
                    ListOfButtons[ListOfButtons.Count - 1].Background = new SolidColorBrush(Color.FromRgb(0x3a, 0x4b, 0x6f));
                    ListOfButtons[ListOfButtons.Count - 1].BorderBrush = new SolidColorBrush(Color.FromRgb(0x39, 0x18, 0x1a));
                    if (i != 3 || j != 3) //если это не последняя кнопка, то будем задавать ей значение по умолчанию
                    {
                        ListOfButtons[ListOfButtons.Count - 1].Content = (j * 4 + i + 1); //кладём в кнопку значение по умолчанию
                        ListOfButtons[ListOfButtons.Count - 1].FontSize = 16;
                        ListOfButtons[ListOfButtons.Count - 1].FontWeight = FontWeights.Medium;
                        ListOfButtons[ListOfButtons.Count - 1].Foreground = new SolidColorBrush(Colors.Tan);
                    }
                    ListOfButtons[ListOfButtons.Count - 1].Click += btn_Click; //присваивем кнопке метод клика
                }
            }

            //окно с результатом
            wndResult.Visibility = Visibility.Hidden; //по умолчанию делаем окно с результатом невидимым
            wndResult.Width = 300;
            wndResult.Height = 150;
            wndResult.ResizeMode = ResizeMode.NoResize;
            wndResult.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wndResult.Title = "Победа!";
            wndGame.Closing += new System.ComponentModel.CancelEventHandler(wndGameClosing);

            StackPanel spResult = new StackPanel(); //кладём в окно с результом основной StackPanel
            wndResult.Content = spResult;
            spResult.Background = new SolidColorBrush(Color.FromRgb(0xe1, 0xe1, 0xcb));
            StackPanel spResult1 = new StackPanel();
            StackPanel spResult2 = new StackPanel();
            StackPanel spResult3 = new StackPanel();
            StackPanel spResult4 = new StackPanel();
            spResult.Children.Add(spResult1);
            spResult.Children.Add(spResult2);
            spResult.Children.Add(spResult3);
            spResult.Children.Add(spResult4);

            // создаем Label "Вы выиграли"
            Label lblYouAreWin = new Label();
            lblYouAreWin.Content = "Вы выиграли!";
            lblYouAreWin.Foreground = new SolidColorBrush(Color.FromRgb(0xc0, 0x43, 0x39));
            lblYouAreWin.FontSize = 16;
            lblYouAreWin.FontWeight = FontWeights.Bold;
            spResult1.Children.Add(lblYouAreWin);
            lblYouAreWin.HorizontalAlignment = HorizontalAlignment.Center;

            spResult2.Orientation = Orientation.Horizontal;
            // создаем Label "Ваш результат"
            Label lblResult = new Label();
            spResult2.Children.Add(lblResult);
            lblResult.Content = "   Ваш результат: ";
            spResult2.Children.Add(lblTimeResult);
            lblResult.Foreground = new SolidColorBrush(Color.FromRgb(0x1D, 0x29, 0x3E));

            //устанавливаем таймер
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            lblTimeResult.Foreground = new SolidColorBrush(Color.FromRgb(0x1D, 0x29, 0x3E));

            // создаем Label "сек."
            Label lblSec = new Label();
            lblSec.Content = " сек.";
            spResult2.Children.Add(lblSec);
            lblSec.Foreground = new SolidColorBrush(Color.FromRgb(0x1D, 0x29, 0x3E));

            // StackPanel с кнопками "Начать новую игру" и "Выйти из игры"
            spResult4.Orientation = Orientation.Horizontal;
            spResult4.HorizontalAlignment = HorizontalAlignment.Center;
            btnNewGame.Margin = new Thickness(10);
            btnNewGame.Content = "  Начать новую игру  ";
            btnNewGame.Foreground = new SolidColorBrush(Colors.Tan);
            btnNewGame.Background = new SolidColorBrush(Color.FromRgb(0x3a, 0x4b, 0x6f));
            btnNewGame.Height = 30;
            btnNewGame.Click += btnMix_Click;

            btnCloseGame.Content = "  Выйти из игры  ";
            btnCloseGame.Foreground = new SolidColorBrush(Colors.Tan);
            btnCloseGame.Background = new SolidColorBrush(Color.FromRgb(0x3a, 0x4b, 0x6f));
            btnCloseGame.Margin = new Thickness(10);
            btnCloseGame.Click += btnCloseGame_Click;
            btnCloseGame.Height = 30;
            spResult4.Children.Add(btnNewGame);
            spResult4.Children.Add(btnCloseGame);

            //окно "Правила игры"
            FileStream fsRules = new FileStream("Rules.txt", FileMode.Open, FileAccess.Read);
            StreamReader srRules = new StreamReader(fsRules, Encoding.Default);
            strRules = srRules.ReadToEnd();
            srRules.Close();
            fsRules.Close();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            myTime++; //за один тик увеличиваем счетчик времени - увеличиваем время на 1 секунду
            lblTimer.Content = myTime; // кладём значение в Label со временем в окне с игрой
        }

        // обрабатываем событие клика по кнопке внутри блока с самой игрой
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            //проверка, являются ли кнопки соседними через значения Margin
            if (((Math.Abs((sender as Button).Margin.Left - ListOfButtons[ListOfButtons.Count - 1].Margin.Left) + Math.Abs((sender as Button).Margin.Top - ListOfButtons[ListOfButtons.Count - 1].Margin.Top))) == 50)
            {
                Thickness th = new Thickness();
                th = ListOfButtons[ListOfButtons.Count - 1].Margin;
                ListOfButtons[ListOfButtons.Count - 1].Margin = (sender as Button).Margin;
                (sender as Button).Margin = th;
            }

            // проверка, решена ли головоломка 
            if (btnMixIsPressed) // проверка, нажата ли кнопка "Начать игру"
            {
                int l = 0; //счетчик ячеек, совпадающих с их положением в решенной головоломке
                for (int i = 0; i < 15; i++) //счетчик всех ячеек
                {
                    int marginLeft = int.Parse(ListOfButtons[i].Margin.Left.ToString()); //находим значение Margin от левого края
                    marginLeft = marginLeft / 50 + 1; //находим позицию ячейки по горизонтали
                    int marginTop = int.Parse(ListOfButtons[i].Margin.Top.ToString()); //находим значение Margin от верхнего края
                    marginTop = (marginTop / 50) * 4; //находим позицию ячейки по вертикали

                    //если значение сумм Margin(положение кнопки с цифрой в блоке игры) равно содержимому кнопки, то увели-чиваем счетчик l
                    if ((marginLeft + marginTop) == int.Parse(ListOfButtons[i].Content.ToString()))
                    {
                        l++;
                    }
                }
                //проверяем счетчик
                if (l == 15)
                {
                    //если каждая кнопка оказывается на своём месте, то делаем окно с результатом видимым
                    wndResult.Visibility = Visibility.Visible;
                    timer.Stop(); //останавливаем таймер
                    lblTimeResult.Content = myTime; //в Label с результатом кладём значение времени
                }
            }
        }
        //кнопка для перемешивания значений в ячейках для начала игры
        private void btnMix_Click(object sender, RoutedEventArgs e) //перемешать
        {
            myTime = 0; //обнуляем таймер в начале игры
            timer.Start(); //запускаем таймер
            wndResult.Visibility = Visibility.Hidden; //делаем окно "Победа" невидимым
            btnMixIsPressed = true; //устанавливаем флаг, что игра начата
            int N;      //сумма, для определения, решаема ли головоломка
                        //если N - нечетная, то решения головоломки не существует
            do
            {
                N = 0;
                Random random = new Random();
                for (int i = 0; i < 15 / 2; i++)
                {
                    int i1 = random.Next(0, 15); // первый индекс
                    int i2 = random.Next(0, 15); // второй индекс
                                                 // обмен значений элементов с индексами i1 и i2 

                    int temp = int.Parse(ListOfButtons[i1].Content.ToString());
                    ListOfButtons[i1].Content = ListOfButtons[i2].Content;
                    ListOfButtons[i2].Content = temp;
                }
                //находим значение N, для того что бы определить, решаема ли головоломка (это берем из её метематического описания)
                for (int j = 1; j < 15; j++) //начинаем со второй ячейки (с индексом 1)
                {
                    int k = 0; //счетчик количества ячеек, имеющих значение больше, чем j, но находящихся перед ней в таблице головоломки
                               //изначально обнуляем его
                    for (int i = 0; i < j; i++) //проходим все ячейки, находящиеся перед ячейкой с индексом j
                    {
                        //сравниваем значения лежащие в ячейках с индексами i и j, если в i-ой ячейке находится значение больше, чем в j-ой, то увеличиваем счетчик
                        if (int.Parse(ListOfButtons[j].Content.ToString()) < int.Parse((ListOfButtons[i].Content.ToString())))
                        {
                            k++; //увеличиваем счетчик
                        }
                    }
                    N += k; //находим сумму количества таких ячеек
                }
                int row = (int.Parse(ListOfButtons[14].Margin.Top.ToString())) / 50 + 1; //находим ряд, в котором находится пустая ячейка
                N += row;
            } while (N % 2 == 1); //пока сумма нечетна, перемешиваем, пока N ни станет четным и головоломка ни станет решаемой
        }
        //элемент из меню, показывающий MessageBox с информацией об игре
        private void miRulesOfGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(strRules, "Правила игры", MessageBoxButton.OK);
        }
        //кнопка закрытия игры в окне с результатом
        private void btnCloseGame_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            wndResult.Close(); //закрываем окно результатов
            wndGame.Close(); //закрываем окно с игрой            
        }
        private void wndGameClosing(object sender, EventArgs e)
        {
            wndResult.Close(); //закрывание окна результатов 
        }
    }
}
