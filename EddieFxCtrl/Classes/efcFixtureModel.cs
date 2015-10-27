using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class efcFixtureModel
    {
        public int Id { get; set; }
        public int Manufacturer { get; set; }
        public efcFixtureType Type { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        public byte TotalChannelCount { get; set; }

        public int MaxModeId = -1;
        public ObservableCollection<efcFixtureMode> Modes { get; set; }

        public efcFixtureModel()
        {
            Modes = new ObservableCollection<efcFixtureMode>();
        }

        public override string ToString()
        {
            return "#" + Id.ToString() + ": " + Name + " (" + Type.ToString() + ", " + Image + ", " + Description + ")";
        }
    }
}
