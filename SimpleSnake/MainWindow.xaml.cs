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
        public Dictionary<Tuple<double, double>, Image> allFood = new Dictionary<Tuple<double, double>, Image>();
        private Random random = new Random();
        private Image createImage(int left, int top)
        {
            Image image = new Image()
            {
                Width = 30,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(left, top, 0, 0)
            };
            return image;
        }
        private int[] optimizedCoordinates()
        {
            int w = random.Next(0, 771);
            int h = random.Next(0, 570);
            if (w % 30 != 0)
            {
                w -= (w % 30);
            }
            if (h % 30 != 0)
            {
                h -= (h % 30);
            }
            return new int[] { w, h };

        }
        public Image CreateFood()
        {
            BitmapImage bitmapImage = createBitmapImage();
            var w_h = optimizedCoordinates();
            if (allFood.ContainsKey(new Tuple<double, double>(w_h[0], w_h[1])))
            {
                var tempArCreateFood = optimizedCoordinates();
                while(allFood.ContainsKey(new Tuple<double, double>(tempArCreateFood[0], tempArCreateFood[1])))
                {
                    tempArCreateFood = optimizedCoordinates();
                }
                w_h = tempArCreateFood;
            }
            Image image = createImage(w_h[0], w_h[1]);
            image.Stretch = Stretch.UniformToFill;
            image.Source = bitmapImage;
            allFood.Add(new Tuple<double, double>(image.Margin.Left, image.Margin.Top), image);
            return image;

        }

        private static BitmapImage createBitmapImage()
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("Properties/foodImage1.png", UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
    public partial class MySnake
    {
        public event Action SnakeDead;
        public string DirectionSnake { get; set; } = "";
        private double marginLeft;
        private double marginTop;
        public int CountHead { get; set; } = 1;
        public Rectangle Tail { get; set; } = new Rectangle()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Height = 30,
            Width = 30,
            Stroke = Brushes.Black,
            VerticalAlignment = VerticalAlignment.Top,
        };
        public double TailMarginLeft { get; set; }
        public double TailMarginTop { get; set; }
        public double PrevMarginLeft { get; set; }
        public double PrevMarginTop { get; set; }
        public Rectangle Head { get; set; } = new Rectangle()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Height = 30,
            Width = 30,
            Stroke = Brushes.Black,
            VerticalAlignment = VerticalAlignment.Top,
        };
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
        public void SetTailMargin(Thickness thickness)
        {
            TailMarginLeft = thickness.Left;
            TailMarginTop = thickness.Top;
        }
        public void SetPrevMargin(Thickness thickness)
        {
            PrevMarginLeft = thickness.Left;
            PrevMarginTop = thickness.Top;
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
            timerSnake.Interval = new TimeSpan(0, 0, 0,0,800);

            timerFood.Tick += new EventHandler(timerTickCreateFood);
            timerFood.Interval = new TimeSpan(0, 0, 1);


        }

        private void Snake_SnakeDead()
        {
            if (snake.Head.Margin.Left < 0 || snake.Head.Margin.Top < 0 || snake.Head.Margin.Left > 750 || snake.Head.Margin.Top > 540)
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
            
            controlSnake();
            if (foods.allFood.ContainsKey(new Tuple<double, double>(snake.Head.Margin.Left, snake.Head.Margin.Top)))
            {
                MainGrid.Children.Remove(foods.allFood[new Tuple<double, double>(snake.Head.Margin.Left, snake.Head.Margin.Top)]);
                Growth(snake);
                snake.CountHead += 1;
            }
            cord.Content = snake.CountHead;//темп

            void controlSnake()
            {
                if (snake.DirectionSnake.Equals("d", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Right", StringComparison.OrdinalIgnoreCase))
                {
                    if (snake.CountHead == 1)
                    {
                        snake.Head.Margin = MySnake.Margin;
                        snake.Head.Fill = MySnake.Fill;
                        snake.SetPrevMargin(MySnake.Margin);

                        MySnake.Margin = new Thickness(MySnake.Margin.Left + MySnake.Width, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.SetMargin(MySnake.Margin);

                        snake.SetTailMargin(MySnake.Margin);
                    }
                }
                if (snake.DirectionSnake.Equals("a", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Left", StringComparison.OrdinalIgnoreCase))
                {

                    if (snake.CountHead == 1)
                    {
                        snake.Head.Margin = MySnake.Margin;
                        snake.Head.Fill = MySnake.Fill;
                        snake.SetPrevMargin(MySnake.Margin);

                        MySnake.Margin = new Thickness(MySnake.Margin.Left - MySnake.Width, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.SetMargin(MySnake.Margin);

                        snake.SetTailMargin(MySnake.Margin);
                    }
                }
                if (snake.DirectionSnake.Equals("s", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Down", StringComparison.OrdinalIgnoreCase))
                {
                    if (snake.CountHead == 1)
                    {
                        snake.Head.Margin = MySnake.Margin;
                        snake.Head.Fill = MySnake.Fill;
                        snake.SetPrevMargin(MySnake.Margin);

                        MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top + MySnake.Width, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.SetMargin(MySnake.Margin);

                        snake.SetTailMargin(MySnake.Margin);
                    }
                }
                if (snake.DirectionSnake.Equals("w", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Up", StringComparison.OrdinalIgnoreCase))
                {
                    if (snake.CountHead == 1)
                    {

                        snake.SetPrevMargin(MySnake.Margin);

                        MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top - MySnake.Width, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.Head.Margin = MySnake.Margin;
                        snake.Head.Fill = MySnake.Fill;
                        snake.Tail.Margin = MySnake.Margin;
                        snake.Tail.Fill = Brushes.Red;
                        snake.SetMargin(MySnake.Margin);
                        snake.SetTailMargin(MySnake.Margin);
                    }
                    else if (snake.CountHead == 2)
                    {
                        MainGrid.Children.Remove(snake.Head);
                        MainGrid.Children.Remove(snake.Tail);
                        //Thickness temp = snake.Tail.Margin;
                        snake.Tail.Margin = snake.Head.Margin;
                        //snake.Tail.Margin = new Thickness(snake.Tail.Margin.Left, snake.Tail.Margin.Top - 30, 0, 0);
                        snake.Head.Margin = new Thickness(snake.Head.Margin.Left, snake.Head.Margin.Top - 30, 0, 0);
                        snake.Head.Fill = Brushes.Black;
                        MainGrid.Children.Add(snake.Head);
                        MainGrid.Children.Add(snake.Tail);

                        /*snake.Tail = new Rectangle()
                        {
                            Fill = MySnake.Fill,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Height = 30,
                            Width = 30,
                            Stroke = Brushes.Black,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(snake.MarginLeft, snake.MarginTop, 0, 0)
                        };*/


                    }

                }
            }
        }

        private void Growth(MySnake snake)
        {
            Rectangle newTail = null;
            newTail = new Rectangle()
            {
                Fill = snake.Head.Fill,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 30,
                Width = 30,
                Stroke = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Top,

            };  
            if (snake.CountHead == 1)
            {
                newTail.Margin = new Thickness(snake.PrevMarginLeft, snake.PrevMarginTop, 0, 0);
                newTail.Fill = Brushes.Red;
                snake.Tail = newTail;
                MainGrid.Children.Add(snake.Tail);
                snake.SetTailMargin(snake.Tail.Margin);
                
                //
                MainGrid.Children.Remove(snake.Head);
                snake.Head.Fill = Brushes.Green;
                MainGrid.Children.Add(snake.Head);

            }


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
