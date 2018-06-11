using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EddieFxCtrl.Classes
{
    public class EfcFixture
    {
        private Guid _ID;
        private String _Name;
        private UInt16 _Address;
        private byte _Universe;
        private EfcFixtureModel _Fixture;
        private EfcFixtureMode _Mode;
        private String _Note;

        public Guid ID { get => _ID;  }
        public String Name { get => _Name; set => _Name = value; }
        public UInt16 Address { get => _Address; set => _Address = value; }
        public byte Universe { get => _Universe; set => _Universe = value; }
        public EfcFixtureMode Mode { get => _Mode; set => _Mode = value; }
        public EfcFixtureModel Fixture { get => _Fixture; set => _Fixture = value; }
        public String Note { get => _Note; set => _Note = value; }

        public override string ToString()
        {
            return "#" + _Address.ToString() + ": " + _Name + " (U" + _Universe.ToString() + ", " + _Fixture.Name + ", " + _Mode.ToString() + ")";
        }

        public EfcFixture()
        {
            _ID = Guid.NewGuid();
        }
        public EfcFixture( Guid id )
        {
            _ID = id;
        }

        public XElement ToXML(XNamespace ns)
        {
            XElement elm = new XElement(ns + "fixture");

            elm.SetAttributeValue("id", _ID);
            elm.SetAttributeValue("universe", _Universe);
            elm.SetAttributeValue("address", _Address);
            elm.SetAttributeValue("mode", _Mode.Id);

            elm.Add(new XElement(ns + "name", _Name));
            elm.Add(new XElement(ns + "note", _Note));

            elm.Add(new XElement(ns + "fixtureModel", _Fixture.Id)); // TODO: Include more info? Use GUID?

            return elm;
        }
        public static EfcFixture FromXML(XElement elm, EfcShow show, XNamespace ns)
        {
            EfcFixture fixture = new EfcFixture()
            {
                _ID = new Guid(elm.Attribute("id").Value),
                _Universe = Convert.ToByte(elm.Attribute("universe").Value),
                _Address = Convert.ToUInt16(elm.Attribute("address").Value),
                //_Mode = new EfcFixtureMode(Convert.ToInt32(elm.Attribute("mode").Value)),

                _Name = elm.Element(ns + "name").Value,
                _Note = elm.Element(ns + "note").Value,

            };
            try
            {
                fixture._Fixture = show.GetFixtureModel(new Guid(elm.Element(ns + "fixtureModel").Value));
            }
            catch (Exception e)
            {
                
            }

            fixture._Mode = fixture._Fixture.GetMode(Convert.ToInt32(elm.Attribute("mode").Value));

            return fixture;
        }
    }
}
