using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcFixtureModel
    {
        public Guid Id { get; set; }
        public Guid Manufacturer { get; set; }
        public EfcFixtureType Type { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public EfcCompany Company { get; set; }

        public byte TotalChannelCount { get; set; }

        public int MaxModeId = -1;
        public ObservableCollection<EfcFixtureMode> Modes { get; set; }

        public EfcFixtureModel()
        {
            Modes = new ObservableCollection<EfcFixtureMode>();
        }

        public EfcFixtureMode GetMode(int id)
        {
            return Modes.First(mode => mode.Id == id);
        }

        public override string ToString()
        {
            return "#" + Id.ToString() + ": " + Name + " (" + Type.ToString() + ", " + Image + ", " + Description + ")";
        }
    }
}
