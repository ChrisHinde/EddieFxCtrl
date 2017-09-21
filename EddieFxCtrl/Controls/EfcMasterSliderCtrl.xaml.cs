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
    /// Interaction logic for EfcMasterSliderCtrl.xaml
    /// </summary>
    public partial class EfcMasterSliderCtrl : UserControl, INotifyPropertyChanged
    {
        protected int _Master;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Master
        {
            get { return _Master; }
            set {
                _Master = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Master"));
            }
        }

        public EfcMasterSliderCtrl()
        {
            _Master = 255;
            this.DataContext = this;
                
            InitializeComponent();
        }
    }
}
