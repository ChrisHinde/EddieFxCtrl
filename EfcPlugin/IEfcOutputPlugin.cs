using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Plugin
{
    public interface IEfcOutputPlugin : IEfcPlugin
    {
        void Stop();
        void Freeze();

        //bool ChannelByChannel { get; }

        void Output(byte Universe, byte[] data, UInt16 count = 512);    // Default and prefered method, array (and Universe) is 0 based
        void Output(byte Universe, UInt16 channel, byte data);          //
    }
}
