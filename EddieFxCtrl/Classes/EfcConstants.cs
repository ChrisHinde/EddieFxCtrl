using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcConstants
    {
        public const ushort DMX_UNIVERSE_SIZE = 512;
        public const ushort DMX_MIN_SLOT_NUMBER = 1;
        public const ushort DMX_MAX_SLOT_NUMBER = DMX_UNIVERSE_SIZE;
        public const byte DMX_MIN_SLOT_VALUE = 0x00;
        public const byte DMX_MAX_SLOT_VALUE = 0xFF;

        public const byte DMX512_START_CODE = 0;
    }
}
