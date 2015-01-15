using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ISSProject.Controls;
using ISSProject.Models;

namespace ISSProject
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private readonly GameContext _context;
        private DispatcherTimer _dispatcher;

        private readonly IList<Stimulus> _activeStimulus = new List<Stimulus>();
        private readonly IList<Sound> _activeSounds = new List<Sound>();

        private int _ticks;
        private GoNoGoResult _gameResult = new GoNoGoResult();

        public GameWindow(GameContext context)
        {
            _context = context;

            InitializeComponent();
            this.Loaded += GameStarted;
            

        }

        private void GameStarted(object sender, RoutedEventArgs e)
        {
            // Position game screen on the secondary monitor
            if (Screen.AllScreens.Length >= 2)
            {
                var secondary = 0;
                for (int index = 0; index < Screen.AllScreens.Length; index++)
                {
                    if (Screen.AllScreens[index].Primary) continue;
                    secondary = index;
                    break;
                }

                var screen = Screen.AllScreens[secondary];
                if (screen != null)
                {
                    var area = screen.WorkingArea;
                    if (!area.IsEmpty)
                    {
                        this.Left = area.Left;
                        this.Top = area.Top;
                        this.Width = area.Width;
                        this.Height = area.Height;
                    }
                }
            }
            this.WindowState = WindowState.Maximized;

            // Initialize go-no-go game
            InitializeGoNoGame();
            GoGameContainer.GameResultChanged += (o, args) =>
            {
                _gameResult = args.Result;
                ErrorsScoreLabel.Text = args.Result.Errors + "";
                HitsScoreLabel.Text = args.Result.Hits + "";
                MissesScoreLabel.Text = args.Result.Misses + "";
                ReactionScoreLabel.Text = args.Result.AverageReactionTime + "";
            };

            // Start timer and dispatcher
            _dispatcher = new DispatcherTimer();

            _dispatcher.Tick += IntervalTickEvent;
            _dispatcher.Interval = new TimeSpan(0, 0, 1);

            _dispatcher.Start();
        }

        private void InitializeGoNoGame()
        {
            var gameStimulus = _context.StimulusList.Where(x => x.Type == StimulusType.Game).ToList();

            var parameters = new GoNoGoParameters()
            {
                SectionNum = gameStimulus.Count
            };
            foreach (var section in gameStimulus)
            {
                parameters.SectionDurations.Add(section.EndTme - section.StartTime);
                parameters.SectionSamplings.Add(section.Quantity);
            }
            
            GoGameContainer.Initialize(parameters);
        }

        private void IntervalTickEvent(object sender, EventArgs e)
        {
            var elapsedSpan = new TimeSpan(0, 0, _ticks++);
            // Update time label
            TimeLabel.Text = elapsedSpan.ToString(@"hh\:mm\:ss");

            // Game ended event
            if (elapsedSpan > _context.Duration)
            {
                this.Close();
                return;
            }

            #region End Finished Stimulus
            var stimulusEndingOnThisTick = _activeStimulus.Where(x => x.EndTme == elapsedSpan).ToList();
            foreach (var stimulus in stimulusEndingOnThisTick)
            {
                var guid = stimulus.Guid;
                _activeStimulus.Remove(stimulus);
                // handle stimulus
                switch (stimulus.Type)
                {
                    case StimulusType.Image:
                        ImageContainer.Visibility = Visibility.Collapsed;
                        break;
                    case StimulusType.Sound:
                        var sound = _activeSounds.First(x => x.StimulusGuid == guid);
                        sound.Player.Stop();
                        _activeSounds.Remove(sound);
                        break;
                    case StimulusType.Video:
                        VideoContainer.Visibility = Visibility.Collapsed;
                        VideoContainer.Stop();
                        break;
                    case StimulusType.Text:
                        TextContainer.Visibility = Visibility.Collapsed;
                        break;
                    case StimulusType.Game:
                        GoGameContainer.Visibility = Visibility.Collapsed;
                        GoGameContainer.Stop();
                        break;
                }
            }
            #endregion

            #region Start New Stimulus
            // Handle new starting stimulus
            var stimulusStartingOnThisTick = _context.StimulusList.Where(x => x.StartTime == elapsedSpan).ToList();
            foreach (var stimulus in stimulusStartingOnThisTick)
            {
                _activeStimulus.Add(stimulus);
                // handle stimulus
                switch (stimulus.Type)
                {
                    case StimulusType.Image:
                        ImageContainer.Source = new BitmapImage(new Uri(stimulus.Value));
                        ImageContainer.Visibility = Visibility.Visible;
                        break;
                    case StimulusType.Sound:

                        var mediaPlayer = new MediaPlayer();
                        mediaPlayer.Open(new Uri(stimulus.Value));
                        _activeSounds.Add(new Sound() { Player = mediaPlayer, StimulusGuid = stimulus.Guid });
                        mediaPlayer.Play();

                        break;
                    case StimulusType.Video:
                        VideoContainer.Source = new Uri(stimulus.Value);
                        VideoContainer.Visibility = Visibility.Visible;
                        VideoContainer.Play();

                        break;
                    case StimulusType.Text:
                        TextContainer.Text = stimulus.Value;
                        TextContainer.Visibility = Visibility.Visible;

                        break;
                    case StimulusType.Game:
                        GoGameContainer.Visibility = Visibility.Visible;
                        GoGameContainer.Start();

                        break;
                }
            }
            #endregion





        }


        protected override void OnClosing(CancelEventArgs e)
        {
            _dispatcher.Stop();
            

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            OnGameEnded(new GameResultChangedArgs()
            {
                Result = _gameResult
            });
        }

        private void ButtonCloseAppClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }




        #region Event handling
        public event GameEndedDelegate GameEnded;

        protected virtual void OnGameEnded(GameResultChangedArgs args)
        {
            GameEndedDelegate handler = GameEnded;
            if (handler != null) handler(this, args);
        }

        public delegate void GameEndedDelegate(object sender, GameResultChangedArgs args);
        #endregion
    }
}
