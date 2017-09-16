using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcSoftPatch : INotifyPropertyChanged
    {
        protected UInt16 _In;
        protected UInt16 _Out;
        protected Boolean _Enabled = true;

        public UInt16 In {
            get { return _In; }
            set
            {
                _In = value;
                PropertyChanged(this, new PropertyChangedEventArgs("In"));
            }
        }
        public UInt16 Out
        {
            get { return _Out; }
            set
            {
                _Out = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Out"));
            }
        }

        public Boolean Enabled
        {
            get { return _Enabled; }
            set
            {
                _Enabled = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Enabled"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
