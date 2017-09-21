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

        protected UInt16 _Channel;
        protected EfcPatch _Patch;
        protected EfcSoftPatch _SoftPatch;
        protected byte _Value;
        //protected Boolean _IsUsed = false;

        public EfcPatchInfo()
        {
            _Patch = null;
            _SoftPatch = null;
            _Channel = 0;
            _Value = 0;
        }
        public EfcPatchInfo(UInt16 channel)
        {
            _Channel = channel;
            _Value = 0;
        }

        public EfcPatch Patch
        {
            get { return _Patch; }
            set
            {
               _Patch = value;
               if (_Patch != null)
                    _Channel = _Patch.Channel;

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
                if (_SoftPatch != null)
                    _Channel = _SoftPatch.In;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SoftPatch"));
                _SoftPatch.PropertyChanged += _SoftPatch_PropertyChanged;
            }
        }

        public String Label
        {
            get
            {
                 if (!Used)
                     return @"Not In Use";
                 else if (_SoftPatch != null)
                     return "Soft:" + _SoftPatch.In + " => " + _SoftPatch.Out;
                 else
                     return _Patch.Fixture.Name;
              //  return "FUBAR";
            }
        }

        public UInt16 Channel { get => _Channel; set => _Channel = value; /*HasSoftPatch ? _SoftPatch.In : _Patch.Channel; */ }
        public byte Value
        {
            get => Used ? (HasSoftPatch ? _Value : _Patch.Value) : _Value;
            set
            {
                if ( (!HasSoftPatch) && _Patch != null ) _Patch.Value = value;
                _Value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }

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
