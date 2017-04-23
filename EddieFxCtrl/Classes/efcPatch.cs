using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcPatch
    {
        private UInt16 _Channel;
        private EfcFixture _Fixture;
        private EfcUniverse _Universe;
        private Boolean _Enabled;

        public UInt16 Channel { get => _Channel; set => _Channel = value; }
        public EfcFixture Fixture { get => _Fixture; set => _Fixture = value; }
        public EfcUniverse Universe { get => _Universe; set => _Universe = value; }
        public Boolean Enabled { get => _Enabled; set => _Enabled = value; }

        public void Enable() { _Enabled = true; }
        public void Disable() { _Enabled = false; }

        public EfcPatch()
        {

        }
    }
}
