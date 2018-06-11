
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

        /* DMX "universal" constants */
        public const ushort DMX_UNIVERSE_SIZE = 512;
        public const ushort DMX_MIN_SLOT_NUMBER = 1;
        public const ushort DMX_MAX_SLOT_NUMBER = DMX_UNIVERSE_SIZE;
        public const byte DMX_MIN_SLOT_VALUE = 0x00;
        public const byte DMX_MAX_SLOT_VALUE = 0xFF;

        public const byte DMX512_START_CODE = 0;
    }
}
