using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    enum efcRunningMode
    {
        Programming,
        Running
    };

    enum efcMasterMode
    {
        Smart,
        ColorAndDimming,
        OnlyColor,
        OnlyDimming,
        AllChannels
    };

    enum efcBlackoutMode
    {
        Master,
        Smart,
        ColorAndDimming,
        OnlyColor,
        OnlyDimming,
        AllChannels
    };

    enum efcOutputMode
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
