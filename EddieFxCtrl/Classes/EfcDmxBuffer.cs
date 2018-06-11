using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcDmxBuffer
    {

        protected ushort _length;
        protected byte[] _data;

        public EfcDmxBuffer()
        {

        }
        public EfcDmxBuffer(ushort length)
        {
            _length = length;
        }
        public EfcDmxBuffer(EfcDmxBuffer buffer)
        {
            Set(buffer);
        }
        public EfcDmxBuffer(byte[] data, ushort length)
        {
            Set(data, length);
        }

        protected bool Init()
        {
            EfcMain.Log("DmxBuffer init");

            _data = new byte[EfcConstants.DMX_UNIVERSE_SIZE];

            return true;
        }

        public bool Blackout()
        {
            EfcMain.Log("DmxBuffer Blackout");

            if (_data == null)
            {
                if (!Init())
                    return false;
            }

            EfcMain.Log("Blacking out");
            for (int i = 0; i < EfcConstants.DMX_UNIVERSE_SIZE; i++)
            {
                _data[i] = EfcConstants.DMX_MIN_SLOT_VALUE;
            }
            _length = EfcConstants.DMX_UNIVERSE_SIZE;
            EfcMain.Log("Blackout done");

            return true;
        }

        internal ushort Size()
        {
            return _length;
        }

        public bool Reset()
        {
            if (_data != null)
            {
                _length = 0;
                return true;
            }
            return false;
        }

        public bool SetValue(ushort channel, byte data)
        {
            if (channel >= EfcConstants.DMX_UNIVERSE_SIZE)
                return false;

            if (_data == null)
                Blackout();

            if (channel > _length) // ??? 
                return false;

            _data[channel] = data;
            _length = (ushort)Math.Max(channel + 1, _length);

            return true;
        }
        public bool SetRangeToValue(ushort start, byte data, ushort length)
        {
            if ((start >= EfcConstants.DMX_UNIVERSE_SIZE) || (start > _length))
                return false;

            if (_data == null)
                Blackout();

            ushort copy_length = (ushort)Math.Min(length, EfcConstants.DMX_UNIVERSE_SIZE - start);

            for (int i = start; i < copy_length; i++)
            {
                _data[i] = data;
            }
            _length = (ushort)Math.Max(_length, start + copy_length);

            return true;
        }
        public bool SetRange(ushort start, byte[] data, ushort length)
        {
            if ((data == null) || (start >= EfcConstants.DMX_UNIVERSE_SIZE))
                return false;

            if (_data == null)
                Blackout();

            if (start > _length)
                return false;

            Array.Copy(data, 0, _data, start, _length);

            return true;
        }
        public void SetFromString(string data)
        {
            throw new NotImplementedException();
        }
        public void Set(EfcDmxBuffer buffer)
        {
            throw new NotImplementedException();
        }
        public bool Set(byte[] data, ushort length)
        {
            if (data == null)
                return false;

            if (_data == null)
            {
                if (!Init())
                    return false;
            }
            _length = Math.Min(length, EfcConstants.DMX_UNIVERSE_SIZE);

            /*for (ushort i=0; i<length; i++)
            {
                _data[i] = data[i];
            }
            _length = length;*/

            Array.Copy(data, _data, _length);

            return true;
        }

        public bool HTPMerge(EfcDmxBuffer buffer)
        {
            if (_data == null)
            {
                if (!Init())
                    return false;
            }

            ushort other_length = Math.Min(EfcConstants.DMX_UNIVERSE_SIZE, buffer._length);
            ushort merge_length = Math.Min(_length, buffer._length);

            for (ushort i = 0; i < merge_length; i++)
            {
                _data[i] = Math.Max(_data[i], buffer._data[i]);
            }

            if (other_length > _length)
            {
                for (ushort i = merge_length; i < other_length; i++)
                    _data[i] = buffer._data[i];

                _length = other_length;
            }

            return true;
        }

        public byte GetValue(ushort channel)
        {
            if ((_data != null) && (channel < _length))
            {
                return _data[channel];
            }
            else
            {
                return 0;
            }
        }
        public byte Get(ushort channel)
        {
            return GetValue(channel);
        }
        public void Get(ref byte[] data, ref ushort length)
        {
            if (_data != null)
            {
                length = Math.Min(length, _length);
                Array.Copy(_data, data, length);
            }
            else
                length = 0;
        }
        public void Get(ref byte[] data, ushort data_start, ref ushort length)
        {
            if (_data != null)
            {
                length = Math.Min(length, _length);
                Array.Copy(_data, 0, data, data_start, length);
            }
            else
                length = 0;
        }
        public void GetRange(ushort start, ref byte[] data, ref ushort length)
        {
            if (start >= _length)
            {
                length = 0;
                return;
            }

            if (_data != null)
            {
                length = (ushort)Math.Min(length, length - start);
                Array.Copy(_data, start, data, 0, length);
            }
            else
            {
                length = 0;
            }
        }

        public byte this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }
}
