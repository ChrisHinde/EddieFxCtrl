using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class efcFixtureChannel
    {
        public byte FixtureChannel;
        public string Name;
        public string Description;
        public efcChannelType Type;

        public override string ToString()
        {
            return "#" + ((int)FixtureChannel).ToString() + ": " + Name + " (" + Type.ToString() + ")";
        }
    }
}
