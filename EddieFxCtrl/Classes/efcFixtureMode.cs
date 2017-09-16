using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcFixtureMode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte ChannelCount { get; set; }

        public ObservableCollection<EfcFixtureChannel> Channels { get; set; }

        public EfcFixtureMode()
        {
            Id = -1;
            Name = "";

            Channels = new ObservableCollection<EfcFixtureChannel>();
        }
        public EfcFixtureMode( int id, string name = "" )
        {
            Id = id;
            Name = name;

            Channels = new ObservableCollection<EfcFixtureChannel>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
