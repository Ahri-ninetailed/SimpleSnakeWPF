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
    /// 
    public partial class Music
    {
        public static MediaPlayer BackgroundMusic;
        public enum BackgroundPlaylist { Numb, SweetDreams, BadGuy, Demons, Spotlight };
        private static BackgroundPlaylist backgroundMusics;
        public static int DurMinBack { get; set; }
        public static int DurSecBack { get; set; }
        public static string BackgroungMusics 
        {
            get
            {
                return backgroundMusics.ToString();
            }
            set 
            {
                backgroundMusics = (BackgroundPlaylist)Int32.Parse(value);
            }
        }
        public static void ChangeBackgroundMusic()
        {
            if (GameMenu.TimerMusic.IsEnabled)
                GameMenu.TimerMusic.Stop();
            if (BackgroundMusic.IsBuffering)
                BackgroundMusic.Stop();
            Random random = new Random();
            BackgroungMusics = random.Next(5).ToString();
            BackgroundMusic.Open(new Uri($@"{Music.BackgroungMusics}.mp3", UriKind.Relative));
            BackgroundMusic.Volume = 0.15;
            BackgroundMusic.Play();
            GameMenu.TimerMusic.Start();
        }
    }
    
    public partial class GameMenu : Window
    {
        public static bool MusicCheckboxIsChecked = false;
        public static System.Windows.Threading.DispatcherTimer TimerMusic = new System.Windows.Threading.DispatcherTimer();
        public GameMenu()
        {
            InitializeComponent();
            Music.BackgroundMusic = new MediaPlayer();
            TimerMusic.Tick += TimerMusic_Tick;
            TimerMusic.Interval = new TimeSpan(0, 0, 1);
            Music.BackgroundMusic.MediaOpened += BackgroundMusic_MediaOpened;
        }

        private void BackgroundMusic_MediaOpened(object sender, EventArgs e)
        {
            Music.DurMinBack = Int32.Parse(Music.BackgroundMusic.NaturalDuration.TimeSpan.Minutes.ToString());
            Music.DurSecBack = Int32.Parse(Music.BackgroundMusic.NaturalDuration.TimeSpan.Seconds.ToString());
        }

        private void TimerMusic_Tick(object sender, EventArgs e)
        {
            int sec = Int32.Parse(Music.BackgroundMusic.Position.Seconds.ToString());
            int min = Int32.Parse(Music.BackgroundMusic.Position.Minutes.ToString());
            if (sec >= (Music.DurSecBack - 3) && min >= (Music.DurMinBack))
                Music.ChangeBackgroundMusic();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Music.ChangeBackgroundMusic();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {

            var options = new Options();
            options.WindowStartupLocation = WindowStartupLocation;
            options.Music.IsChecked = MusicCheckboxIsChecked;
            //options.ShowInTaskbar = true;
            options.ShowDialog();
        }

        private void Reference_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Управление осуществляется клавишами A, W, S, D или стрелками.\nПри выборе текстуры еды, нужно выбирать картинку с прозрачным фоном.");
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().ShowDialog();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            
        }

        private void Textures_Click(object sender, RoutedEventArgs e)
        {
            var options = new Textures();
            options.WindowStartupLocation = WindowStartupLocation;
            //options.ShowInTaskbar = true;
            options.ShowDialog();
        }
    }
}
