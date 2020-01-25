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
    /// Interaction logic for GeneratorCountDialog.xaml
    /// </summary>
    public partial class GeneratorCountDialog : Window
    {
        public static readonly DependencyProperty EntityCountProperty = DependencyProperty.Register("EntityCount", typeof(int), typeof(GeneratorCountDialog));

        public int EntityCount
        {
            get { return (int)GetValue(EntityCountProperty); }
            set { SetValue(EntityCountProperty, value); }
        }

        public GeneratorCountDialog()
        {
            InitializeComponent();
        }

        private void Btn_Generate_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
