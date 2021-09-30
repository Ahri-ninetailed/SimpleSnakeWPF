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
    public partial class Food
    {
        public Image CreateFood()
        {
            Image image = new Image()
            {
                Name = "food",
                Width = 30,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(1, 1, 0, 0)
            };

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("Properties/foodImage1.png", UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();
            image.Stretch = Stretch.UniformToFill;
            image.Source = bitmapImage;
            return image;
        }
    }
    public partial class MySnake
    {
        public event Action SnakeDead;
        public string DirectionSnake { get; set; } = "";
        private double marginLeft;
        private double marginTop;

        public double MarginLeft
        {
            get { return marginLeft; }
            set
            {
                marginLeft = value;
                SnakeDead();
            }
        }
        public double MarginTop
        {
            get { return marginTop; }
            set
            {
                marginTop = value;
                SnakeDead();

            }
        }
        public void SetMargin(Thickness thickness)
        {
            MarginLeft = thickness.Left;
            MarginTop = thickness.Top;
        }

    }
    public partial class MainWindow : Window
    {
        Food foods = new Food();
        MySnake snake = new MySnake();
        System.Windows.Threading.DispatcherTimer timerSnake = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer timerFood = new System.Windows.Threading.DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            ContentRendered += Window_ContentRendered;
            KeyDown += Window_KeyDown;
            snake.SnakeDead += Snake_SnakeDead;


            timerSnake.Tick += new EventHandler(timerTickSnakeMoving);
            timerSnake.Interval = new TimeSpan(0, 0, 1);

            timerFood.Tick += new EventHandler(timerTickCreateFood);
            timerFood.Interval = new TimeSpan(0, 0, 2);


        }

        private void Snake_SnakeDead()
        {
            if (snake.MarginLeft < 0 || snake.MarginTop < 0 || snake.MarginLeft > 770 || snake.MarginTop > 570)
            { timerFood.Stop(); timerSnake.Stop(); Close(); }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Background = new SolidColorBrush(Colors.DimGray);
        }
        private void timerTickCreateFood(object sender, EventArgs e)
        {
            
            

            MainGrid.Children.Add(foods.CreateFood());
            //MainGrid.Children.Remove(rectangle);




        }
        private void timerTickSnakeMoving(object sender, EventArgs e)
        {
            void controlSnake()
            {
                if (snake.DirectionSnake.Equals("d", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Right", StringComparison.OrdinalIgnoreCase))
                {
                    MySnake.Margin = new Thickness(MySnake.Margin.Left + MySnake.Width, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
                    snake.SetMargin(MySnake.Margin);
                }
                if (snake.DirectionSnake.Equals("a", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Left", StringComparison.OrdinalIgnoreCase))
                {
                    MySnake.Margin = new Thickness(MySnake.Margin.Left - MySnake.Width, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
                    snake.SetMargin(MySnake.Margin);

                }
                if (snake.DirectionSnake.Equals("s", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Down", StringComparison.OrdinalIgnoreCase))
                {
                    MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top + MySnake.Width, MySnake.Margin.Right, MySnake.Margin.Bottom);
                    snake.SetMargin(MySnake.Margin);

                }
                if (snake.DirectionSnake.Equals("w", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Up", StringComparison.OrdinalIgnoreCase))
                {
                    MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top - MySnake.Width, MySnake.Margin.Right, MySnake.Margin.Bottom);
                    snake.SetMargin(MySnake.Margin);

                }
            }
            controlSnake();
            cord.Content = MySnake.Margin.Left.ToString() + " : " + MySnake.Margin.Top.ToString();//темп

        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            cord.Content = MySnake.Margin.Left.ToString() + " : " + MySnake.Margin.Top.ToString();//темп
            timerSnake.Start();
            timerFood.Start();
            
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
