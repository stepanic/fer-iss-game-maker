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
using ISSProject.Models;

namespace ISSProject
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private readonly GameContext _context;

        public GameWindow(GameContext context)
        {
            _context = context;
            InitializeComponent();
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            // Try to position application to first non-primary monitor
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
                        this.WindowState = WindowState.Maximized;
                    }
                }
            }
        }
    }
}
