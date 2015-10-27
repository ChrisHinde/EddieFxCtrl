using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class efcFixtureChannel
    {
        public byte FixtureChannel { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public efcChannelType Type { get; set; }

        public efcFixtureChannel( byte channel = 0, string name = "", efcChannelType type = efcChannelType.NoFunction )
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
