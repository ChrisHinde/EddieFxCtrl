using EddieFxCtrl.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace EddieFxCtrl.Controls
{
    /// <summary>
    /// Interaction logic for EfcSimpleDeskCtrl.xaml
    /// </summary>
    public partial class EfcSimpleDeskCtrl : UserControl
    {
        private byte _Universe = 1;
        private UInt16 _ChannelStart = 1;
        private UInt16 _ChannelCount = 24;

        private EfcMainWindow _MainWin;
        protected ObservableCollection<EfcPatchInfo> Items;

        public EfcMainWindow MainWin
        {
            get => _MainWin;
            set
            {
                _MainWin = value;
                MasterCtrl.Master = EfcMain.MasterValue;
                _MainWin.OnUpdate += _MainWin_OnUpdate;

                AddChannelSliders();
            }
        }

        private void _MainWin_OnUpdate(object sender, EfcUpdateEventArgs e)
        {
            if (e.Type == EfcEventType.PatchChanged)
                AddChannelSliders();
        }

        public EfcSimpleDeskCtrl()
        {
            this.DataContext = this;

            InitializeComponent();

            MasterCtrl.Master = 255;
            
        }

        protected void AddChannelSliders()
        {
            slidePanel.ItemsSource = new ObservableCollection<EfcPatchInfo>(EfcMain.CurrentShow.Universes[_Universe].PatchInfos.Skip(_ChannelStart).Take(_ChannelCount));
            //slidePanel.Items = Items;
            /*
            for (int c=1; c<=count; c++)
            {
                EfcChannelSliderCtrl slide = new EfcChannelSliderCtrl()
                {
                    PatchInfo = _MainWin.CurrentShow.Universes[_universe].PatchInfos[c]
                };

                slidePanel.Children.Add(slide);
            }*/
        }
    }
}
