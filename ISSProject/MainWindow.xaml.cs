using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using ISSProject.Models;

namespace ISSProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var context = CreateGameContextFromTimeline();

            var gameWindow = new GameWindow(context);
            gameWindow.Show();
        }

        private GameContext CreateGameContextFromTimeline()
        {
            var path =
                System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            var context = new GameContext();
            context.Duration = new TimeSpan(0,1,0);
            context.StimulusList.Add(new Stimulus()
            {
                StartTime = new TimeSpan(0,0,0),
                EndTme =  new TimeSpan(0,0,10),
                Value = path + @"\data\video.mp4",
                Type = StimulusType.Video
            });
            context.StimulusList.Add(new Stimulus()
            {
                StartTime = new TimeSpan(0, 0, 10),
                EndTme = new TimeSpan(0, 0, 12),
                Value = "2 sekunde blikakvog teksta pa igra 10 sekundi",
                Type = StimulusType.Text
            });
            context.StimulusList.Add(new Stimulus()
            {
                StartTime = new TimeSpan(0, 0, 12),
                EndTme = new TimeSpan(0, 0, 22),
                Quantity = 10,
                Type = StimulusType.Game
            });
            context.StimulusList.Add(new Stimulus()
            {
                StartTime = new TimeSpan(0, 0, 22),
                EndTme = new TimeSpan(0, 0, 26),
                Value = path +@"\data\image.jpg",
                Type = StimulusType.Image
            });
            context.StimulusList.Add(new Stimulus()
            {
                StartTime = new TimeSpan(0, 0, 26),
                EndTme = new TimeSpan(0, 0, 36),
                Quantity = 10,
                Type = StimulusType.Game
            });
            context.StimulusList.Add(new Stimulus()
            {
                StartTime = new TimeSpan(0, 0, 26),
                EndTme = new TimeSpan(0, 0, 36),
                Value = path + @"\data\audio.wav",
                Type = StimulusType.Sound
            });
            return context;
        }
    }
}
