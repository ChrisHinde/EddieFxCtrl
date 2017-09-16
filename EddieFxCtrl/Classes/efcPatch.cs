using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcPatch : INotifyPropertyChanged
    {
        public enum PatchEvent { ADDED = 1, REMOVED = 2, UPDATED = 3, DISABLED = 4, ENABLED = 5 };

        private UInt16 _Channel;
        private EfcFixture _Fixture;
        private EfcUniverse _Universe;
        private Boolean _Enabled;
        private byte _Value;

        public event PropertyChangedEventHandler PropertyChanged;

        public UInt16 Channel { get => _Channel;
            set
            {
                _Channel = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("Channel"));
            }
        }
        public EfcFixture Fixture { get => _Fixture;
            set
            {
                _Fixture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Fixture"));
            }
        }
        public EfcUniverse Universe { get => _Universe;
            set
            {
                _Universe = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Universe"));
            }
        }
        public Boolean Enabled { get => _Enabled;
            set
            {
                _Enabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Enabled"));
            }
        }

        public byte Value { get => _Value;
            set
            {
                _Value = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }

        public void Enable() { Enabled = true; }
        public void Disable() { Enabled = false; }

        public EfcPatch()
        {
            _Channel = 0;
            _Value = 0;
        }
    }
}
