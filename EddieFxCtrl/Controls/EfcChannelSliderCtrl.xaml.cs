using EddieFxCtrl.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for EfcChannelSliderCtrl.xaml
    /// </summary>
    public partial class EfcChannelSliderCtrl : UserControl, INotifyPropertyChanged
    {
        protected int _Value;
        protected String _Label;
        protected String _Info;
        protected EfcPatchInfo _PatchInfo;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get => (byte)(_PatchInfo?.Value);
            set
            {
                if (_PatchInfo != null)
                {
                    _PatchInfo.Value = (byte)value;
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
                }
            }
        }

        public String Label
        {
            get => _PatchInfo?.Label;
            set
            {
                //_Label = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
            }
        }
        /*
        public String Info {
            get => _Info;
            set
            {
                _Info = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Info"));
            }
        }*/

        public EfcPatchInfo PatchInfo
        {
            get => _PatchInfo;
            set
            {
                _PatchInfo = value;
                if (_PatchInfo != null)
                {
                    this.DataContext = _PatchInfo;
                    //_PatchInfo.PropertyChanged += _PatchInfo_PropertyChanged;
                }
            }
        }

        private void _PatchInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public EfcChannelSliderCtrl()
        {
            //_Label = "#";
            //_Info = "-";
            //_Value = 0;
            //_PatchInfo = null;

            InitializeComponent();
        }
    }
}
