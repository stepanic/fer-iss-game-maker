using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ISSProject.Models;
using TimeLineTool;

namespace ISSProject
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly ObservableCollection<ITimeLineDataItem> MainTimelineData = new ObservableCollection<ITimeLineDataItem>();
        public readonly ObservableCollection<ITimeLineDataItem> SecondaryTimelineData = new ObservableCollection<ITimeLineDataItem>();

        public static readonly DateTime ReferentDate = new DateTime(2000,1,1,0,0,0);

        public MainWindow()
        {
            InitializeComponent();

            var path = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            MainTimelineData.Add(new TimelineData()
            {
                StartTime = ReferentDate,
                EndTime = ReferentDate.AddSeconds(10),
                Value = path + @"\data\video.mp4",
                Type = StimulusType.Video

            });
            MainTimelineData.Add(new TimelineData()
            {
                StartTime = ReferentDate.AddSeconds(10),
                EndTime = ReferentDate.AddSeconds(12),
                Value = "2 sekunde blikakvog teksta pa igra 10 sekundi",
                Type = StimulusType.Text

            });
            MainTimelineData.Add(new TimelineData()
            {
                StartTime = ReferentDate.AddSeconds(12),
                EndTime = ReferentDate.AddSeconds(22),
                Quantity = 10,
                Type = StimulusType.Game
            });
            MainTimelineData.Add(new TimelineData()
            {
                StartTime = ReferentDate.AddSeconds(22),
                EndTime = ReferentDate.AddSeconds(26),
                Value = path + @"\data\image.jpg",
                Type = StimulusType.Image

            });
            MainTimelineData.Add(new TimelineData()
            {
                StartTime = ReferentDate.AddSeconds(26),
                EndTime = ReferentDate.AddSeconds(36),
                Quantity = 10,
                Type = StimulusType.Game
            });
            SecondaryTimelineData.Add(new TimelineData()
            {
                StartTime = ReferentDate.AddSeconds(26),
                EndTime = ReferentDate.AddSeconds(30),
                Value = path + @"\data\audio.wav",
                Type = StimulusType.Sound
            });


            MainTimeline.StartDate = ReferentDate;
            SecondaryTimeline.StartDate = ReferentDate;

            MainTimeline.Items = MainTimelineData;
            SecondaryTimeline.Items = SecondaryTimelineData;


        }

        private void StartTheTestClick(object sender, RoutedEventArgs e)
        {
            var context = new GameContext();

            foreach (var stimuli in MainTimelineData.Union(SecondaryTimelineData))
            {
                var st = stimuli as TimelineData;
                if (st == null) continue;

                // Kill ms information
                st.StartTime = st.StartTime.GetValueOrDefault().AddMilliseconds(-st.StartTime.GetValueOrDefault().Millisecond);
                st.EndTime = st.EndTime.GetValueOrDefault().AddMilliseconds(-st.EndTime.GetValueOrDefault().Millisecond);

                context.StimulusList.Add(new Stimulus()
                {
                    StartTime = (stimuli.StartTime.GetValueOrDefault() - ReferentDate),
                    EndTme = (stimuli.EndTime.GetValueOrDefault() - ReferentDate),
                    Value = st.Value,
                    Type = st.Type,
                    Quantity = st.Quantity,
                });

                
            }
            if (!context.StimulusList.Any())
            {
                return;
            }

            context.Duration = context.StimulusList.OrderByDescending(x => x.EndTme).Select(x => x.EndTme).First();
            var gameWindow = new GameWindow(context);
            gameWindow.GameEnded += (o, args) =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("Hits: " +  args.Result.Hits);
                sb.AppendLine("Errors: " + args.Result.Errors);
                sb.AppendLine("Misses: " + args.Result.Misses);
                sb.AppendLine("Average reaction time: " + args.Result.AverageReactionTime + "ms");

                MessageBox.Show(sb.ToString(), "Results", MessageBoxButton.OK, MessageBoxImage.Information);
                
            };
            gameWindow.Show();
        }

        private void SliderScaleChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MainTimeline.UnitSize = Slider_Scale.Value;
            SecondaryTimeline.UnitSize = Slider_Scale.Value;
        }


        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Double clicked
                var x = ((Border)sender).DataContext as TimelineData;
                
                var result = MessageBox.Show("Are you sure you would like to delete clicked stimuli?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    MainTimelineData.Remove(x);
                    SecondaryTimelineData.Remove(x);
                }
            } 
        }

        private void AddStimuliClick(object sender, RoutedEventArgs e)
        {

            var win = new AddStimuli(this);
            win.ShowDialog();
        }
    }

    public class TimelineData : ITimeLineDataItem
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string Title
        {
            get { return string.Format("({0})", Type); }
        }

        public string Subtitle
        {
            get
            {
                if (Quantity > 0) return "Frequency: " + Quantity;
                return Value;
            }            
        }

        public bool TimelineViewExpanded { get; set; }

        public string Value { get; set; }
        public int Quantity { get; set; }
        public StimulusType Type { get; set; }


    }
}
