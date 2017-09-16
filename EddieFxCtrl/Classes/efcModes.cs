using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public enum EfcRunningMode
    {
        Programming,
        Running
    };

    public enum EfcPriorityMode
    {
        HTP,
        LTP,
        Capture
    };

    public enum EfcMasterMode
    {
        Smart,
        ColorAndDimming,
        OnlyColor,
        OnlyDimming,
        AllChannels
    };

    public enum EfcBlackoutMode
    {
        Master,
        Smart,
        ColorAndDimming,
        OnlyColor,
        OnlyDimming,
        AllChannels
    };

    public enum EfcOutputMode
    {
        OnlyUsed,
        First_16,
        First_32,
        First_48,
        First_64,
        First_96,
        First_128,
        First_256,
        All
    };
}
