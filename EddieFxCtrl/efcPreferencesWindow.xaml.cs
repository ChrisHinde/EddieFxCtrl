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

namespace EddieFxCtrl
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
    public partial class efcPreferencesWindow : Window
    {
        protected efcMainWindow _MainWin;
        protected efcCompany _CurrentCompany;
        protected int _LastSelectedIndex = -1;

        public efcPreferencesWindow( efcMainWindow mainWin )
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
            //_MainWin.SaveXMLFiles();

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
            /*if (FixtureModesComboBox.SelectedItem != null)
            {
                FixtureModeChannelsDataGrid.ItemsSource = (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels;
                
            }*/
        }

        private void FixtureImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of_dlg = new OpenFileDialog();
            of_dlg.Filter = Properties.Resources.FileExtensions_Images;// "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            
            of_dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (of_dlg.ShowDialog() == true)
                FixtureImageTextBox.Text = of_dlg.FileName;

            FixtureImageTextBox.Focus();
        }

        private void AddFixtureModeButton_Click(object sender, RoutedEventArgs e)
        {
            PromptDialog prompt = new PromptDialog("What do you want to call this new mode?", "New mode");

            if (prompt.ShowDialog() == true)
            {
                int new_id = ++(FixturesTreeView.SelectedItem as efcFixtureModel).MaxModeId;
                (FixturesTreeView.SelectedItem as efcFixtureModel).Modes.Add(new efcFixtureMode(new_id, prompt.Answer));

                FixtureModesComboBox.SelectedIndex = (FixturesTreeView.SelectedItem as efcFixtureModel).Modes.Count - 1;
            }
        }

        private void RemoveFixtureModeButton_Click(object sender, RoutedEventArgs e)
        {
            (FixturesTreeView.SelectedItem as efcFixtureModel).Modes.RemoveAt(FixtureModeChannelsDataGrid.SelectedIndex);
        }

        private void FixtureModeChannelAddButton_Click(object sender, RoutedEventArgs e)
        {
            byte channel = ++(FixtureModesComboBox.SelectedItem as efcFixtureMode).ChannelCount;
            (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels.Add(new efcFixtureChannel( channel, "New Channel" ));
        }

        private void FixtureModeChannelRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels.RemoveAt(FixtureModeChannelsDataGrid.SelectedIndex);
        }

        private void FixtureModeChannelMoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            int index_1 = FixtureModeChannelsDataGrid.SelectedIndex;
            int index_2 = index_1 - 1;

            //Console.WriteLine("I2:" + (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_2].FixtureChannel.ToString() + ";; " + (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_2].Name);

            // Update the channel info (channel #1 as index 0..)
            (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_1].FixtureChannel = (byte)(index_1);
            (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_2].FixtureChannel = (byte)(index_1 + 1);

           // Console.WriteLine("I2:" + (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_2].FixtureChannel.ToString() + ";; " + (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_2].Name);

            // Move the channel in the list
            (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels.Move(index_1, index_2);

          //  Console.WriteLine("I1:" + (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_1].FixtureChannel.ToString() + ";; " + (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_1].Name);
          //  Console.WriteLine("I2:" + (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_2].FixtureChannel.ToString() + ";; " + (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_2].Name);
        }

        private void FixtureModeChannelMoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            int index_1 = FixtureModeChannelsDataGrid.SelectedIndex;
            int index_2 = index_1 + 1;


            // Update the channel info (channel #1 as index 0..)
            /*(FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_1].FixtureChannel = (byte)(index_2 + 1);
            (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels[index_2].FixtureChannel = (byte)(index_2);*/
            (FixtureModeChannelsDataGrid.SelectedItem as efcFixtureChannel).FixtureChannel = (byte)(index_2 + 1);
            (FixtureModeChannelsDataGrid.Items[index_2] as efcFixtureChannel).FixtureChannel = (byte)(index_2);

            // Move the channel in the list
            (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels.Move(index_1, index_2);
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
