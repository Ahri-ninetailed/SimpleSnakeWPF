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

namespace SimpleSnake
{
    /// <summary>
    /// Логика взаимодействия для GameMenu.xaml
    /// </summary>
    public partial class GameMenu : Window
    {
        public static MediaPlayer backgroundMusic;
        public GameMenu()
        {
            InitializeComponent();
            backgroundMusic = new MediaPlayer();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            backgroundMusic.Open(new Uri(@"8-Bit Misfits - Dollhouse.mp3", UriKind.Relative));
            backgroundMusic.Volume = 0.15;
            backgroundMusic.Play();
            
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {

            var options = new Options();
            options.WindowStartupLocation = WindowStartupLocation;
            //options.ShowInTaskbar = true;
            options.ShowDialog();
        }

        private void Reference_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Управление осуществляется клавишами A, W, S, D или стрелками");
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().ShowDialog();
        }
    }
}
