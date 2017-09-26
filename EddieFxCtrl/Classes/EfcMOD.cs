using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcMOD
    {
        public static String GetMOD()
        {
            Random r = new Random();
            int i = r.Next(0, Properties.Settings.Default.MODList.Count);

            return Properties.Settings.Default.MODList[i];
        }
    }
}
