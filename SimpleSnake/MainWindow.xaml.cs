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
using System.Threading;
namespace SimpleSnake
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MySnake
    {
        public event Action SnakeDead;

        public string DirectionSnake { get; set; } = "";
        public double MarginLeft { get; set; }
        public double MarginTop { get; set; }
        public void SetMargin(Thickness thickness)
        {
            MarginLeft = thickness.Left;
            MarginTop = thickness.Top;
        }

    }
    public partial class MainWindow : Window
    {
        MySnake snake = new MySnake();
        public MainWindow()
        {
            InitializeComponent();
            ContentRendered += Window_ContentRendered;
            KeyDown += Window_KeyDown;

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Background = new SolidColorBrush(Colors.DimGray);
            

        }

        private void timerTickSnakeMoving(object sender, EventArgs e)
        {
            void controlSnake()
            {
                if (snake.DirectionSnake.Equals("d", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Right", StringComparison.OrdinalIgnoreCase))
                {
                    MySnake.Margin = new Thickness(MySnake.Margin.Left + MySnake.Width / 2, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
                    snake.SetMargin(MySnake.Margin);
                }
                if (snake.DirectionSnake.Equals("a", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Left", StringComparison.OrdinalIgnoreCase))
                {
                    MySnake.Margin = new Thickness(MySnake.Margin.Left - MySnake.Width / 2, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
                    snake.SetMargin(MySnake.Margin);

                }
                if (snake.DirectionSnake.Equals("s", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Down", StringComparison.OrdinalIgnoreCase))
                {
                    MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top + MySnake.Width / 2, MySnake.Margin.Right, MySnake.Margin.Bottom);
                    snake.SetMargin(MySnake.Margin);

                }
                if (snake.DirectionSnake.Equals("w", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Up", StringComparison.OrdinalIgnoreCase))
                {
                    MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top - MySnake.Width / 2, MySnake.Margin.Right, MySnake.Margin.Bottom);
                    snake.SetMargin(MySnake.Margin);

                }
            }
            controlSnake();
            cord.Content = MySnake.Margin.Left.ToString() + " : " + MySnake.Margin.Top.ToString();//темп

        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            cord.Content = MySnake.Margin.Left.ToString() + " : " + MySnake.Margin.Top.ToString();//темп

            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTickSnakeMoving);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            snake.DirectionSnake = e.Key.ToString();
            but.Content = snake.DirectionSnake;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MessageBox.Show("Конец игры");
        }
    }

}
