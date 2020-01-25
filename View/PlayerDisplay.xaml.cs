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

namespace InitiativeTracker.View
{
    /// <summary>
    /// Interaction logic for PlayerDisplay.xaml
    /// </summary>
    public partial class PlayerDisplay : Window
    {
        bool _isFullscreen;

        public PlayerDisplay()
        {
            InitializeComponent();
        }

        public void ToggleFullscreen()
        {
            if (_isFullscreen)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }

            _isFullscreen = !_isFullscreen;
        }

        private void PlayerDisplay_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                ToggleFullscreen();
            }
        }
    }
}
