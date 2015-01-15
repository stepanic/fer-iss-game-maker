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
using System.Windows.Threading;
using ISSProject.Models;

namespace ISSProject.Controls
{
    /// <summary>
    /// Interaction logic for GoNoGoGameControl.xaml
    /// </summary>
    public partial class GoNoGoGameControl : UserControl
    {
        public bool IsRunning { get; set; }

        private int _currentSection;
        private GoNoGoResult _gameResult;
        private DispatcherTimer _dispatcher;
        private readonly IList<GoNoGoExecutionPlan> _plans = new List<GoNoGoExecutionPlan>();
        private int _ticks;
        private GoNoGoStimuli _activeQuestion;
        private const float GreenStimuliChance = 0.8f;
        public GoNoGoGameControl()
        {
            InitializeComponent();
        }

        public void Initialize(GoNoGoParameters parameters)
        {
            _gameResult = new GoNoGoResult();
            BuildGamePlan(parameters);
            // Start timer and dispatcher
            _dispatcher = new DispatcherTimer();

            _dispatcher.Tick += IntervalTickEvent;
            _dispatcher.Interval = new TimeSpan(0, 0, 1);
        }

        private void IntervalTickEvent(object sender, EventArgs e)
        {
            var elapsedSpan = new TimeSpan(0, 0, _ticks++);
            var question = _plans[_currentSection].Stimulis.FirstOrDefault(x => (int)x.Time.TotalSeconds == (int)elapsedSpan.TotalSeconds);

            if (question != null)
            {
                if (_activeQuestion != null && _activeQuestion.ShouldPress)
                {
                    // Mark unanswered question
                    _gameResult.Misses++;
                    OnGameResultChanged(new GameResultChangedArgs() { Result = _gameResult });

                }
                _activeQuestion = question;
                // Set colors
                if (_activeQuestion.RightButtonInFocus)
                {
                    ColorShape(LeftChoiceButton, "#FFE6E6E6");
                    ColorShape(RightChoiceButton, _activeQuestion.ShouldPress ? "GreenYellow" : "Red");
                }
                else
                {
                    ColorShape(LeftChoiceButton, _activeQuestion.ShouldPress ? "GreenYellow" : "Red");
                    ColorShape(RightChoiceButton, "#FFE6E6E6");
                }
                _activeQuestion.TimeShowed = DateTime.Now;
            }
        }

        private void ColorShape(Shape circle, string colorCode)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString(colorCode);
            circle.Fill = brush;
        }

        public void Start()
        {
            if (_currentSection >= _plans.Count)
            {
                return;
            }
            _ticks = 0;
            IsRunning = true;
            _dispatcher.Start();
        }

        public void Stop()
        {
            if (_activeQuestion != null)
            {
                _activeQuestion = null;
                _gameResult.Misses++;
                OnGameResultChanged(new GameResultChangedArgs() { Result = _gameResult });
            }
            _dispatcher.Stop();
            IsRunning = false;
            _currentSection++;
            
        }

        private void LeftButtonClicked(object sender, MouseButtonEventArgs e)
        {
            if (_activeQuestion != null)
            {
                var elapsedSpan = new TimeSpan(0, 0, _ticks);
                _gameResult.ReactionTimes.Add((int)(DateTime.Now - _activeQuestion.TimeShowed).TotalMilliseconds);

                if (!_activeQuestion.RightButtonInFocus)
                {
                    if (_activeQuestion.ShouldPress)
                        _gameResult.Hits++;
                    else
                        _gameResult.Errors++;
                }
                else
                {
                    _gameResult.Errors++;
                }
                _activeQuestion = null;
                ColorShape(LeftChoiceButton, "#FFE6E6E6");
                ColorShape(RightChoiceButton, "#FFE6E6E6");
                OnGameResultChanged(new GameResultChangedArgs() { Result = _gameResult });
            }
        }
        private void RightButtonClicked(object sender, MouseButtonEventArgs e)
        {
       

            if (_activeQuestion != null)
            {
                var elapsedSpan = new TimeSpan(0, 0, _ticks);
                _gameResult.ReactionTimes.Add((int)(DateTime.Now - _activeQuestion.TimeShowed).TotalMilliseconds);

                if (_activeQuestion.RightButtonInFocus)
                {
                    if (_activeQuestion.ShouldPress)
                        _gameResult.Hits++;
                    else
                        _gameResult.Errors++;
                }
                else
                {
                    _gameResult.Errors++;
                }

                _activeQuestion = null;
                ColorShape(LeftChoiceButton, "#FFE6E6E6");
                ColorShape(RightChoiceButton, "#FFE6E6E6");
                OnGameResultChanged(new GameResultChangedArgs() { Result = _gameResult });
            }
        }
        private void BuildGamePlan(GoNoGoParameters parameters)
        {
            var random = new Random();
            for (int i = 0; i < parameters.SectionNum; i++)
            {
                var stimuliNum = parameters.SectionSamplings[i];
                var duration = parameters.SectionDurations[i];


                var msBeetweenStimuli = (int)(duration.TotalMilliseconds / stimuliNum);

                var plan = new GoNoGoExecutionPlan();
                for (int j = 0; j < duration.TotalMilliseconds; j += msBeetweenStimuli)
                {

                    var randomOffset = random.Next(-msBeetweenStimuli / 3, msBeetweenStimuli / 3);
                    if (randomOffset + msBeetweenStimuli < 0)
                    {
                        randomOffset = 0;
                    }

                    plan.Stimulis.Add(new GoNoGoStimuli()
                    {
                        Time = new TimeSpan(0, 0, 0, 0, j + randomOffset),
                        ShouldPress = !(random.NextDouble() > GreenStimuliChance),
                        RightButtonInFocus = random.NextDouble() > 0.5
                    });
                }
                _plans.Add(plan);

            }
        }

        #region Event handling
        public event GameResultChangedDelegate GameResultChanged;

        protected virtual void OnGameResultChanged(GameResultChangedArgs args)
        {
            GameResultChangedDelegate handler = GameResultChanged;
            if (handler != null) handler(this, args);
        }

        public delegate void GameResultChangedDelegate(object sender, GameResultChangedArgs args);
        #endregion




    }

    public class GameResultChangedArgs
    {
        public GoNoGoResult Result { get; set; }
    }

    public class GoNoGoExecutionPlan
    {
        public IList<GoNoGoStimuli> Stimulis { get; set; }

        public GoNoGoExecutionPlan()
        {
            Stimulis = new List<GoNoGoStimuli>();
        }
    }

    public class GoNoGoStimuli
    {
        public TimeSpan Time { get; set; }
        public bool ShouldPress { get; set; }
        public bool RightButtonInFocus { get; set; }
        public DateTime TimeShowed { get; set; }
    }


}
