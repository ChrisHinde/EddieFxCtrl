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
            get => _MainWin;
            set
            {
                _MainWin = value;
                for (byte i = 0; i < EfcMainWindow.MAX_UNIVERSES; i++)
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

            for (byte i = 0; i < EfcMainWindow.MAX_UNIVERSES; i++)
            {
                _Universes[i] = new EfcUniverse(i, null);
            }

            Fixtures = new ObservableCollection<EfcFixture>();

            _MainWin.Log("New Show (N/A)");
        }
        public EfcShow(EfcMainWindow mainWin, String filename, Boolean fillUniverses = true)
        {
            _MainWin = mainWin;
            _Filename = filename;
            _Name = Path.GetFileNameWithoutExtension(filename);
            _Universes = new EfcUniverse[EfcMainWindow.MAX_UNIVERSES];

            if (fillUniverses)
            {
                for (byte i = 0; i < EfcMainWindow.MAX_UNIVERSES; i++)
                {
                    _Universes[i] = new EfcUniverse(i, mainWin);
                }
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

        internal EfcFixture GetFixture(Guid guid)
        {
            return Fixtures.First(fix => fix.ID == guid);
        }

        internal EfcFixtureModel GetFixtureModel(Guid guid)
        {
            return _MainWin?.GetFixtureModel(guid);
        }


        public void SaveToFile(String filename)
        {
            Filename = filename;
            SaveToFile();
        }
        public void SaveToFile()
        {
            XNamespace ns = "http://diversum.se/apps/efc.xsd";
            XDocument sDoc = new XDocument();

            XElement sRootElm = new XElement(ns + "show");
            sRootElm.SetAttributeValue(XNamespace.Xmlns + "efc", "http://diversum.se/apps/efc.xsd");

            sRootElm.SetAttributeValue("name", Name);
            sRootElm.SetAttributeValue("version", Properties.Resources.Version);

            /** Add Fixtures to file **/
            XElement currElm = new XElement(ns + "fixtures");
            XElement elm;

            foreach (EfcFixture fix in Fixtures)
            {
                elm = fix.ToXML(ns);
                currElm.Add(elm);
            }
            
            elm = null;

            sRootElm.Add(currElm);

            /** Add universes (and subordinate data, as patches) to file **/
            currElm = new XElement(ns + "universes");

            foreach (EfcUniverse uni in Universes)
            {
                elm = uni.ToXML(ns);
                currElm.Add(elm);
            }

            elm = null;
            
            sRootElm.Add(currElm);

            // TODO: Save Effects, Scenes etc

            sDoc.Add(sRootElm);
            sDoc.Save(_Filename);

            _IsSaved = true;

            _MainWin?.Log("Show saved to: " + _Filename);
        }

        public static EfcShow OpenFromFile( string filename, EfcMainWindow mainWin )
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("Couldn't open '" + filename + "' as a show, the file doesn't exist!");
            }
            EfcShow show = new EfcShow(mainWin, filename);

            XNamespace ns = "http://diversum.se/apps/efc.xsd";

            mainWin?.Log("Loading show from '" + filename + "'");

            XDocument sDoc = XDocument.Load(filename);

            EfcFixture fixture;
            var fList = sDoc.Root.Element(ns + "fixtures").Elements(ns + "fixture");

            foreach (XElement elm in fList)
            {
                fixture = EfcFixture.FromXML(elm, show, ns);
                show.AddFixture(fixture);
            }

            EfcUniverse universe;
            var uList = sDoc.Root.Element(ns + "universes").Elements(ns + "universe");

            foreach (XElement elm in uList)
            {
                universe = EfcUniverse.FromXML(elm, show, ns);
                show.SetUniverse(universe);
            }

            for (ushort i = 0; i < EfcMainWindow.MAX_UNIVERSES; i++)
            {
                if (show._Universes[i] == null)
                    show._Universes[i] = new EfcUniverse(i, mainWin);
            }

            mainWin.Log("Show loaded!");
            // TODO: Scenes etc

            return show;
        }

        private void SetUniverse(EfcUniverse universe)
        {
            _Universes[universe.Universe] = universe;
        }
    }
}
