using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EddieFxCtrl.Classes
{
    public class EfcMain
    {
        protected static EfcMainWindow _MainWin;

        public static EfcShow CurrentShow;

        protected static UInt16 _MasterValue;
        public static UInt16 MasterValue
        {
            get => _MasterValue;
            set
            {
                _MasterValue = value;
                NotifyPropertyChanged("MasterValue");
            }
        }

        private static bool _blackoutActive;
        public static bool BlackoutActive
        {
            get { return _blackoutActive; }
            set
            {
                if (value != _blackoutActive)
                {
                    _blackoutActive = value;
                    Log("Blackout:" + _blackoutActive.ToString());
                    NotifyPropertyChanged();
                }
            }
        }
        private static bool _isRunning;
        public static bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                if (value != _isRunning)
                {
                    _isRunning = value;
                    Log("RunMode:" + _isRunning.ToString());
                    NotifyPropertyChanged();
                }
            }
        }
        private static bool _isFreezed;
        public static bool IsFreezed
        {
            get { return _isFreezed; }
            set
            {
                if (value != _isFreezed)
                {
                    _isFreezed = value;
                    Log("Freeze:" + _isFreezed.ToString());
                    NotifyPropertyChanged("Freezed");
                }
            }
        }
        private static EfcPriorityMode _PriorityMode;
        public static EfcPriorityMode PriorityMode
        {
            get => _PriorityMode;
            set => _PriorityMode = value;
        }

        public static event PropertyChangedEventHandler PropertyChanged;

        public static void Initialize( EfcMainWindow mainWin )
        {
            _MainWin = mainWin;

            _MasterValue = 255;
            PriorityMode = EfcPriorityMode.LTP;

            IsFreezed = false;
            IsRunning = false;
            BlackoutActive = Properties.Settings.Default.BlackoutDefault;
            Log("Blackout Default:" + BlackoutActive.ToString());

            CurrentShow = new EfcShow(_MainWin)
            {
                Name = "New Show"
            };
        }

        public static void Log(String info)
        {
            _MainWin?.Log(info);
        }
        
        private static void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}
