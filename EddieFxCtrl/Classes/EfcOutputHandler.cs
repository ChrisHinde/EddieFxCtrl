using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcOutputHandler
    {
        protected static Boolean _Initated = false;

        protected static byte[,] _Buffer;
        protected static EfcMainWindow _MainWin;
        protected static byte _UniverseCount;
        protected static UInt16[] _MaxChannel;
        protected static Boolean _RunRead;
        protected static Boolean _RunOutput;
        protected static Thread _ReadThread;
        protected static Thread _OutputThread;

        public static EfcMainWindow MainWin
        {
            get => _MainWin;
            set
            {
                _MainWin = value;
            }
        }

        public static void Initiate(EfcMainWindow mainWin, byte unicount = 0)
        {
            _MainWin = mainWin;

            _RunOutput = false;
            _RunRead = false;

            _UniverseCount = (unicount == 0) ? EfcMainWindow.MAX_UNIVERSES : unicount;

            _Buffer = new byte[_UniverseCount+1,513];
            _MaxChannel = new UInt16[_UniverseCount+1];

            for (int u = 0; u <= _UniverseCount; u++)
            {
                _MaxChannel[u] = 32;
            }

            _Initated = true;
        }

        public static void Start()
        {
            if (!_Initated) throw new Exception("Output handler is not initiated!");

            _RunOutput = true;
            _RunRead = true;

            _ReadThread = new Thread(new ThreadStart(ReadLoop));
            _OutputThread = new Thread(new ThreadStart(OutputLoop));

            _ReadThread.Start();
            _OutputThread.Start();
        }
        public static void Stop()
        {
            if (!_Initated) throw new Exception("Output handler is not initiated!");

            _RunOutput = false;
            _RunRead = false;

            _OutputThread.Abort();
        }
        public static void Pause()
        {
            //_ReadThread.Suspend();

        }

        public static void ReadLoop()
        {
            while (_RunRead)
            {
                for (byte u = 1; u <= _UniverseCount; u++)
                {
                    for (UInt16 c = 1; c <= _MaxChannel[u]; c++)
                    {
                        _Buffer[u, c] = _MainWin.CurrentShow.Universes[u].PatchInfos[c].Value;
                    }
                }
                //Thread.Sleep(500);
            }
        }
        public static void OutputLoop()
        {
            UInt16 u, c;

            while (_RunOutput)
            {
                for (u = 1; u <= _UniverseCount; u++)
                {
                    //Console.Write("U{0}: ", u);
                    for (c = 1; c <= _MaxChannel[u]; c++)
                    {
                    //    Console.Write("{0:000},", _Buffer[u, c]);
                    }
                   // Console.WriteLine(";");
                }
                //Console.WriteLine("--------");
                Thread.Sleep(100);
            }
        }
    }
}
