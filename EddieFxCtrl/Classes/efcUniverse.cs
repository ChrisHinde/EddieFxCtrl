using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EddieFxCtrl.Classes
{
    public class EfcUniverse : INotifyPropertyChanged
    {
        protected UInt16 _Universe;
        protected String _Label;
        protected UInt16 _MaxAddress = 0;
        protected EfcMainWindow _MainWin;
        protected EfcPatch[] _Patches;
        protected ObservableCollection<EfcSoftPatch> _SoftPatches;

        public EfcPatchInfo[] _PatchInfos;

        public byte[] ChannelValues;

        public event PropertyChangedEventHandler PropertyChanged;

        public EfcPatch[] Patches
        {
            get => _Patches;
        }
        public EfcPatchInfo[] PatchInfos
        {
            get => _PatchInfos;
        }
        public UInt16 Universe
        {
            get => _Universe;
            set => _Universe = value; 
        }

        public EfcMainWindow MainWindow { set => _MainWin = value; }
        public UInt16 MaxAddress { get => _MaxAddress; }

        public String Label { get => _Label; set => _Label = value; }

        public EfcUniverse()
        {
            _Universe = 0;

            ChannelValues = new byte[513];
            _Patches = new EfcPatch[513];
            _PatchInfos = new EfcPatchInfo[513];
            _SoftPatches = new ObservableCollection<EfcSoftPatch>();
        }
        public EfcUniverse( UInt16 universe, EfcMainWindow mainWin )
        {
            _Universe = universe;
            _MainWin = mainWin;
            _Label = "";

            ChannelValues = new byte[513];
            _Patches = new EfcPatch[513];
            _PatchInfos = new EfcPatchInfo[513];
            _SoftPatches = new ObservableCollection<EfcSoftPatch>();
            
            for (UInt16 c=1; c<=512; c++)
            {
                _PatchInfos[c] = new EfcPatchInfo(c);
            }
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
            EfcPatchInfo pi;

            for (UInt16 i=0; i<fixture.Mode.ChannelCount; i++)
            {
                patch = new EfcPatch()
                {
                    Channel = (UInt16)(fixture.Address + i),
                    Fixture = fixture,
                    Universe = this,

                    Enabled = true
                };

                if (_Patches[patch.Channel] != null)
                {
                    RollbackPatch(fixture, i);
                    throw new EfcAddressInUseException(patch.Channel);
                }

                pi = new EfcPatchInfo()
                {
                    Patch = patch
                };

                _Patches[patch.Channel] = patch;
                _PatchInfos[patch.Channel] = pi;

                _MainWin.PatchChanged(this, new EfcPatchChangedEventArgs() { EventType = EfcPatch.PatchEvent.ADDED, Patch = patch });
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
        
        public EfcPatch GetPatch(UInt16 addr, out bool isSoftPatch, out EfcSoftPatch softPatch)
        {
            isSoftPatch = false;
            softPatch = null;

            try
            {
                softPatch = _SoftPatches.Where(sp => sp.In == addr).First();

                isSoftPatch = true;
                addr = softPatch.Out;
            } catch (InvalidOperationException) { /* This Happens if there is no soft patch */ }

           return _Patches[addr];
        }

        public XElement ToXML(XNamespace ns)
        {
            XElement uElm = new XElement(ns + "universe");
            uElm.SetAttributeValue("universe", Universe);

            XElement currElm = new XElement(ns + "patches");
            XElement elm;

            foreach (EfcPatch patch in _Patches)
            {
                if (patch == null) continue;

                elm = patch.ToXML(ns);
                currElm.Add(elm);
            }
            uElm.Add(currElm);

            currElm = new XElement(ns + "softpatches");

            foreach (EfcSoftPatch patch in _SoftPatches)
            {
                elm = patch.ToXML(ns);
                currElm.Add(elm);
            }
            uElm.Add(currElm);

            return uElm;
        }
        public static EfcUniverse FromXML(XElement elm, EfcShow show, XNamespace ns)
        {
            EfcUniverse universe = new EfcUniverse()
            {
                _Universe = Convert.ToByte(elm.Attribute("universe").Value),
                _MainWin = show.MainWindow
            };

            EfcPatch patch;
            EfcPatchInfo pi;
            var pList = elm.Element(ns + "patches").Elements(ns + "patch");

            foreach (XElement pElm in pList)
            {
                patch = EfcPatch.FromXML(pElm, show, universe, ns);
                pi = new EfcPatchInfo()
                {
                    Patch = patch
                };

                universe._Patches[patch.Channel] = patch;
                universe._PatchInfos[patch.Channel] = pi;

                

                if (patch.Channel > universe._MaxAddress)
                    universe._MaxAddress = patch.Channel;
            }

            EfcSoftPatch sp;
            var spList = elm.Element(ns + "softpatches").Elements(ns + "softpatch");

            foreach (XElement pElm in spList)
            {
                sp = EfcSoftPatch.FromXML(pElm, show, universe, ns);

                universe._SoftPatches.Add(sp);

                //universe._PatchInfos[sp.In] = pi;
            }

            show.MainWindow.PatchChanged(universe, new EfcPatchChangedEventArgs() { EventType = EfcPatch.PatchEvent.ADDED, Patch = null });

            return universe;
        }

        /*
public EfcPatchInfo GetPatchInfo( UInt16 addr )
{
   EfcPatchInfo pi = new EfcPatchInfo();

   EfcPatch patch = GetPatch(addr, out bool isSoftPatch, out EfcSoftPatch softPatch);

   if (patch != null)
       pi.Patch = patch;

   if (isSoftPatch)
       pi.SoftPatch = softPatch;


   return pi;
}*/
    }
}
