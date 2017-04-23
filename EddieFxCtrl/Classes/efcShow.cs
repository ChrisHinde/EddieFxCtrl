using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcShow
    {
        protected EfcMainWindow _MainWin;
        protected Boolean _IsSaved;
        protected String _Filename;
        protected UInt16 _MaxAddress = 0;
        protected EfcUniverse[] _Universes;

        public ObservableCollection<EfcFixture> Fixtures;

        public UInt16 MaxAddress
        {
            get { return _MaxAddress; }
        }

        public EfcShow(EfcMainWindow mainWin, String filename)
        {
            _MainWin = mainWin;
            _Filename = filename;
            _Universes = new EfcUniverse[4];

            for (byte i=0; i<4; i++)
            {
                _Universes[i] = new EfcUniverse(i,mainWin);
            }

            Fixtures = new ObservableCollection<EfcFixture>();

            _MainWin.Log("New Show (" + filename + ")");
        }
        public EfcShow( EfcMainWindow mainWin ) : this(mainWin,"")
        {
        }

        public void Modified()
        {
            _IsSaved = false;
        }
        
        public Boolean CheckFreeAddress( UInt16 addr )
        {
            return true;
        }

        public Boolean AddFixture( EfcFixture fixture )
        {
            Modified();

            Fixtures.Add(fixture);
            _Universes[fixture.Universe].Patch(fixture);

            _MainWin.Log("Fixture added to Show (U:" + fixture.Universe.ToString() + "): " + fixture.ToString());

            if ((fixture.Address + fixture.Mode.ChannelCount) > _MaxAddress)
                _MaxAddress = (UInt16)(fixture.Address + (fixture.Mode.ChannelCount - 1));

            _MainWin.Log("New MaxAddress: " + _MaxAddress.ToString());
            return true;
        }

        public void SaveToFile()
        {
            _IsSaved = true;

                
        }
    }
}
