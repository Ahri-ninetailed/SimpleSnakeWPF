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
using Microsoft.Win32;
namespace SimpleSnake
{
    /// <summary>
    /// Логика взаимодействия для Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();
        }

        private void Music_Checked(object sender, RoutedEventArgs e)
        {
            SimpleSnake.Music.BackgroundMusic.Stop();
            GameMenu.MusicCheckboxIsChecked = true;
        }

        private void Music_Unchecked(object sender, RoutedEventArgs e)
        {
            SimpleSnake.Music.BackgroundMusic.Play();
            GameMenu.MusicCheckboxIsChecked = false;

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChooseTetxure_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files|*.bmp;*.jpg;*.gif;*.png;*.tif|All files|*.*";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == true)
                SimpleSnake.Food.FoodDirectory = openFileDialog.FileName;
        }

        private void StandartTextureFood_Click(object sender, RoutedEventArgs e)
        {
            SimpleSnake.Food.FoodDirectory = "Orange.png";
        }
    }
}
