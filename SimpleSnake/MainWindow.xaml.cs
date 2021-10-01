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
        public string DirectionSnake { get; set; } = "Up";
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
        public double HeadMarginLeft
        {
            get
            {
                return Head.Margin.Left;
            }
            set
            {
                Head.Margin = new Thickness(value, HeadMarginTop, 0, 0);
                SnakeDead();
            }
        }
        public double HeadMarginTop
        {
            get
            {
                return Head.Margin.Top;
            }
            set
            {
                Head.Margin = new Thickness(HeadMarginLeft, value, 0, 0);
                SnakeDead();
            }
        }
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
            HeadMarginLeft = thickness.Left;
            HeadMarginTop = thickness.Top;
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
            HeadMarginLeft = thickness.Left;
            HeadMarginTop = thickness.Top;
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
            if (snake.HeadMarginLeft < 0 || snake.HeadMarginTop < 0 || snake.HeadMarginLeft > 750 || snake.HeadMarginTop > 540)
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
            if (foods.allFood.ContainsKey(new Tuple<double, double>(snake.HeadMarginLeft, snake.HeadMarginTop)))
            {
                MainGrid.Children.Remove(foods.allFood[new Tuple<double, double>(snake.HeadMarginLeft, snake.HeadMarginTop)]);
                Growth(snake);
                snake.CountHead += 1;
            }
            cord.Content = snake.CountHead;//темп

            void controlSnake()
            {
                if (snake.CountHead == 1)
                {
                    if (snake.DirectionSnake.Equals("d", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Right", StringComparison.OrdinalIgnoreCase))
                    {
                        snake.SetPrevMargin(MySnake.Margin);
                        MySnake.Margin = new Thickness(MySnake.Margin.Left + 30, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.Head.Margin = MySnake.Margin;
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        snake.Head.Fill = MySnake.Fill;
                        snake.Tail.Margin = MySnake.Margin;
                        snake.Tail.Fill = Brushes.Red;
                        snake.SetMargin(MySnake.Margin);
                        snake.SetTailMargin(MySnake.Margin);
                    }
                    if (snake.DirectionSnake.Equals("a", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Left", StringComparison.OrdinalIgnoreCase))
                    {
                        snake.SetPrevMargin(MySnake.Margin);
                        MySnake.Margin = new Thickness(MySnake.Margin.Left - 30, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.Head.Margin = MySnake.Margin;
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        snake.Head.Fill = MySnake.Fill;
                        snake.Tail.Margin = MySnake.Margin;
                        snake.Tail.Fill = Brushes.Red;
                        snake.SetMargin(MySnake.Margin);
                        snake.SetTailMargin(MySnake.Margin);
                    }
                    if (snake.DirectionSnake.Equals("s", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Down", StringComparison.OrdinalIgnoreCase))
                    {
                        snake.SetPrevMargin(MySnake.Margin);
                        MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top + MySnake.Width, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.Head.Margin = MySnake.Margin;
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        snake.Head.Fill = MySnake.Fill;
                        snake.Tail.Margin = MySnake.Margin;
                        snake.Tail.Fill = Brushes.Red;
                        snake.SetMargin(MySnake.Margin);
                        snake.SetTailMargin(MySnake.Margin);
                    }
                    if (snake.DirectionSnake.Equals("w", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Up", StringComparison.OrdinalIgnoreCase))
                    {
                        snake.SetPrevMargin(MySnake.Margin);
                        MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top - MySnake.Width, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.Head.Margin = MySnake.Margin;
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        snake.Head.Fill = MySnake.Fill;
                        snake.Tail.Margin = MySnake.Margin;
                        snake.Tail.Fill = Brushes.Red;
                        snake.SetMargin(MySnake.Margin);
                        snake.SetTailMargin(MySnake.Margin);
                    }
                }
                else if (snake.CountHead == 2)
                {
                    
                    if (snake.DirectionSnake.Equals("d", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Right", StringComparison.OrdinalIgnoreCase))
                    {
                        MainGrid.Children.Remove(snake.Head);
                        MainGrid.Children.Remove(snake.Tail);
                        snake.Tail.Margin = snake.Head.Margin;
                        snake.Head.Margin = new Thickness(snake.Head.Margin.Left + 30, snake.Head.Margin.Top, 0, 0);
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        snake.Head.Fill = Brushes.Black;
                        MainGrid.Children.Add(snake.Head);
                        MainGrid.Children.Add(snake.Tail);
                        if (!startedMySnakeDeleted)
                        {
                            MainGrid.Children.Remove(MySnake);//убирает изначальный объект MySnake
                            startedMySnakeDeleted = true;
                        }
                    }
                    if (snake.DirectionSnake.Equals("a", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Left", StringComparison.OrdinalIgnoreCase))
                    {
                        MainGrid.Children.Remove(snake.Head);
                        MainGrid.Children.Remove(snake.Tail);
                        snake.Tail.Margin = snake.Head.Margin;
                        snake.Head.Margin = new Thickness(snake.Head.Margin.Left - 30, snake.Head.Margin.Top, 0, 0);
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        MainGrid.Children.Add(snake.Head);
                        MainGrid.Children.Add(snake.Tail);
                        if (!startedMySnakeDeleted)
                        {
                            MainGrid.Children.Remove(MySnake);//убирает изначальный объект MySnake
                            startedMySnakeDeleted = true;
                        }
                    }
                    if (snake.DirectionSnake.Equals("s", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Down", StringComparison.OrdinalIgnoreCase))
                    {
                        MainGrid.Children.Remove(snake.Head);
                        MainGrid.Children.Remove(snake.Tail);
                        snake.Tail.Margin = snake.Head.Margin;
                        snake.Head.Margin = new Thickness(snake.Head.Margin.Left, snake.Head.Margin.Top + 30, 0, 0);
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        MainGrid.Children.Add(snake.Head);
                        MainGrid.Children.Add(snake.Tail);
                        if (!startedMySnakeDeleted)
                        {
                            MainGrid.Children.Remove(MySnake);//убирает изначальный объект MySnake
                            startedMySnakeDeleted = true;
                        }
                    }
                    if (snake.DirectionSnake.Equals("w", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Up", StringComparison.OrdinalIgnoreCase))
                    {
                        MainGrid.Children.Remove(snake.Head);
                        MainGrid.Children.Remove(snake.Tail);
                        snake.Tail.Margin = snake.Head.Margin;
                        snake.Head.Margin = new Thickness(snake.Head.Margin.Left, snake.Head.Margin.Top - 30, 0, 0);
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        MainGrid.Children.Add(snake.Head);
                        MainGrid.Children.Add(snake.Tail);
                        if (!startedMySnakeDeleted)
                        {
                            MainGrid.Children.Remove(MySnake);//убирает изначальный объект MySnake
                            startedMySnakeDeleted = true;
                        }
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

            }
            else if (snake.CountHead == 2)
            {

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
            if (snake.CountHead != 1)
            {
                string tempKey = e.Key.ToString();
                if (snake.DirectionSnake == "Up" || snake.DirectionSnake == "W")
                {
                    if (tempKey != "Down" && tempKey != "S")
                        snake.DirectionSnake = tempKey;
                }
                if (snake.DirectionSnake == "Down" || snake.DirectionSnake == "S")
                {
                    if (tempKey != "Up" && tempKey != "W")
                        snake.DirectionSnake = tempKey;
                }
                if (snake.DirectionSnake == "Left" || snake.DirectionSnake == "A")
                {
                    if (tempKey != "Right" && tempKey != "D")
                        snake.DirectionSnake = tempKey;
                }
                if (snake.DirectionSnake == "Right" || snake.DirectionSnake == "D")
                {
                    if (tempKey != "Left" && tempKey != "A")
                        snake.DirectionSnake = tempKey;
                }
            }
            else
                snake.DirectionSnake = e.Key.ToString();
            but.Content = snake.DirectionSnake;//темп
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MessageBox.Show("Конец игры");
        }
        private bool startedMySnakeDeleted = false;
    }

}
