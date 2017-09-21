using System;
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
using System.Windows.Shapes;

namespace EddieFxCtrl.Dialogs
{
    /// <summary>
    /// Interaction logic for EfcSimpleDeskWindow.xaml
    /// </summary>
    public partial class EfcSimpleDeskWindow : Window
    {
        protected EfcMainWindow _MainWin;

        public EfcSimpleDeskWindow(EfcMainWindow mainWin)
        {
            _MainWin = mainWin;

            InitializeComponent();

            SimpleDeskCtrl.MainWin = mainWin;
        }
    }
}
