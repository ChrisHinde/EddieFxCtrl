using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class efcCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }
        public int FixtureCount { get { return Fixtures.Count; } }
        public ObservableCollection<efcFixtureModel> Fixtures { get; set; }

        public efcCompany(string name = "", int id = -1, string url = "", string logo = "", int fixCount = 0)
        {
            this.Name = name;
            this.Id = id;
            this.Url = url;
            this.Logo = logo;
            

            //this.FixtureCount = fixCount;

            this.Fixtures = new ObservableCollection<efcFixtureModel>();
        }

        public override string ToString()
        {
            return "#" + Id.ToString() + ": " + Name + " (" + Url + ", " + Logo + ")";
        }
    }
}
