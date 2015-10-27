using EddieFxCtrl.Classes;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace EddieFxCtrl
{
    /// <summary>
    /// Interaction logic for efcMainWindow.xaml
    /// </summary>
    public partial class efcMainWindow : Window, INotifyPropertyChanged
    {
        public const int FIXTURES = 0, EFFECTS = 1, SCENES = 2, INFO = 3, OUTPUT = 4;
        public ObservableCollection<efcCompany> Companies;
        public int MaxCompanyID = 0;
        public ObservableCollection<efcFixtureModel> FixtureModels;
        public int MaxFixtureID = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _blackoutActive;
        public bool BlackoutActive
        {
            get { return _blackoutActive; }
            set
            {
                if (value != _blackoutActive)
                {
                    _blackoutActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public efcMainWindow()
        {
            InitializeComponent();

            log("EFC Started!");

            LoadData();

            MainTabCtrl.SelectedIndex = INFO;
            InfoTabControl.SelectedIndex = 1;

            BlackoutActive = Properties.Settings.Default.BlackoutDefault;
            log("Blackout Default:" + BlackoutActive.ToString());
        }

        public void log(string info)
        {
            logTextBox.Text += "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "]: " + info + Environment.NewLine;
        }

        private void LoadData()
        {
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EFC")))
            {
                log("Creating EFC directory in AppData!");
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EFC"));
            }

            /** Companies **/
            if ((Properties.Settings.Default.CompaniesFile == "") || !File.Exists(Properties.Settings.Default.CompaniesFile))
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EFC", "efcCompanies.xml");

                log("No Companies file found! Creating it from default data!");

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
                {
                    //if the file doesn't exist, create it
                    if (!File.Exists(fileName))
                        File.Create(fileName);

                    file.Write(Properties.Resources.efcCompanies);
                }

                log("Company data stored in: " + fileName);

                Properties.Settings.Default.CompaniesFile = fileName;

                Properties.Settings.Default.Save();
            }

            Companies = new ObservableCollection<efcCompany>();

            log("Loading company data from: " + Properties.Settings.Default.CompaniesFile);

            XDocument cDoc = XDocument.Load(Properties.Settings.Default.CompaniesFile);
            MaxCompanyID = Convert.ToInt32(cDoc.Element("{http://diversum.se/apps/efc.xsd}data").Attribute("max_id").Value);

            var cList = cDoc.Root.Elements("{http://diversum.se/apps/efc.xsd}company");
            foreach (XElement el in cList)
            {
                efcCompany comp = new efcCompany();
                comp.Id = Convert.ToInt32(el.Attribute("id").Value);
                comp.Name = el.Element("{http://diversum.se/apps/efc.xsd}name").Value;
                comp.Url = el.Element("{http://diversum.se/apps/efc.xsd}url").Value;
                comp.Logo = el.Element("{http://diversum.se/apps/efc.xsd}logo").Value;

                Companies.Add(comp);

                log("Company added: " + comp.ToString());
            }

            /** Fixtures **/
            if ((Properties.Settings.Default.FixturesFile == "") || !File.Exists(Properties.Settings.Default.FixturesFile))
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EFC", "efcFixtures.xml");

                log("No Fixtures file found! Creating it from default data!");

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
                {
                    //if the file doesn't exist, create it
                    if (!File.Exists(fileName))
                        File.Create(fileName);

                    file.Write(Properties.Resources.efcFixtures);
                }

                log("Fixture data stored in: " + fileName);

                Properties.Settings.Default.FixturesFile = fileName;

                Properties.Settings.Default.Save();
            }

            FixtureModels = new ObservableCollection<efcFixtureModel>();

            log("Loading fixture data from: " + Properties.Settings.Default.FixturesFile);

            XDocument fDoc = XDocument.Load(Properties.Settings.Default.FixturesFile);
            MaxFixtureID = Convert.ToInt32(cDoc.Element("{http://diversum.se/apps/efc.xsd}data").Attribute("max_id").Value);

            var fList = fDoc.Root.Elements("{http://diversum.se/apps/efc.xsd}fixture");
            foreach (XElement el in fList)
            {
                efcFixtureModel fix = new efcFixtureModel();
                fix.Id = Convert.ToInt32(el.Attribute("id").Value);
                fix.Type = (efcFixtureType)Convert.ToInt32(el.Attribute("type").Value);
                fix.Manufacturer = Convert.ToInt32(el.Attribute("company_id").Value);
                fix.Name = el.Element("{http://diversum.se/apps/efc.xsd}name").Value;
                fix.Description = el.Element("{http://diversum.se/apps/efc.xsd}description").Value;
                fix.Image = el.Element("{http://diversum.se/apps/efc.xsd}image").Value;

                efcFixtureChannel channel;
                efcFixtureMode mode;
                byte totalChannelCount = 0;

                var modes = el.Element("{http://diversum.se/apps/efc.xsd}modes").Elements("{http://diversum.se/apps/efc.xsd}mode");
                int maxModeId = -1;

                foreach (XElement m in modes)
                {
                    mode = new efcFixtureMode();

                    mode.Id = Convert.ToInt32(m.Attribute("id").Value);
                    mode.Name = m.Element("{http://diversum.se/apps/efc.xsd}name").Value;

                    var channels = m.Element("{http://diversum.se/apps/efc.xsd}channels").Elements("{http://diversum.se/apps/efc.xsd}channel");
                    byte channelCount = 0;

                    foreach (XElement c in channels)
                    {
                        channel = new efcFixtureChannel();

                        channel.FixtureChannel = Convert.ToByte(c.Attribute("channel").Value);
                        channel.Type = (efcChannelType)Convert.ToInt32(c.Attribute("type").Value);
                        channel.Name = c.Element("{http://diversum.se/apps/efc.xsd}name").Value;
                        channel.Description = c.Element("{http://diversum.se/apps/efc.xsd}description").Value;

                        mode.Channels.Add(channel);
                        channelCount++;
                    }

                    mode.ChannelCount = channelCount;

                    if (channelCount > totalChannelCount)
                        totalChannelCount = channelCount;

                    if (mode.Id > maxModeId)
                        maxModeId = mode.Id;

                    fix.Modes.Add(mode);

                    log("Mode added: " + mode.ToString());
                }

                fix.MaxModeId = maxModeId;

                fix.TotalChannelCount = totalChannelCount;

                efcCompany comp = Companies.Where(X => X.Id == fix.Manufacturer).FirstOrDefault();

                if (comp != null)
                    comp.Fixtures.Add(fix);
                else
                    log("Can't add Fixture to Company: " + fix.Manufacturer.ToString());

                FixtureModels.Add(fix);

                log("Fixture added: " + fix.ToString());
            }
        }

        private void GeneralCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void QuitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            /*if (Xceed.Wpf.Toolkit.MessageBox.Show("Do you really want to close the application?", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Application.Current.Shutdown();*/
            Close();
        }

        private void PreferencesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            efcPreferencesWindow prefDlg = new efcPreferencesWindow( this );
            prefDlg.ShowDialog();
        }

        private void BlackoutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            BlackoutActive = !BlackoutActive;
        }

        private void BlackoutOutputTB_Click(object sender, RoutedEventArgs e)
        {
            BlackoutActive = !BlackoutActive;
        }

        private void ScreenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string name = (e.Command as RoutedUICommand).Name;
            int tab = Convert.ToInt32( name.Substring(name.IndexOf('_')+1) );

            MainTabCtrl.SelectedIndex = tab;
        }

        private void ClearLogBtn_Click(object sender, RoutedEventArgs e)
        {
            logTextBox.Text = "";
        }

        private void SaveLogBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Xceed.Wpf.Toolkit.MessageBox.Show("Do you really want to close Eddie Fx Controller?", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public static class efcUICommands
    {
        public static readonly RoutedUICommand Quit = new RoutedUICommand
            (
                "_Quit",
                "Quit",
                typeof(efcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.Q,ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand Preferences = new RoutedUICommand
            (
                "_Preferences",
                "Preferences",
                typeof(efcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.P,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand Blackout = new RoutedUICommand
            (
                "_Blackout",
                "Blackout",
                typeof(efcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.B,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand ScreenFixtures = new RoutedUICommand
            (
                "_Fixtures",
                "ScreenFixtures_0",
                typeof(efcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D1,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand ScreenEffects = new RoutedUICommand
            (
                "_Effects",
                "ScreenEffects_1",
                typeof(efcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D2,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand ScreenScenes = new RoutedUICommand
            (
                "_Scenes",
                "ScreenScenes_2",
                typeof(efcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D3,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand ScreenInfo = new RoutedUICommand
            (
                "_Info",
                "ScreenInfo_3",
                typeof(efcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D4,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand ScreenOutput = new RoutedUICommand
            (
                "_Output",
                "ScreenOutput_4",
                typeof(efcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D9,ModifierKeys.Control | ModifierKeys.Shift),
                    new KeyGesture(Key.D5,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
    }

}
