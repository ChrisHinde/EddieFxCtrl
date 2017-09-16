using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EddieFxCtrl.Classes
{
    public class EfcShow
    {
        protected EfcMainWindow _MainWin;
        protected Boolean _IsSaved;
        protected String _Filename;
        protected String _Name;
        protected UInt16 _MaxAddress = 0;
        protected EfcUniverse[] _Universes;

        public ObservableCollection<EfcFixture> Fixtures;

        public String Filename
        {
            get => _Filename;
            set
            {
                _Filename = value;
                if (_Name == "")
                    _Name = Path.GetFileNameWithoutExtension(_Filename);
            }
        }
        public String Name
        {
            get => _Name;
            set => _Name = value;
        }

        public UInt16 MaxAddress
        {
            get { return _MaxAddress; }
        }
        public EfcMainWindow MainWindow
        {
            set
            {
                _MainWin = value;
                for (byte i = 0; i < 4; i++)
                {
                    _Universes[i].MainWindow = _MainWin;
                }
            }
        }

        public EfcUniverse[] Universes { get => _Universes; }

        public EfcShow()
        {
            _Filename = "";
            _Name = "";
            _Universes = new EfcUniverse[EfcMainWindow.MAX_UNIVERSES];

            for (byte i = 0; i < 4; i++)
            {
                _Universes[i] = new EfcUniverse(i, null);
            }

            Fixtures = new ObservableCollection<EfcFixture>();

            _MainWin.Log("New Show (N/A)");
        }
        public EfcShow(EfcMainWindow mainWin, String filename)
        {
            _MainWin = mainWin;
            _Filename = filename;
            _Name = Path.GetFileNameWithoutExtension(filename);
            _Universes = new EfcUniverse[EfcMainWindow.MAX_UNIVERSES];

            for (byte i=0; i<4; i++)
            {
                _Universes[i] = new EfcUniverse(i,mainWin);
            }

            Fixtures = new ObservableCollection<EfcFixture>();

            _MainWin.Log("New Show (" + filename + ")");
        }
        public EfcShow( EfcMainWindow mainWin ) : this(mainWin,"")
        {
        }

        public void Modified()
        {
            _IsSaved = false;
        }
        
        public Boolean CheckFreeAddress( UInt16 addr )
        {
            return true;
        }

        public Boolean AddFixture( EfcFixture fixture )
        {
            Modified();

            Fixtures.Add(fixture);
            _Universes[fixture.Universe].Patch(fixture);

           //_MainWin.Updated(this, EfcEventType.PatchChanged, new EfcPatchChangedEventArgs() { EventType = EfcPatch.PatchEvent.ADDED });

            _MainWin.Log("Fixture added to Show (U:" + fixture.Universe.ToString() + "): " + fixture.ToString());

            if ((fixture.Address + fixture.Mode.ChannelCount) > _MaxAddress)
                _MaxAddress = (UInt16)(fixture.Address + (fixture.Mode.ChannelCount - 1));

            _MainWin.Log("New MaxAddress: " + _MaxAddress.ToString());

            return true;
        }

        public void SaveToFile()
        {
            _IsSaved = true;

            XNamespace ns = "http://diversum.se/apps/efc.xsd";
            XDocument sDoc = new XDocument();

            XElement sRootElm = new XElement(ns + "show");
            sRootElm.SetAttributeValue(XNamespace.Xmlns + "efc", "http://diversum.se/apps/efc.xsd");

            sRootElm.SetAttributeValue("version", Properties.Resources.Version);
            //sRootElm.SetAttributeValue("max_id", MaxCompanyID);

            XElement fElm = new XElement(ns + "fixtures");
            XElement elm;

            foreach (EfcFixture fix in Fixtures)
            {
                elm = fix.ToXML(ns);
                fElm.Add(elm);
            }

            // TODO: Save Effects, Scenes etc

            elm = null;

            sRootElm.Add(fElm);

            sDoc.Add(sRootElm);
            sDoc.Save(_Filename);

            _MainWin?.Log("Show saved to: " + _Filename);
        }

        public void SaveToFile( String filename )
        {
            Filename = filename;
            SaveToFile();
        }

        public void OpenFromFile( string filename )
        {
            // TODO: Open show from file
            throw new NotImplementedException();
        }
    }
}
