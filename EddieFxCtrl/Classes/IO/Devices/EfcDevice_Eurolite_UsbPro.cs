using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes.IO.Devices
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

    public class EfcDevice_Eurolite_UsbPro : EfcDevice_Template
    {
       /* const ushort URB_TIMEOUT_MS = 500;
        const byte DMX_LABEL = 6;
        const byte START_OF_MESSAGE = 0x7e;
        const byte END_OF_MESSAGE = 0xe7;
        const byte ENDPOINT = 0x02;
        const int BAUDRATE = 250000;

        const ushort EUROLITE_PRO_FRAME_SIZE = 518;*/
        
        protected string _serial;
        protected SerialPort _serialport;

        protected byte[] _tx_frame;

        public EfcDevice_Eurolite_UsbPro()
        {
            _tx_frame = new byte[Eurolite_Constants.EUROLITE_PRO_FRAME_SIZE];
        }

        public override void Initiate(EfcDeviceData data)
        {
            EfcMain.Log("Eurolite Init");

            _isOpen = false;

            EfcMain.Log("Creating object : Port " + data.Port);
            _serialport = new SerialPort(data.Port, Eurolite_Constants.BAUDRATE);//,Parity.None,8,StopBits.One);

            if (!_serialport.IsOpen)
            {
                EfcMain.Log("Port not open > Opening port");
                _serialport.Open();

                _serialport.DataReceived += _serialport_DataReceived;
                _serialport.ErrorReceived += _serialport_ErrorReceived;
                _serialport.PinChanged += _serialport_PinChanged;

                byte[] msg = { 0x7E, 0x0A, 0x00, 0x00, 0xE7 };
                _serialport.Write(msg, 0, 5);/**/

                if (_serialport.BytesToRead > 0)
                    Console.WriteLine("SerialRead:" + _serialport.ReadExisting());
            }
            else
                EfcMain.Log("Port open");

            _isOpen = true;
        }

        private void _serialport_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            Console.WriteLine("Serial PinCh:" + e.EventType.ToString());
        }

        private void _serialport_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine("Serial ERROR:" + e.ToString());
        }

        private void _serialport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Serial Data:" + e.EventType.ToString());
            if (e.EventType == SerialData.Chars)
                Console.WriteLine(_serialport.ReadExisting());
        }

        public override void Close()
        {
            EfcMain.Log("Eurolite Close");

            if (_serialport.IsOpen)
            {
                EfcMain.Log("Eurolite Closing");
                _serialport.Close();
            }

            _isOpen = false;
        }


        public override string GetDeviceId()
        {
            throw new NotImplementedException();
        }
        public string SerialNumber()
        {
            return _serial;
        }

        public override bool SendDMX(ref EfcDmxBuffer buffer)
        {
            //EfcMain.Log("Eurolite SenDMX");
            if (!IsOpen)
                return false;

            try
            {
                ushort frame_size = Eurolite_Constants.EUROLITE_PRO_FRAME_SIZE;
                CreateFrame(ref buffer, ref _tx_frame, ref frame_size);
                /*
                #if DEBUG
                                for (int i = 0; i < frame_size; i++)
                                {
                                    Console.Write(_tx_frame[i].ToString("X2") + " ");
                                }
                                Console.WriteLine('!');
                #endif*/

                _serialport.Write(_tx_frame, 0, frame_size);

                if (_serialport.BytesToRead > 0)
                    Console.WriteLine("SerialRead:" + _serialport.ReadExisting());
            }
            catch (Exception e)
            {
                Console.WriteLine("=============================");
                Console.WriteLine(e.ToString());
            }

            return true;
        }

        public override bool SendData(byte[] data, ushort length = 512)
        {
            throw new NotImplementedException();
        }

        /*
         * Create a Eurolite Pro message to match the supplied DmxBuffer.
         */
        protected void CreateFrame(ref EfcDmxBuffer buffer, ref byte[] frame, ref ushort frame_size)
        {
            //ushort frame_size = buffer.Size();
            frame_size = buffer.Size();

            // Header
            frame[0] = Eurolite_Constants.START_OF_MESSAGE;
            frame[1] = Eurolite_Constants.DMX_LABEL;

            // LSB first for Universe size
            /* frame[2] = (byte)(Constants.DMX_UNIVERSE_SIZE & 0xFFu);         // LSB
             frame[3] = (byte)((Constants.DMX_UNIVERSE_SIZE >> 8) & 0xFFu);  // MSB*/

            frame[4] = EfcConstants.DMX512_START_CODE;

            // Main Body/frame
            buffer.Get(ref frame, 5, ref frame_size);

            frame_size++; // <<<<< THIS!!! THIS IS IMPORTANT!!!

            // LSB first for Universe size
            frame[2] = (byte)(frame_size & 0xFFu);         // LSB
            frame[3] = (byte)((frame_size >> 8) & 0xFFu);  // MSB

            frame_size += 5;
            // End frame
            frame[frame_size - 1] = Eurolite_Constants.END_OF_MESSAGE;
            //frame[EUROLITE_PRO_FRAME_SIZE - 1] = END_OF_MESSAGE;
        }

        /*
         * Find the interface with the endpoint we're after. Usually this is interface
         * 1 but we check them all just in case.
         */
        public static bool LocateInterface()
        {
            throw new NotImplementedException();
        }

        public override EfcDeviceInfo GetDeviceInfo()
        {
            throw new NotImplementedException();
        }
    }
}
