using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcFixtureChannel
    {
        public UInt16 FixtureChannel { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EfcChannelType Type { get; set; }

        public EfcFixtureChannel( UInt16 channel = 0, string name = "", EfcChannelType type = EfcChannelType.NoFunction )
        {
            FixtureChannel = channel;
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return "#" + ((int)FixtureChannel).ToString() + ": " + Name + " (" + Type.ToString() + ")";
        }
    }
}
