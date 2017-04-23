using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcAddressInUseException : Exception
    {
        protected UInt16 _Address;

        public UInt16 Address { get => _Address; }

        public EfcAddressInUseException( UInt16 address, String message ) : base(message)
        {
            _Address = address;
        }
        public EfcAddressInUseException( UInt16 address ) : this(address, String.Format("The adress %d is already in use!",address))
        {
        }
    }
}
