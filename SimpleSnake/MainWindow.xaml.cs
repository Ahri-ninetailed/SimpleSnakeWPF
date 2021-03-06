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
        public static string FoodDirectory = "Orange.png";
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
                Margin = new Thickness(left, top, 0, 0),
            };
            return image;
        }
        public int[] optimizedCoordinates()
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
            if (allFood.ContainsKey(new Tuple<double, double>(w_h[0], w_h[1])) || SimpleSnake.MySnake.CoordAllFirstSnakeUnits.ContainsValue(new Tuple<double, double>(w_h[0], w_h[1])) || ( w_h[0] == SimpleSnake.MySnake.HeadMargin.Left && w_h[1] == SimpleSnake.MySnake.HeadMargin.Top))
            {
                var tempArCreateFood = optimizedCoordinates();
                while(allFood.ContainsKey(new Tuple<double, double>(tempArCreateFood[0], tempArCreateFood[1])) || SimpleSnake.MySnake.CoordAllFirstSnakeUnits.ContainsValue(new Tuple<double, double>(tempArCreateFood[0], tempArCreateFood[1])) ||  (tempArCreateFood[0] == SimpleSnake.MySnake.HeadMargin.Left && tempArCreateFood[1] == SimpleSnake.MySnake.HeadMargin.Top))
                {
                    tempArCreateFood = optimizedCoordinates();
                }
                w_h = tempArCreateFood;
            }
            Image image = createImage(w_h[0], w_h[1]);
            image.Stretch = Stretch.UniformToFill;
            image.Source = bitmapImage;
            if (!allFood.ContainsKey(new Tuple<double, double>(image.Margin.Left, image.Margin.Top)))
                allFood.Add(new Tuple<double, double>(image.Margin.Left, image.Margin.Top), image);
            else
                allFood[new Tuple<double, double>(image.Margin.Left, image.Margin.Top)] = image;
            return image;
        }

        private static BitmapImage createBitmapImage()
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri($"{FoodDirectory}", UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
    public partial class MySnake
    {
        public static Thickness HeadMargin;
        public static Dictionary<Rectangle, Tuple<double, double>> CoordAllFirstSnakeUnits = new Dictionary<Rectangle, Tuple<double, double>>();
        public List<Rectangle> Bodies { get; set; } = new List<Rectangle>();
        public bool Growing =false;
        public bool StartedMySnakeDeleted = false;
        public event Action SnakeDead;
        public string DirectionSnake { get; set; } = "Up";
        public string DirectionSnakeTemp { get; set; } = "Up";

        public int CountHead { get; set; } = 1;
        public Rectangle Tail { get; set; } = new Rectangle()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Height = 30,
            Width = 30,
            Stroke = Brushes.White,
            VerticalAlignment = VerticalAlignment.Top,
        };
        public double TailMarginLeft
        {
            get
            {
                return Tail.Margin.Left;
            }
            set
            {
                Tail.Margin = new Thickness(value, TailMarginTop, 0, 0);
                if (!CoordAllFirstSnakeUnits.ContainsKey(Tail))
                   CoordAllFirstSnakeUnits.Add(Tail, new Tuple<double, double>(TailMarginLeft, TailMarginTop));
                else
                    CoordAllFirstSnakeUnits[Tail] = new Tuple<double, double>(TailMarginLeft, TailMarginTop);
            }
        }
        public double TailMarginTop
        {
            get
            {
                return Tail.Margin.Top;
            }
            set
            {
                Tail.Margin = new Thickness(TailMarginLeft, value, 0, 0);
                if (!CoordAllFirstSnakeUnits.ContainsKey(Tail))
                    CoordAllFirstSnakeUnits.Add(Tail, new Tuple<double, double>(TailMarginLeft, TailMarginTop));
                else
                    CoordAllFirstSnakeUnits[Tail] = new Tuple<double, double>(TailMarginLeft, TailMarginTop);
            }
        }
        public Rectangle Head { get; set; } = new Rectangle()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Height = 30,
            Width = 30,
            Stroke = Brushes.White,
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
                HeadMargin = new Thickness(value, HeadMarginTop, 0, 0);
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
                HeadMargin = new Thickness(HeadMarginLeft, value, 0, 0);
                SnakeDead();
            }
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
            timerSnake.Interval = new TimeSpan(0, 0, 0,0,700);
            timerFood.Tick += new EventHandler(timerTickCreateFood);
            timerFood.Interval = new TimeSpan(0, 0, 0,0,9000);
        }
        private void Snake_SnakeDead()
        {
            if (snake.HeadMarginLeft < 0 || snake.HeadMarginTop < 0 || snake.HeadMarginLeft > 750 || snake.HeadMarginTop > 540)
            { timerFood.Stop(); timerSnake.Stop(); Close(); }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        { 
            MainGrid.Children.Add(foods.CreateFood());
        }
        private void timerTickCreateFood(object sender, EventArgs e)
        {
            MainGrid.Children.Add(foods.CreateFood());
            if ((snake.CountHead - 1) % 3 == 0)
                timerFood.Interval = new TimeSpan(timerFood.Interval.Days, timerFood.Interval.Hours, timerFood.Interval.Minutes, timerFood.Interval.Seconds, timerFood.Interval.Milliseconds - 130);
        }
        private void timerTickSnakeMoving(object sender, EventArgs e)
        {
            controlSnake();
            if (foods.allFood.ContainsKey(new Tuple<double, double>(snake.HeadMarginLeft, snake.HeadMarginTop))) //ест апельсин
            {
                MainGrid.Children.Remove(foods.allFood[new Tuple<double, double>(snake.HeadMarginLeft, snake.HeadMarginTop)]);
                foods.allFood.Remove(new Tuple<double, double>(snake.HeadMarginLeft, snake.HeadMarginTop));
                Growth(snake);
                snake.CountHead += 1;
                Score.Content = (snake.CountHead - 1).ToString();
                if ((snake.CountHead-1) % 3 == 0)
                    timerSnake.Interval = new TimeSpan(timerSnake.Interval.Days, timerSnake.Interval.Hours, timerSnake.Interval.Minutes, timerSnake.Interval.Seconds, timerSnake.Interval.Milliseconds - 35);
            }
            if (SimpleSnake.MySnake.CoordAllFirstSnakeUnits.ContainsValue(new Tuple<double, double>(snake.HeadMarginLeft, snake.HeadMarginTop)) && snake.CountHead > 4 && snake.Growing == false)
            {
                Close();
            }
            void controlSnake()
            {   
                if (snake.CountHead == 1)
                {
                    if (snake.DirectionSnakeTemp.Equals("d", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Right", StringComparison.OrdinalIgnoreCase))
                    {
                        MySnake.Margin = new Thickness(MySnake.Margin.Left + 30, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.Head.Margin = MySnake.Margin;
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        snake.Head.Fill = MySnake.Fill;
                        snake.Tail.Margin = MySnake.Margin;
                        snake.DirectionSnake = "Right";

                    }
                    if (snake.DirectionSnakeTemp.Equals("a", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Left", StringComparison.OrdinalIgnoreCase))
                    {
                        MySnake.Margin = new Thickness(MySnake.Margin.Left - 30, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.Head.Margin = MySnake.Margin;
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        snake.Head.Fill = MySnake.Fill;
                        snake.Tail.Margin = MySnake.Margin;
                        snake.DirectionSnake = "Left";

                    }
                    if (snake.DirectionSnakeTemp.Equals("s", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Down", StringComparison.OrdinalIgnoreCase))
                    {
                        MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top + MySnake.Width, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.Head.Margin = MySnake.Margin;
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        snake.Head.Fill = MySnake.Fill;
                        snake.Tail.Margin = MySnake.Margin;
                        snake.DirectionSnake = "Down";

                    }
                    if (snake.DirectionSnakeTemp.Equals("w", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Up", StringComparison.OrdinalIgnoreCase))
                    {
                        MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top - MySnake.Width, MySnake.Margin.Right, MySnake.Margin.Bottom);
                        snake.Head.Margin = MySnake.Margin;
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        snake.Head.Fill = MySnake.Fill;
                        snake.Tail.Margin = MySnake.Margin;
                        snake.DirectionSnake = "Up";

                    }
                }
                else if (snake.CountHead == 2)
                {                
                    if (snake.DirectionSnakeTemp.Equals("d", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Right", StringComparison.OrdinalIgnoreCase))
                    {
                        MainGrid.Children.Remove(snake.Head);
                        MainGrid.Children.Remove(snake.Tail);
                        snake.Tail.Margin = snake.Head.Margin;
                        snake.Head.Margin = new Thickness(snake.Head.Margin.Left + 30, snake.Head.Margin.Top, 0, 0);
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        MainGrid.Children.Add(snake.Head);
                        MainGrid.Children.Add(snake.Tail);
                        if (!snake.StartedMySnakeDeleted)
                        {
                            MainGrid.Children.Remove(MySnake);//убирает изначальный объект MySnake
                            snake.StartedMySnakeDeleted = true;
                        }
                        snake.DirectionSnake = "Right";
                    }
                    if (snake.DirectionSnakeTemp.Equals("a", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Left", StringComparison.OrdinalIgnoreCase))
                    {
                        MainGrid.Children.Remove(snake.Head);
                        MainGrid.Children.Remove(snake.Tail);
                        snake.Tail.Margin = snake.Head.Margin;
                        snake.Head.Margin = new Thickness(snake.Head.Margin.Left - 30, snake.Head.Margin.Top, 0, 0);
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        MainGrid.Children.Add(snake.Head);
                        MainGrid.Children.Add(snake.Tail);
                        if (!snake.StartedMySnakeDeleted)
                        {
                            MainGrid.Children.Remove(MySnake);//убирает изначальный объект MySnake
                            snake.StartedMySnakeDeleted = true;
                        }
                        snake.DirectionSnake = "Left";

                    }
                    if (snake.DirectionSnakeTemp.Equals("s", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Down", StringComparison.OrdinalIgnoreCase))
                    {
                        MainGrid.Children.Remove(snake.Head);
                        MainGrid.Children.Remove(snake.Tail);
                        snake.Tail.Margin = snake.Head.Margin;
                        snake.Head.Margin = new Thickness(snake.Head.Margin.Left, snake.Head.Margin.Top + 30, 0, 0);
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top;
                        MainGrid.Children.Add(snake.Head);
                        MainGrid.Children.Add(snake.Tail);
                        if (!snake.StartedMySnakeDeleted)
                        {
                            MainGrid.Children.Remove(MySnake);//убирает изначальный объект MySnake
                            snake.StartedMySnakeDeleted = true;
                        }
                        snake.DirectionSnake = "Down";

                    }
                    if (snake.DirectionSnakeTemp.Equals("w", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Up", StringComparison.OrdinalIgnoreCase))
                    {
                        MainGrid.Children.Remove(snake.Head);
                        MainGrid.Children.Remove(snake.Tail);
                        snake.Tail.Margin = snake.Head.Margin;
                        snake.Head.Margin = new Thickness(snake.Head.Margin.Left, snake.Head.Margin.Top - 30, 0, 0);
                        snake.HeadMarginLeft = snake.Head.Margin.Left;
                        snake.HeadMarginTop = snake.Head.Margin.Top; 
                        MainGrid.Children.Add(snake.Head);
                        MainGrid.Children.Add(snake.Tail);
                        if (!snake.StartedMySnakeDeleted)
                        {
                            MainGrid.Children.Remove(MySnake);//убирает изначальный объект MySnake
                            snake.StartedMySnakeDeleted = true;
                        }
                        snake.DirectionSnake = "Up";

                    }
                }
                else if (snake.Growing)
                {
                    if (snake.DirectionSnake.Equals("d", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Right", StringComparison.OrdinalIgnoreCase))
                    {
                        snake.HeadMarginLeft += 30;
                    }
                    if (snake.DirectionSnake.Equals("a", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Left", StringComparison.OrdinalIgnoreCase))
                    {
                        snake.HeadMarginLeft -= 30;
                    }
                    if (snake.DirectionSnake.Equals("s", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Down", StringComparison.OrdinalIgnoreCase))
                    {
                        snake.HeadMarginTop += 30;
                    }
                    if (snake.DirectionSnake.Equals("w", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnake.Equals("Up", StringComparison.OrdinalIgnoreCase))
                    {
                        snake.HeadMarginTop -= 30;
                    }
                    snake.Growing = false;
                }
                else
                {
                    if (snake.DirectionSnakeTemp.Equals("d", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Right", StringComparison.OrdinalIgnoreCase))
                    {
                        Thickness tempHeadMargin = snake.Head.Margin;
                        snake.HeadMarginLeft += 30;
                        snake.TailMarginLeft = snake.Bodies[0].Margin.Left;
                        snake.TailMarginTop = snake.Bodies[0].Margin.Top;
                        snake.Bodies[0].Margin = tempHeadMargin;                  
                        Rectangle temp = snake.Bodies[0];
                        SimpleSnake.MySnake.CoordAllFirstSnakeUnits[temp] = new Tuple<double, double>(temp.Margin.Left, temp.Margin.Top);
                        snake.Bodies.Remove(snake.Bodies[0]);
                        snake.Bodies.Add(temp);
                        snake.DirectionSnake = "Right";

                    }
                    if (snake.DirectionSnakeTemp.Equals("a", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Left", StringComparison.OrdinalIgnoreCase))
                    {
                        Thickness tempHeadMargin = snake.Head.Margin;
                        snake.HeadMarginLeft -= 30;
                        snake.TailMarginLeft = snake.Bodies[0].Margin.Left;
                        snake.TailMarginTop = snake.Bodies[0].Margin.Top;
                        snake.Bodies[0].Margin = tempHeadMargin;
                        Rectangle temp = snake.Bodies[0];
                        SimpleSnake.MySnake.CoordAllFirstSnakeUnits[temp] = new Tuple<double, double>(temp.Margin.Left, temp.Margin.Top);
                        snake.Bodies.Remove(snake.Bodies[0]);
                        snake.Bodies.Add(temp);
                        snake.DirectionSnake = "Left";

                    }
                    if (snake.DirectionSnakeTemp.Equals("s", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Down", StringComparison.OrdinalIgnoreCase))
                    {
                        Thickness tempHeadMargin = snake.Head.Margin;
                        snake.HeadMarginTop += 30;
                        snake.TailMarginLeft = snake.Bodies[0].Margin.Left;
                        snake.TailMarginTop = snake.Bodies[0].Margin.Top;
                        snake.Bodies[0].Margin = tempHeadMargin;
                        Rectangle temp = snake.Bodies[0];
                        SimpleSnake.MySnake.CoordAllFirstSnakeUnits[temp] = new Tuple<double, double>(temp.Margin.Left, temp.Margin.Top);
                        snake.Bodies.Remove(snake.Bodies[0]);
                        snake.Bodies.Add(temp);
                        snake.DirectionSnake = "Down";

                    }
                    if (snake.DirectionSnakeTemp.Equals("w", StringComparison.OrdinalIgnoreCase) || snake.DirectionSnakeTemp.Equals("Up", StringComparison.OrdinalIgnoreCase))
                    {
                        Thickness tempHeadMargin = snake.Head.Margin;
                        snake.HeadMarginTop -= 30;
                        snake.TailMarginLeft = snake.Bodies[0].Margin.Left;
                        snake.TailMarginTop = snake.Bodies[0].Margin.Top;
                        snake.Bodies[0].Margin = tempHeadMargin;
                        Rectangle temp = snake.Bodies[0];
                        SimpleSnake.MySnake.CoordAllFirstSnakeUnits[temp] = new Tuple<double, double>(temp.Margin.Left, temp.Margin.Top);
                        snake.Bodies.Remove(snake.Bodies[0]);
                        snake.Bodies.Add(temp);
                        snake.DirectionSnake = "Up";
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
                Stroke = Brushes.White,
                VerticalAlignment = VerticalAlignment.Top,
            };
            if (snake.CountHead == 1)
            {
                newTail.Margin = new Thickness(snake.HeadMarginLeft, snake.HeadMarginTop, 0, 0);
                snake.Tail = newTail; 
                MainGrid.Children.Add(snake.Tail);
                snake.TailMarginLeft = snake.Tail.Margin.Left;
                snake.TailMarginTop = snake.Tail.Margin.Top;
            }
            else
            {
                newTail.Margin = new Thickness(snake.HeadMarginLeft, snake.HeadMarginTop, 0, 0);
                MainGrid.Children.Add(newTail);
                snake.Growing = true;
                snake.Bodies.Add(newTail);
                SimpleSnake.MySnake.CoordAllFirstSnakeUnits.Add(newTail, new Tuple<double, double>(newTail.Margin.Left, newTail.Margin.Top));
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            timerSnake.Start();
            timerFood.Start();
            var temp = foods.optimizedCoordinates();
            MySnake.Margin = new Thickness(temp[0], temp[1], 0, 0);
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            string tempKey = e.Key.ToString();
            if (tempKey != "Up" && tempKey != "Down" && tempKey != "Left" && tempKey != "Right" && tempKey != "W" && tempKey != "A" && tempKey != "S" && tempKey != "D")
                return;
            if (snake.CountHead != 1)
            {
                if (snake.DirectionSnake == "Up" || snake.DirectionSnake == "W")
                {
                    if (tempKey != "Down" && tempKey != "S")
                        snake.DirectionSnakeTemp = tempKey;
                }
                if (snake.DirectionSnake == "Down" || snake.DirectionSnake == "S")
                {
                    if (tempKey != "Up" && tempKey != "W")
                        snake.DirectionSnakeTemp = tempKey;
                }
                if (snake.DirectionSnake == "Left" || snake.DirectionSnake == "A")
                {
                    if (tempKey != "Right" && tempKey != "D")
                        snake.DirectionSnakeTemp = tempKey;
                }
                if (snake.DirectionSnake == "Right" || snake.DirectionSnake == "D")
                {
                    if (tempKey != "Left" && tempKey != "A")
                        snake.DirectionSnakeTemp = tempKey;
                }
            }
            else
            {
                snake.DirectionSnakeTemp = tempKey;
            }
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MessageBox.Show($"Конец игры. Ваш счет: {Score.Content}.");
        }

    }

}
