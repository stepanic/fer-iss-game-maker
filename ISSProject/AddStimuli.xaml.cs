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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ISSProject.Models;

namespace ISSProject
{
    /// <summary>
    /// Interaction logic for AddStimuli.xaml
    /// </summary>
    public partial class AddStimuli : Window
    {
        private readonly MainWindow _mainWindow;

        private StimulusType _type = StimulusType.Video;
        public AddStimuli(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }


        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.Content != null)
            {
                var selected = radioButton.Content.ToString();

                FrequencyBox.Text = "";
                ValueBox.Text = "";
                
                if (selected == "Video")
                {
                    _type = StimulusType.Video;
                    FrequencyBox.IsEnabled = false;
                    ValueBox.IsEnabled = false;
                    BrowseButton.IsEnabled = true;

                } else if (selected == "Game")
                {
                    _type = StimulusType.Game;
                    FrequencyBox.IsEnabled = true;
                    ValueBox.IsEnabled = false;
                    BrowseButton.IsEnabled = false;

                    
                }
                else if (selected == "Image")
                {
                    _type = StimulusType.Image;
                    FrequencyBox.IsEnabled = false;
                    ValueBox.IsEnabled = false;
                    BrowseButton.IsEnabled = true;

                }
                else if (selected == "Text")
                {
                    _type = StimulusType.Text;
                    FrequencyBox.IsEnabled = false;
                    ValueBox.IsEnabled = true;
                    BrowseButton.IsEnabled = false;

                    
                } 
                else if (selected == "Sound")
                {
                    _type = StimulusType.Sound;
                    FrequencyBox.IsEnabled = false;
                    ValueBox.IsEnabled = false;
                    BrowseButton.IsEnabled = true;

                }

            }
        }

        private void BrowseFiles(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            var result = dlg.ShowDialog();
            if (result == true)
            {
                ValueBox.Text = dlg.FileName;
            }

        }


        private void AddStimuliClick(object sender, RoutedEventArgs e)
        {
            int beginsAt = 0;
            int.TryParse(StartTimeBox.Text, out beginsAt);

            int frequency = 0;
            int.TryParse(FrequencyBox.Text, out frequency);

            var referentDate = MainWindow.ReferentDate;
            _mainWindow.MainTimeline.Items = null;
                
                _mainWindow.MainTimelineData.Add(new TimelineData()
            {
                StartTime = referentDate.AddSeconds(beginsAt),
                EndTime = referentDate.AddSeconds(beginsAt + 10),
                Value = ValueBox.Text,
                Quantity = frequency,
                Type = _type
            });
            _mainWindow.MainTimeline.Items = _mainWindow.MainTimelineData;
            this.Close();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
