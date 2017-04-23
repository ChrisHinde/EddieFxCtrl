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

namespace EddieFxCtrl
{
    /// <summary>
    /// Interaction logic for efcFixturesCtrl.xaml
    /// </summary>
    public partial class efcFixturesCtrl : UserControl
    {
        protected EfcMainWindow _MainWin = null;

        public efcFixturesCtrl()
        {
            InitializeComponent();
        }
        public efcFixturesCtrl(EfcMainWindow mainWin)
        {
            DataContext = mainWin;
            _MainWin = mainWin;

            InitializeComponent();

            channelPanel.MainWindow = mainWin;
            fixtureList.ItemsSource = _MainWin.CurrentShow.Fixtures;
        }
        public void SetMainWin(EfcMainWindow mainWin)
        {
            _MainWin = mainWin;
            channelPanel.MainWindow = mainWin;
            fixtureList.ItemsSource = _MainWin.CurrentShow.Fixtures;
        }

        private void AddFixture_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_MainWin != null)
            {
                _MainWin.AddFixture_CanExecute(sender, e);
            }
        }

        public void AddFixture_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EfcFixtureAddWindow fixtureAddDlg = new EfcFixtureAddWindow(_MainWin);
            fixtureAddDlg.ShowDialog();
        }

        private void addFixtureButton_Click(object sender, RoutedEventArgs e)
        {
            EfcFixtureAddWindow fixtureAddDlg = new EfcFixtureAddWindow(_MainWin);
            fixtureAddDlg.ShowDialog();
        }

        private void channelPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
    public static class EfcFixturesCtrlUICommands
    {
        public static readonly RoutedUICommand AddFixture = new RoutedUICommand
            (
                "_Add Fixture",
                "Add Fixture",
                typeof(EfcFixturesCtrlUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.N,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
    }
}
