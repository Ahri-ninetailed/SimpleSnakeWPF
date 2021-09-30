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
    public partial class MainWindow : Window
    {
        private string directionSnake = "";
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
            if (directionSnake.Equals("d", StringComparison.OrdinalIgnoreCase) || directionSnake.Equals("Right", StringComparison.OrdinalIgnoreCase))
                MySnake.Margin = new Thickness(MySnake.Margin.Left + MySnake.Width/2, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
            if (directionSnake.Equals("a", StringComparison.OrdinalIgnoreCase) || directionSnake.Equals("Left", StringComparison.OrdinalIgnoreCase))
                MySnake.Margin = new Thickness(MySnake.Margin.Left - MySnake.Width / 2, MySnake.Margin.Top, MySnake.Margin.Right, MySnake.Margin.Bottom);
            if (directionSnake.Equals("s", StringComparison.OrdinalIgnoreCase) || directionSnake.Equals("Down", StringComparison.OrdinalIgnoreCase))
                MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top + MySnake.Width / 2, MySnake.Margin.Right, MySnake.Margin.Bottom);
            if (directionSnake.Equals("w", StringComparison.OrdinalIgnoreCase) || directionSnake.Equals("Up", StringComparison.OrdinalIgnoreCase))
                MySnake.Margin = new Thickness(MySnake.Margin.Left, MySnake.Margin.Top - MySnake.Width / 2, MySnake.Margin.Right, MySnake.Margin.Bottom);
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTickSnakeMoving);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            directionSnake = e.Key.ToString();
            but.Content = directionSnake;
        }
    }

}
