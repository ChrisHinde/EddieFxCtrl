using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Plugin
{
    public enum EfcPluginType
    {
        Input = 0,
        Output = 1,

        DMX = 4,
        MIDI = 8,
        HTTP = 16,
        MQTT = 32,

        Effect = 128
    };

    public class EfcPluginInfo
    {
        public String Name;
        public String Version;
        public float NumericVersion;

        public EfcPluginType PluginType;
    }
}
