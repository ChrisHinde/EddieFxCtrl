using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcUniverse
    {
        protected UInt16 _Universe;
        protected UInt16 _MaxAddress = 0;
        protected EfcMainWindow _MainWin;
        protected EfcPatch[] _Patches;
        protected ObservableCollection<EfcSoftPatch> _SoftPatches;

        public byte[] ChannelValues;

        public UInt16 MaxAddress { get => _MaxAddress; }

        public EfcUniverse( UInt16 universe, EfcMainWindow mainWin )
        {
            _Universe = universe;
            _MainWin = mainWin;

            ChannelValues = new byte[512];
            _Patches = new EfcPatch[512];
            _SoftPatches = new ObservableCollection<EfcSoftPatch>();
        }

        public void AddSoftPatch( UInt16 inAddr, UInt16 outAddr )
        {
            foreach (EfcSoftPatch sp in _SoftPatches)
            {
                if (sp.In == inAddr)
                    throw new EfcAddressInUseException(inAddr);
            }

            EfcSoftPatch softPatch = new EfcSoftPatch()
            {
                In = inAddr,
                Out = outAddr
            };

            _SoftPatches.Add(softPatch);
        }

        public void Patch( EfcFixture fixture )
        {
            UInt16 channel = fixture.Address;
            EfcPatch patch;

            for (UInt16 i=0; i<fixture.Mode.ChannelCount; i++)
            {
                patch = new EfcPatch()
                {
                    Channel = (UInt16)(fixture.Address + i)
                };

                if (_Patches[patch.Channel] != null)
                {
                    RollbackPatch(fixture, i);
                    throw new EfcAddressInUseException(patch.Channel);
                }

                _Patches[patch.Channel] = patch;
            }
        }
        protected void RollbackPatch(EfcFixture fixture, UInt16 pos)
        {
            for (UInt16 i=fixture.Address; i < fixture.Address + pos; i++)
            {
                _Patches[i] = null;
            }
        }

        public Boolean CheckFreeAddress(UInt16 addr)
        {
            return true;
        }
    }
}
