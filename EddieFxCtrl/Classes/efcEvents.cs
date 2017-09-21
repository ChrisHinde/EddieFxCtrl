using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public enum EfcEventType {
        ChannelChanged = 1,
        PatchChanged = 2,
        SoftPatchChanged = 4,
        ValuesUpdated = 8,
        NewShow = 16
    }

    public class EfcChannelChangedEventArgs : EventArgs
    {
        public UInt16 Channel { get; set; }
        public byte Value { get; set; }
    }

    public class EfcPatchChangedEventArgs : EventArgs
    {
        public EfcPatch Patch { get; set; }
        public EfcPatch.PatchEvent EventType { get; set; }
    }
    public class EfcSoftPatchChangedEventArgs : EventArgs
    {
        public UInt16 ChannelIn { get; set; }
        public UInt16 ChannelOut { get; set; }
        public EfcPatch.PatchEvent EventType { get; set; }
    }
    public class EfcValuesUpdatedEventArgs : EventArgs
    {

    }
    public class EfcNewShowEventArgs : EventArgs
    {
        public EfcShow Show { get; set; }
    }
    public class EfcUpdateEventArgs : EventArgs
    {
        public EfcUpdateEventArgs( EfcEventType t, EventArgs e )
        {
            Type = t;
            Args = e;
        }

        public EfcEventType Type { get; set; }
        public EventArgs Args { get; set; }
    }

    public delegate void EfcChannelChangedEventHandler(object sender, EfcChannelChangedEventArgs e);
    public delegate void EfcPatchChangedEventHandler(object sender, EfcPatchChangedEventArgs e);
    public delegate void EfcCSoftPatchChangedEventHandler(object sender, EfcSoftPatchChangedEventArgs e);

    public delegate void EfcValuesUpdatedEventHandler(object sender, EfcValuesUpdatedEventArgs e);
    public delegate void EfcNewShowEventHandler(object sender, EfcNewShowEventArgs e);

    public delegate void EfcUpdateEventHandler(object sender, EfcUpdateEventArgs e);
}
