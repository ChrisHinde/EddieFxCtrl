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

namespace EddieFxCtrl
{
    /// <summary>
    /// Interaction logic for efcChannelPanel.xaml
    /// </summary>
    public partial class efcChannelPanel : UserControl, INotifyPropertyChanged
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
                    _MainWin.Log("ChannelPreview " + (_isPaused ? "PAUSED" : "RUNNING"));
                    NotifyPropertyChanged();
                }
            }
        }
        public EfcMainWindow MainWindow
        {
            set { _MainWin = value; }
        }

        public efcChannelPanel()
        {
            InitializeComponent();

            CalculatePages();
        }

        private void channelsPerPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (channelPreview is null)
                return;

            channelPreview.ChannelCount = Convert.ToUInt16(((ComboBoxItem)channelsPerPageComboBox.SelectedItem).Content);

            CalculatePages();
        }

        private void CalculatePages()
        {
            channelPreview.ChannelStart = 1;
            pageComboBox.Items.Clear();

            int pages = 512 / channelPreview.ChannelCount;

            for (int i=0; i < pages; i++)
            {
                pageComboBox.Items.Add(i + 1);
            }

            pageComboBox.SelectedIndex = 0;
        }

        private void pageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int page = pageComboBox.SelectedIndex;

            channelPreview.ChannelStart = (UInt16)(channelPreview.ChannelCount * page + 1);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            IsPaused = !IsPaused;
        }
    }
}
