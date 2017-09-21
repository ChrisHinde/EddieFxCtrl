using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace EddieFxCtrl.Controls
{
    /// <summary>
    /// Interaction logic for efcChannelPanel.xaml
    /// </summary>
    public partial class EfcChannelPanel : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private EfcMainWindow _MainWin;
        private bool _isPaused = false;
        
        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                if (value != _isPaused)
                {
                    _isPaused = value;
                    channelView.IsPaused = value;
                    _MainWin.Log("ChannelView " + (_isPaused ? "PAUSED" : "RUNNING"));
                    NotifyPropertyChanged();
                }
            }
        }
        public EfcMainWindow MainWindow
        {
            get => _MainWin;
            set {
                _MainWin = value;
                channelView.MainWindow = value;
            }
        }

        public EfcChannelPanel()
        {
            InitializeComponent();

            //channelPreview.MainWindow = _MainWin;
            pageComboBox.SelectedIndex = 0;

            CalculatePages();
        }

        private void ChannelsPerPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (channelView is null)
                return;

            channelView.ChannelCount = Convert.ToUInt16(((ComboBoxItem)channelsPerPageComboBox.SelectedItem).Content);

            CalculatePages();
        }

        private void CalculatePages()
        {
            channelView.ChannelStart = 1;
            pageComboBox.Items.Clear();

            int pages = 512 / channelView.ChannelCount;

            for (int i=0; i < pages; i++)
            {
                pageComboBox.Items.Add(i + 1);
            }

            pageComboBox.SelectedIndex = 0;
        }
        public void ShowUpdated()
        {
         //   fixtureList.ItemsSource = _MainWin.CurrentShow.Fixtures;
        }

        private void PageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int page = pageComboBox.SelectedIndex;
            if (page == -1)
                page = 0;

            channelView.ChannelStart = (UInt16)(channelView.ChannelCount * page + 1);
        }

        private void NotifyPropertyChanged( String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            IsPaused = !IsPaused;
        }
    }
}
