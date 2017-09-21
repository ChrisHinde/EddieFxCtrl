using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcCompany
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }
        public int FixtureCount { get { return Fixtures.Count; } }
        public ObservableCollection<EfcFixtureModel> Fixtures { get; set; }

        public EfcCompany()
        {
            this.Name = "";
            this.Id = Guid.Empty;
            this.Url = "";
            this.Logo = "";
            this.Fixtures = new ObservableCollection<EfcFixtureModel>();
        }
        public EfcCompany(string name, Guid id, string url = "", string logo = "", int fixCount = 0)
        {
            this.Name = name;
            this.Id = id;
            this.Url = url;
            this.Logo = logo;

            //this.FixtureCount = fixCount;

            this.Fixtures = new ObservableCollection<EfcFixtureModel>();
        }

        public override string ToString()
        {
            return "#" + Id.ToString() + ": " + Name + " (" + Url + ", " + Logo + ")";
        }
    }
}
