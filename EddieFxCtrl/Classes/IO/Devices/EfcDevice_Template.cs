using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes.IO.Devices
{
    public struct EfcDeviceData
    {
        public bool Serial;
        public String Port;
        public uint Baud;
        public int Interval;
    }
    public struct EfcDeviceInfo
    {
        public Guid Id;
        public String Name;
        public String Version;
        public float Version_Numeric;
        public String Description;

    }

    public abstract class EfcDevice_Template
    {
        protected bool _isOpen;

        public bool IsOpen { get => _isOpen; }

        public abstract string GetDeviceId();
        public abstract EfcDeviceInfo GetDeviceInfo();

        public abstract void Initiate(EfcDeviceData data);
        public abstract void Close();

        public abstract Boolean SendData(byte[] data, UInt16 length = EfcConstants.DMX_UNIVERSE_SIZE);
        public virtual Boolean SendDMX(ref EfcDmxBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
