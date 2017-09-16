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
    /// Interaction logic for EfcChannelView.xaml
    /// </summary>
    public partial class EfcChannelView : UserControl
    {
        private EfcMainWindow _MainWin;

        protected UInt16 _ChannelCount = 32;
        protected UInt16 _ChannelStart = 1;
        protected byte _Universe = 1;
        protected bool _Paused = false;
        protected bool _DisplayPercentage = true;

        public UInt16 ChannelCount
        {
            get { return _ChannelCount; }
            set {
                _ChannelCount = value;
                Update();
            }
        }
        public UInt16 ChannelStart
        {
            get { return _ChannelStart; }
            set
            {
                if (value > 512)
                    throw new Exception("The start channel value is too high (>512)! This shouldn't happen!");
                _ChannelStart = value;
                Update();
            }
        }
        public byte Universe
        {
            get { return _Universe; }
            set {
                _Universe = value;
                Update();
            }
        }

        public bool DisplayPercantage { get => _DisplayPercentage; set => _DisplayPercentage = value; }

        public bool IsPaused { get => _Paused; set => _Paused = value; }
        public void Pause() { _Paused = true; }
        public void Unpause() { _Paused = false; }

        public ObservableCollection<EfcPatchInfo> Items;

        public EfcMainWindow MainWindow
        {
            set
            {
                _MainWin = value;
                _MainWin.OnUpdate += _OnUpdate;

                //_MainWin.Log("Setting source for EfcChannelView to Patches");

                //ItemsCtrl.ItemsSource =  _MainWin.CurrentShow.Universes[_Universe].Patches;
                Update();
            }
        }

        public EfcChannelView()
        {
            InitializeComponent();
            Items = new ObservableCollection<EfcPatchInfo>();
            ItemsCtrl.ItemsSource = Items;

        }

        private void _OnUpdate(object sender, EfcUpdateEventArgs e)
        {
            if (_Paused || e.Args == null)
                return;

            switch (e.Type)
            {
                case EfcEventType.ChannelChanged:
                    if (((EfcChannelChangedEventArgs)e.Args).Channel > 0 &&
                            !InRange(((EfcChannelChangedEventArgs)e.Args).Channel))
                        return;
                    break;
                case EfcEventType.PatchChanged:
                    if (e.Args.GetType() != typeof(EfcPatchChangedEventArgs) || (((EfcPatchChangedEventArgs)e.Args).Patch == null))
                        return;
                    if (InRange(((EfcPatchChangedEventArgs)e.Args).Patch.Channel))
                    {
                        Update();
                        return;
                    }
                    break;
                case EfcEventType.SoftPatchChanged:
                    if (!InRange(((EfcSoftPatchChangedEventArgs)e.Args).ChannelIn) &&
                         !InRange(((EfcSoftPatchChangedEventArgs)e.Args).ChannelOut))
                        return;
                    break;
            }

            //var thread = new Thread(Draw);
            //thread.Start();
            //Update();
        }

        private void Update()
        {
            _MainWin?.Log("Channel View update!");

            if (_Paused || _MainWin == null) return;

            Items.Clear();

            for (UInt16 i = _ChannelStart; i < _ChannelStart + _ChannelCount; i++)
            {
                Items.Add(_MainWin.CurrentShow.Universes[_Universe].PatchInfos[i]);
            }
        }

        private bool InRange(ushort channel)
        {
            return (channel >= _ChannelStart) && (channel < _ChannelStart + _ChannelCount);
        }
    }
}
