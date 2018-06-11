using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfcPlugin_Eurolite_UsbPro
{
    public class Eurolite_Constants
    {
        public const ushort URB_TIMEOUT_MS = 500;
        public const byte DMX_LABEL = 6;
        public const byte START_OF_MESSAGE = 0x7e;
        public const byte END_OF_MESSAGE = 0xe7;
        public const byte ENDPOINT = 0x02;
        public const int BAUDRATE = 250000;

        public const ushort EUROLITE_PRO_FRAME_SIZE = 518;
    }

}
