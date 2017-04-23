using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    enum EfcRunningMode
    {
        Programming,
        Running
    };

    enum EfcMasterMode
    {
        Smart,
        ColorAndDimming,
        OnlyColor,
        OnlyDimming,
        AllChannels
    };

    enum EfcBlackoutMode
    {
        Master,
        Smart,
        ColorAndDimming,
        OnlyColor,
        OnlyDimming,
        AllChannels
    };

    enum EfcOutputMode
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
