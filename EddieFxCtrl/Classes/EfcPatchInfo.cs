using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcPatchInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected EfcPatch _Patch;
        protected EfcSoftPatch _SoftPatch;

        public EfcPatchInfo()
        {
            _Patch = null;
            _SoftPatch = null;
        }

        public EfcPatch Patch
        {
            get { return _Patch; }
            set
            {
                _Patch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Patch"));
                _Patch.PropertyChanged += _Patch_PropertyChanged;
            }
        }
        public EfcSoftPatch SoftPatch
        {
            get { return _SoftPatch; }
            set
            {
                _SoftPatch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SoftPatch"));
                _SoftPatch.PropertyChanged += _SoftPatch_PropertyChanged;
            }
        }

        public String Label
        {
            get {
                if (!Used)
                    return @"Not In Use";
                else if (_SoftPatch != null)
                    return "Soft:" + _SoftPatch.In + " => " + _SoftPatch.Out;
                else
                    return _Patch.Fixture.Name;

            }
        }

        public UInt16 Channel { get => HasSoftPatch ? _SoftPatch.In : _Patch.Channel;  }
        public byte Value { get => HasSoftPatch ? (byte)0 : _Patch.Value; }

        public Boolean Used { get => !(_SoftPatch == null && _Patch == null); }
        public Boolean HasSoftPatch { get => _SoftPatch != null; }

        private void _SoftPatch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e); // TODO: Include more info?
        }

        private void _Patch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e); // TODO: Include more info?
        }
    }
}
