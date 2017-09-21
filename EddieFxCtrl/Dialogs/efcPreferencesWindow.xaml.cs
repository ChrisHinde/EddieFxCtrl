using EddieFxCtrl;
using EddieFxCtrl.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Globalization;

namespace EddieFxCtrl.Dialogs
{
    public class IsLastModeChannelConverter : IValueConverter
    {
        public static DataGrid ModesListBox;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ModesListBox == null)
                return false;

            return (int)value == ModesListBox.Items.Count -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for efcPreferencesWindow.xaml
    /// </summary>
    public partial class EfcPreferencesWindow : Window
    {
        protected EfcMainWindow _MainWin;
        protected EfcCompany _CurrentCompany;
        protected int _LastSelectedIndex = -1;

        protected bool _FixturesChanged = false;
        protected bool _CompaniesChanged = false;

        public EfcPreferencesWindow( EfcMainWindow mainWin )
        {
            InitializeComponent();

            DataContext = mainWin;

            _MainWin = mainWin;

           // DataContext = mainWin;
            //CompaniesListBox.DataContext = mainWin;
            CompaniesListBox.ItemsSource = mainWin.Companies;
            FixturesTreeView.ItemsSource = mainWin.Companies;

            IsLastModeChannelConverter.ModesListBox = FixtureModeChannelsDataGrid;

            /*foreach ( efcCompany comp in mainWin.Companies )
            {
                //CompaniesListBox.Items.Add("N:" + comp.Name); 
                FixturesTreeView.Items.Add( new TreeViewItem() { Header = comp.Name, Uid = "C" + comp.Id.ToString() } );
            }/*
            foreach ( efcFixtureModel fix in mainWin.FixtureModels )
            {
                
            }*/

            string[] ports = SerialPort.GetPortNames();
            foreach ( string port in ports )
            {
                ComEKBDInputPrefCmbBox.Items.Add(port);
            }

            ComEKBDInputPrefCmbBox.Text = Properties.Settings.Default.EddieKBD_COM;
        }

        private void OkCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void OkCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavePreferences();

            DialogResult = true;
            Close();
        }

        private void SavePreferences()
        {
            _MainWin.Log("CompCh:" + _CompaniesChanged);
            _MainWin.Log("FixCh:" + _FixturesChanged);

            if (_CompaniesChanged)
                _MainWin.SaveCompanyData();
            if (_FixturesChanged)
                _MainWin.SaveFixtureData();

            Properties.Settings.Default.EddieKBD_COM = ComEKBDInputPrefCmbBox.Text;
            Properties.Settings.Default.MasterMode = (uint)MasterModeCombo.SelectedIndex;
            Properties.Settings.Default.BlackoutMode = (uint)BlackoutMode.SelectedIndex;
            Properties.Settings.Default.BlackoutDefault = BlackoutDefault.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void FixturesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            /*if (FixturesTreeView.SelectedItem is efcFixtureModel)
            {
                FixtureModesComboBox.ItemsSource = (FixturesTreeView.SelectedItem as efcFixtureModel).Modes;
            }*/
        }

        private void FixtureModesCombBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _FixturesChanged = true;
            /*if (FixtureModesComboBox.SelectedItem != null)
            {
                FixtureModeChannelsDataGrid.ItemsSource = (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels;
                
            }*/
        }

        private void FixtureImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of_dlg = new OpenFileDialog()
            {
                Filter = Properties.Resources.FileExtensions_Images,// "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";

                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (of_dlg.ShowDialog() == true)
            {
                FixtureImageTextBox.Text = of_dlg.FileName;
                _FixturesChanged = true;
            }

            FixtureImageTextBox.Focus();
        }

        private void AddFixtureModeButton_Click(object sender, RoutedEventArgs e)
        {
            PromptDialog prompt = new PromptDialog("What do you want to call this new mode?", "New mode");

            if (prompt.ShowDialog() == true)
            {
                int new_id = ++(FixturesTreeView.SelectedItem as EfcFixtureModel).MaxModeId;
                (FixturesTreeView.SelectedItem as EfcFixtureModel).Modes.Add(new EfcFixtureMode(new_id, prompt.Answer));

                FixtureModesComboBox.SelectedIndex = (FixturesTreeView.SelectedItem as EfcFixtureModel).Modes.Count - 1;

                _FixturesChanged = true;
            }
        }

        private void RemoveFixtureModeButton_Click(object sender, RoutedEventArgs e)
        {
            (FixturesTreeView.SelectedItem as EfcFixtureModel).Modes.RemoveAt(FixtureModeChannelsDataGrid.SelectedIndex);

            _FixturesChanged = true;
        }

        private void FixtureModeChannelAddButton_Click(object sender, RoutedEventArgs e)
        {
            byte channel = ++(FixtureModesComboBox.SelectedItem as EfcFixtureMode).ChannelCount;
            (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels.Add(new EfcFixtureChannel( channel, "New Channel" ));

            _FixturesChanged = true;
        }

        private void FixtureModeChannelRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels.RemoveAt(FixtureModeChannelsDataGrid.SelectedIndex);

            _FixturesChanged = true;
        }

        private void FixtureModeChannelMoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            int index_1 = FixtureModeChannelsDataGrid.SelectedIndex;
            int index_2 = index_1 - 1;

            //Console.WriteLine("I2:" + (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_2].FixtureChannel.ToString() + ";; " + (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_2].Name);

            // Update the channel info (channel #1 as index 0..)
            (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_1].FixtureChannel = (byte)(index_1);
            (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_2].FixtureChannel = (byte)(index_1 + 1);

           // Console.WriteLine("I2:" + (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_2].FixtureChannel.ToString() + ";; " + (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_2].Name);

            // Move the channel in the list
            (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels.Move(index_1, index_2);

            _FixturesChanged = true;

            //  Console.WriteLine("I1:" + (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_1].FixtureChannel.ToString() + ";; " + (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_1].Name);
            //  Console.WriteLine("I2:" + (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_2].FixtureChannel.ToString() + ";; " + (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_2].Name);
        }

        private void FixtureModeChannelMoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            int index_1 = FixtureModeChannelsDataGrid.SelectedIndex;
            int index_2 = index_1 + 1;


            // Update the channel info (channel #1 as index 0..)
            /*(FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_1].FixtureChannel = (byte)(index_2 + 1);
            (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels[index_2].FixtureChannel = (byte)(index_2);*/
            (FixtureModeChannelsDataGrid.SelectedItem as EfcFixtureChannel).FixtureChannel = (byte)(index_2 + 1);
            (FixtureModeChannelsDataGrid.Items[index_2] as EfcFixtureChannel).FixtureChannel = (byte)(index_2);

            // Move the channel in the list
            (FixtureModesComboBox.SelectedItem as EfcFixtureMode).Channels.Move(index_1, index_2);

            _FixturesChanged = true;
        }

        private void AddCompanyBtn_Click(object sender, RoutedEventArgs e)
        {
            EfcCompany company = new EfcCompany()
            {
                Id = Guid.NewGuid(),
                Name = "Unnamed Company"
            };
            _MainWin.Companies.Add(company);
            //_MainWin.MaxCompanyID = company.Id;

            _CompaniesChanged = true;
        }

        private void DelCompanyBtn_Click(object sender, RoutedEventArgs e)
        {
            EfcCompany company = (EfcCompany)CompaniesListBox.SelectedItem;

            if (Xceed.Wpf.Toolkit.MessageBox.Show("Do you really want to remove " + company.Name + "? This will also remove related fixtures!", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            _MainWin.Companies.Remove(company);
            company = null;
            
            _FixturesChanged = true;
            _CompaniesChanged = true;
        }

        private void AddFixtureBtn_Click(object sender, RoutedEventArgs e)
        {
            EfcCompany company = null;

            if (FixturesTreeView.SelectedItem is EfcFixtureModel)
            {
                company = (FixturesTreeView.SelectedItem as EfcFixtureModel).Company;
                _MainWin.Log("Selected Item Is FixtureModel");
            }
            else if (FixturesTreeView.SelectedItem is EfcCompany)
            {
                company = FixturesTreeView.SelectedItem as EfcCompany;
                _MainWin.Log("Selected Item Is Company");
            } else
            {
                _MainWin.Log("Selected Item Is " + FixturesTreeView.SelectedItem.GetType().ToString());
            }

            //_MainWin.Log("MaxFixtureID:" + _MainWin.MaxFixtureID.ToString());

            if (company == null)
                return;

            EfcFixtureModel fixture = new EfcFixtureModel()
            {
                Id = Guid.NewGuid(),
                Name = "Unnamed fixture",
                Company = company,
                Manufacturer = company.Id,
                Type = EfcFixtureType.Par_can
            };
            company.Fixtures.Add(fixture);
            _MainWin.FixtureModels.Add(fixture);

            //_MainWin.MaxFixtureID = fixture.Id;

            _FixturesChanged = true;
        }

        private void DelFixtureBtn_Click(object sender, RoutedEventArgs e)
        {
            EfcFixtureModel fixture = FixturesTreeView.SelectedItem as EfcFixtureModel;

            if (Xceed.Wpf.Toolkit.MessageBox.Show("Do you really want to remove " + fixture.Company.Name + " -> " + fixture.Name + "?", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            
            _MainWin.FixtureModels.Remove(fixture);
            fixture.Company.Fixtures.Remove(fixture);
            fixture = null;

            _FixturesChanged = true;
        }

        private void FixtureModeChannelsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _FixturesChanged = true;
        }   
        
        private void FixtureInfoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            _FixturesChanged = true;
        }

        private void CompanyInfoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            _CompaniesChanged = true;
        }
    }
    public static class efcPrefUICommands
    {
        public static readonly RoutedUICommand Ok = new RoutedUICommand
            (
                "_Ok",
                "Ok",
                typeof(efcPrefUICommands),
                new InputGestureCollection()
                {
                }
            );
    }
}
