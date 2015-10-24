using EddieFxCtrl.Classes;
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

namespace EddieFxCtrl
{
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
            Properties.Settings.Default.Save();
        }

        private void FixturesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (FixturesTreeView.SelectedItem is efcFixtureModel)
            {
                FixtureModesComboBox.ItemsSource = (FixturesTreeView.SelectedItem as efcFixtureModel).Modes;
            }
        }

        private void FixtureModesCombBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FixtureModesComboBox.SelectedItem != null)
            {
                FixtureModeChannelsListBox.ItemsSource = (FixtureModesComboBox.SelectedItem as efcFixtureMode).Channels;
                FixtureModeChannelsListBox.DataContext = new efcFixtureChannel();
                
            }
        }
    }
    public static class efcPrefUICommands
    {
        public static readonly RoutedUICommand Ok = new RoutedUICommand
            (
                "_Ok",
                "Ok",
                typeof(efcUICommands),
                new InputGestureCollection()
                {
                }
            );
    }
}
