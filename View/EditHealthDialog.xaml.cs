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
using InitiativeTracker.Data.Stack;
using InitiativeTracker.Data.Util;

namespace InitiativeTracker.View
{
    /// <summary>
    /// Interaction logic for EditHealthDialog.xaml
    /// </summary>
    public partial class EditHealthDialog : Window
    {
        public enum EditType
        {
            Heal,
            Damage
        }

        public static readonly DependencyProperty DieCountProperty = DependencyProperty.Register("DieCount", typeof(int), typeof(EditHealthDialog), new PropertyMetadata(1));
        public static readonly DependencyProperty DieSidesProperty = DependencyProperty.Register("DieSides", typeof(int), typeof(EditHealthDialog), new PropertyMetadata(4));
        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount", typeof(int), typeof(EditHealthDialog));

        public int DieCount
        {
            get { return (int)GetValue(DieCountProperty); }
            set { SetValue(DieCountProperty, value); }
        }

        public int DieSides
        {
            get { return (int)GetValue(DieSidesProperty); }
            set { SetValue(DieSidesProperty, value); }
        }

        public int Amount
        {
            get { return (int)GetValue(AmountProperty); }
            set { SetValue(AmountProperty, value); }
        }

        Entity _target;

        public EditHealthDialog(Entity target)
        {
            InitializeComponent();
            _target = target;
        }

        private void Btn_Roll_OnClick(object sender, RoutedEventArgs e)
        {
            Amount = RngProvider.XdY(DieCount, DieSides);
        }

        private void Btn_HealBy_OnClick(object sender, RoutedEventArgs e)
        {
            _target.CurrentHealth = Math.Max(0, _target.CurrentHealth + Amount);
            Close();
        }

        private void Btn_DamageBy_OnClick(object sender, RoutedEventArgs e)
        {
            _target.CurrentHealth = Math.Max(0, _target.CurrentHealth - Amount);
            Close();
        }

        private void Btn_Set_OnClick(object sender, RoutedEventArgs e)
        {
            _target.CurrentHealth = Math.Max(0, Amount);
            Close();
        }
    }
}