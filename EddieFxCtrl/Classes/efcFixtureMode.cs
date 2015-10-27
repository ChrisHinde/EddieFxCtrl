using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class efcFixtureMode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte ChannelCount { get; set; }

        public ObservableCollection<efcFixtureChannel> Channels { get; set; }

        public efcFixtureMode( int id = -1, string name = "" )
        {
            Id = id;
            Name = name;

            Channels = new ObservableCollection<efcFixtureChannel>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
