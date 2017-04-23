using EddieFxCtrl.Classes;
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

namespace EddieFxCtrl
{
    /// <summary>
    /// Interaction logic for efcFixtureAddWindow.xaml
    /// </summary>
    public partial class EfcFixtureAddWindow : Window
    {
        protected EfcMainWindow _MainWin;
        protected EfcFixtureModel _SelectedFixture;
        protected String _DefaultFixtureDesc;

        public EfcFixtureModel SelectedFixture
        {
            get { return _SelectedFixture;  }
        }

        public EfcFixtureAddWindow( EfcMainWindow mainWin )
        {
            InitializeComponent();
            _SelectedFixture = null;
            _DefaultFixtureDesc = fixtureDescriptionTextBlock.Text;

            //DataContext = mainWin;
            _MainWin = mainWin;
            FixturesTreeView.ItemsSource = mainWin.Companies;

            fixtureAddressUpDown.Value = mainWin.CurrentShow.MaxAddress + 1;
            fixtureCountUpDown.Maximum = 512 - mainWin.CurrentShow.MaxAddress;
        }

        private void FixturesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (FixturesTreeView.SelectedItem is EfcFixtureModel)
            {
                _SelectedFixture = (EfcFixtureModel)FixturesTreeView.SelectedItem;

                fixtureInfoGrid.IsEnabled = true;
                fixtureNameTextBox.Text = _SelectedFixture.Name;
                fixtureDescriptionTextBlock.Text = _SelectedFixture.Description;
                
                FixtureModeComboBox.ItemsSource = _SelectedFixture.Modes;
                FixtureModeComboBox.SelectedIndex = 0;
            } else {
                _SelectedFixture = null;

                fixtureInfoGrid.IsEnabled = false;
                fixtureNameTextBox.Text = "";
                fixtureDescriptionTextBlock.Text = _DefaultFixtureDesc;

                FixtureModeComboBox.ItemsSource = null;
            }
        }

        private void OkCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _SelectedFixture != null;
        }
        private void OkCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddFixture();

            DialogResult = true;
            Close();
        }

        private void AddFixture()
        {
            String name;
            UInt16 adress;
            byte universe;
            efcFixtureMode mode;
            String note;
            EfcFixture fixture;

            universe = (byte)(fixtureUniverseComboBox.SelectedIndex + 1);

            for (int i = 0; i < fixtureCountUpDown.Value; i++)
            {
                name = fixtureNameTextBox.Text;
                mode = (efcFixtureMode)FixtureModeComboBox.SelectedItem;
                note = fixtureNoteTextBox.Text;
                adress = (UInt16)(fixtureAddressUpDown.Value + i * (mode.ChannelCount + fixtureAdressGapUpDown.Value));

                if (i > 0)
                {
                    name += String.Format(".{0:000}", i);
                }

                fixture = new EfcFixture()
                {
                    Name = name,
                    Address = adress,
                    Universe = universe,
                    Mode = mode,
                    Note = note,
                    Fixture = _SelectedFixture
                };

                _MainWin.CurrentShow.AddFixture(fixture);
            }
        }


        private void CalculateAdressRange()
        {
            
        }

        private void OptionsChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CalculateAdressRange();
        }

        private void OptionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateAdressRange();
        }
    }

    public static class efcFixtureAddUICommands
    {
        public static readonly RoutedUICommand Ok = new RoutedUICommand
            (
                "_Ok",
                "Ok",
                typeof(efcFixtureAddUICommands),
                new InputGestureCollection()
                {
                }
            );
    }
}
