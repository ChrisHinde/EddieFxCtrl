using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcFixture
    {
        private String _Name;
        private UInt16 _Address;
        private byte _Universe;
        private EfcFixtureModel _Fixture;
        private efcFixtureMode _Mode;
        private String _Note;

        public String Name { get => _Name; set => _Name = value; }
        public UInt16 Address { get => _Address; set => _Address = value; }
        public byte Universe { get => _Universe; set => _Universe = value; }
        public efcFixtureMode Mode { get => _Mode; set => _Mode = value; }
        public EfcFixtureModel Fixture { get => _Fixture; set => _Fixture = value; }
        public String Note { get => _Note; set => _Note = value; }

        public override string ToString()
        {
            return "#" + _Address.ToString() + ": " + _Name + " (U" + _Universe.ToString() + ", " + _Fixture.Name + ", " + _Mode.ToString() + ")";
        }
    }
}
