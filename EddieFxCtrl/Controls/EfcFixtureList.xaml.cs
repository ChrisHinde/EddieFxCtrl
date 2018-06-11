using System;
using System.Collections;
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

namespace EddieFxCtrl.Controls
{
    /// <summary>
    /// Interaction logic for EfcFixtureList.xaml
    /// </summary>
    public partial class EfcFixtureList : UserControl
    {
        private EfcMainWindow _MainWin;

        public EfcMainWindow MainWindow
        {
            get => _MainWin;
            set
            {
                _MainWin = value;
                //  _MainWin.OnUpdate += _OnUpdate;

            }
        }

        public IEnumerable ItemsSource
        {
            get => ItemsCtrl.ItemsSource;
            set => ItemsCtrl.ItemsSource = value;
        }

        public EfcFixtureList()
        {
            InitializeComponent();
           
        }
        
    }
}
