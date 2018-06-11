using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Plugin
{
    public interface IEfcPlugin
    {
        String Name { get; }
        EfcPluginInfo Info { get; }

        Boolean Enabled { get; set; }
    
        void Initalize();
    }
}
