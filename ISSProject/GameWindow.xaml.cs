using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ISSProject.Models;

namespace ISSProject {
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window {
        private readonly GameContext _context;
        private DispatcherTimer _dispatcher;

        private readonly IList<Stimulus> _activeStimulus = new List<Stimulus>();
        private int _ticks;

        public GameWindow(GameContext context) {
            _context = context;

            InitializeComponent();
            this.Loaded += GameStarted;


        }

        private void GameStarted(object sender, RoutedEventArgs e) {
            // Position game screen on the secondary monitor
            if (Screen.AllScreens.Length >= 2) {
                var secondary = 0;
                for (int index = 0; index < Screen.AllScreens.Length; index++) {
                    if (Screen.AllScreens[index].Primary) continue;
                    secondary = index;
                    break;
                }

                var screen = Screen.AllScreens[secondary];
                if (screen != null) {
                    var area = screen.WorkingArea;
                    if (!area.IsEmpty) {
                        this.Left = area.Left;
                        this.Top = area.Top;
                        this.Width = area.Width;
                        this.Height = area.Height;
                        this.WindowState = WindowState.Maximized;
                    }
                }
            }

            // Start timer and dispatcher
            _dispatcher = new DispatcherTimer();

            _dispatcher.Tick += IntervalTickEvent;
            _dispatcher.Interval = new TimeSpan(0, 0, 1);

            _dispatcher.Start();
        }

        private void IntervalTickEvent(object sender, EventArgs e) {
            var elapsedSpan = new TimeSpan(0,0, ++_ticks);
            // Update time label
            TimeLabel.Text = elapsedSpan.ToString(@"hh\:mm\:ss");

            // Game ended event
            if (elapsedSpan > _context.Duration) {
                this.Close();
                return;
            }

            #region End Finished Stimulus
            var stimulusEndingOnThisTick = _activeStimulus.Where(x => x.EndTme == elapsedSpan);
            foreach (var stimulus in stimulusEndingOnThisTick) {
                // handle stimulus
                switch (stimulus.Type) {
                    case StimulusType.Image:
                        break;
                    case StimulusType.Sound:
                        break;
                    case StimulusType.Video:
                        break;
                    case StimulusType.Text:
                        break;
                    case StimulusType.Game:
                        break;
                }
            } 
            #endregion

            #region Start New Stimulus
            // Handle new starting stimulus
            var stimulusStartingOnThisTick = _context.StimulusList.Where(x => x.StartTime == elapsedSpan);
            foreach (var stimulus in stimulusStartingOnThisTick) {
                // handle stimulus
                switch (stimulus.Type) {
                    case StimulusType.Image:
                        break;
                    case StimulusType.Sound:
                        break;
                    case StimulusType.Video:
                        break;
                    case StimulusType.Text:
                        break;
                    case StimulusType.Game:
                        break;
                }
            } 
            #endregion


         


        }

      
        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
        }

        private void ButtonCloseAppClick(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
