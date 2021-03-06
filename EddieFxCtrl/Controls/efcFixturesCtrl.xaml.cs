﻿using System;
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
using EddieFxCtrl;
using EddieFxCtrl.Dialogs;
using EddieFxCtrl.Classes;

namespace EddieFxCtrl.Controls
{
    /// <summary>
    /// Interaction logic for EfcFixturesCtrl.xaml
    /// </summary>
    public partial class EfcFixturesCtrl : UserControl
    {
        protected EfcMainWindow _MainWin = null;

        public EfcFixturesCtrl()
        {
            InitializeComponent();
        }
        public EfcFixturesCtrl(EfcMainWindow mainWin)
        {
            DataContext = mainWin;
            _MainWin = mainWin;
            _MainWin.OnUpdate += _MainWin_OnUpdate;

            InitializeComponent();

            channelPanel.MainWindow = mainWin;
            fixtureList.ItemsSource = EfcMain.CurrentShow.Fixtures;
        }

        private void _MainWin_OnUpdate(object sender, EfcUpdateEventArgs e)
        {
            if (e.Type == EfcEventType.NewShow)
                fixtureList.ItemsSource = EfcMain.CurrentShow.Fixtures;
        }

        public void SetMainWin(EfcMainWindow mainWin)
        {
            _MainWin = mainWin;
            _MainWin.OnUpdate += _MainWin_OnUpdate;

            channelPanel.MainWindow = mainWin;
            fixtureList.ItemsSource = EfcMain.CurrentShow.Fixtures;
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

        private void AddFixtureButton_Click(object sender, RoutedEventArgs e)
        {
            EfcFixtureAddWindow fixtureAddDlg = new EfcFixtureAddWindow(_MainWin);
            fixtureAddDlg.ShowDialog();
        }
        
        public void ShowUpdated()
        {
            fixtureList.ItemsSource = EfcMain.CurrentShow.Fixtures;
            channelPanel.ShowUpdated();
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
