using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public enum efcChannelType
    {
        NoFunction = 0,

        Macro,
        Filter,

        ColorMacro,
        ColorRed,
        ColorGreen,
        ColorBlue,
        ColorWhite,
        ColorCyan,
        ColorMagenta,
        ColorYellow,
        Dimmer,
        DimmerFine,

        Shutter,
        ShutterFine,
        Gobo,
        GoboRotation,
        Zoom,
        ZoomFine,
        Speed,
        Beam,
        Strobe,
        StrobeSpeed,

        Pan,
        PanFine,
        Tilt,
        TiltFine,
        PanSpeed,
        TiltSpeed,
        PanTiltSpeed,

        Pump,
        Fan,


        Control,

        Custom
    }
}
