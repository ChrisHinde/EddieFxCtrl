using EddieFxCtrl.Classes;
using EddieFxCtrl.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace EddieFxCtrl
{
    /// <summary>
    /// Interaction logic for efcMainWindow.xaml
    /// </summary>
    public partial class EfcMainWindow : Window, INotifyPropertyChanged
    {
        public const int FIXTURES = 0, EFFECTS = 1, SCENES = 2, INFO = 3, OUTPUT = 4;
        public const int MAX_UNIVERSES = 4;
        
        public ObservableCollection<EfcCompany> Companies;
        public int MaxCompanyID = 0;

        public ObservableCollection<EfcFixtureModel> FixtureModels;
        public int MaxFixtureID = 0;

        public EfcShow CurrentShow;

        protected UInt16 _MasterValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EfcValuesUpdatedEventHandler OnValuesUpdated;
        public event EfcChannelChangedEventHandler OnChannelChanged;
        public event EfcPatchChangedEventHandler OnPatchChanged;
        public event EfcCSoftPatchChangedEventHandler OnSoftPatchChanged;
        public event EfcUpdateEventHandler OnUpdate;

        public UInt16 MasterValue
        {
            get => _MasterValue;
            set
            {
                _MasterValue = value;
                NotifyPropertyChanged("MasterValue");
            }
        }

        private bool _blackoutActive;
        public bool BlackoutActive
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
        private bool _isRunning;
        public bool IsRunning
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
        private EfcPriorityMode _PriorityMode;
        public EfcPriorityMode PriorityMode
        {
            get => _PriorityMode;
            set => _PriorityMode = value;
        }

        public EfcMainWindow()
        {
            _MasterValue = 255;

            InitializeComponent();

            LoadData();

            //MainTabCtrl.SelectedIndex = INFO;
            //InfoTabControl.SelectedIndex = 1;
            PriorityMode = EfcPriorityMode.LTP;

            IsRunning = false;
            BlackoutActive = Properties.Settings.Default.BlackoutDefault;
            Log("Blackout Default:" + BlackoutActive.ToString());

            CurrentShow = new EfcShow(this)
            {
                Name = "New Show"
            };
            FixturesCtrl.SetMainWin(this);

            Log("EFC Started!");
            
        }

        public void Log(string info)
        {
            logTextBox.Text += "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "]: " + info + Environment.NewLine;
        }

        private void LoadData()
        {
            XNamespace ns = "http://diversum.se/apps/efc.xsd";
            
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EFC")))
            {
                Log("Creating EFC directory in AppData!");
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EFC"));
            }

            /** Companies **/
            if ((Properties.Settings.Default.CompaniesFile == "") || !File.Exists(Properties.Settings.Default.CompaniesFile))
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EFC", "efcCompanies.xml");

                Log("No Companies file found! Creating it from default data!");

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
                {
                    //if the file doesn't exist, create it
                    if (!File.Exists(fileName))
                        File.Create(fileName);

                    file.Write(Properties.Resources.efcCompanies);
                }

                Log("Company data stored in: " + fileName);

                Properties.Settings.Default.CompaniesFile = fileName;

                Properties.Settings.Default.Save();
            }

            Companies = new ObservableCollection<EfcCompany>();

            Log("Loading company data from: " + Properties.Settings.Default.CompaniesFile);

            XDocument cDoc = XDocument.Load(Properties.Settings.Default.CompaniesFile);
            //MaxCompanyID = Convert.ToInt32(cDoc.Element(ns + "data").Attribute("max_id").Value);

            var cList = cDoc.Root.Elements(ns + "company");
            foreach (XElement el in cList)
            {
                EfcCompany comp = new EfcCompany()
                {
                    Id = new Guid(el.Attribute("id").Value),
                    Name = el.Element(ns + "name").Value,
                    Url = el.Element(ns + "url").Value,
                    Logo = el.Element(ns + "logo").Value
                };
                Companies.Add(comp);

                Log("Company added: " + comp.ToString());
            }

            /** Fixtures **/
            if ((Properties.Settings.Default.FixturesFile == "") || !File.Exists(Properties.Settings.Default.FixturesFile))
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EFC", "efcFixtures.xml");

                Log("No Fixtures file found! Creating it from default data!");

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
                {
                    //if the file doesn't exist, create it
                    if (!File.Exists(fileName))
                        File.Create(fileName);

                    file.Write(Properties.Resources.efcFixtures);
                }

                Log("Fixture data stored in: " + fileName);

                Properties.Settings.Default.FixturesFile = fileName;

                Properties.Settings.Default.Save();
            }

            FixtureModels = new ObservableCollection<EfcFixtureModel>();

            Log("Loading fixture data from: " + Properties.Settings.Default.FixturesFile);

            XDocument fDoc = XDocument.Load(Properties.Settings.Default.FixturesFile);
            //MaxFixtureID = new Guid(fDoc.Element(ns + "data").Attribute("max_id").Value);
            
            //Log("MaxFixID::" + MaxFixtureID.ToString());

            //MaxFixtureID = 0;
            var fList = fDoc.Root.Elements(ns + "fixture");
            foreach (XElement el in fList)
            {
                EfcFixtureModel fix = new EfcFixtureModel()
                {
                    Id = new Guid(el.Attribute("id").Value),
                    Type = (EfcFixtureType)Convert.ToInt32(el.Attribute("type").Value),
                    Manufacturer = new Guid(el.Attribute("company_id").Value),
                    Name = el.Element(ns + "name").Value,
                    Description = el.Element(ns + "description").Value,
                    Image = el.Element(ns + "image").Value
                };
                EfcFixtureChannel channel;
                EfcFixtureMode mode;
                byte totalChannelCount = 0;

                var modes = el.Element(ns + "modes").Elements(ns + "mode");
                int maxModeId = -1;

                foreach (XElement m in modes)
                {
                    mode = new EfcFixtureMode()
                    {
                        Id = Convert.ToInt32(m.Attribute("id").Value),
                        Name = m.Element(ns + "name").Value
                    };

                    var channels = m.Element(ns + "channels").Elements(ns + "channel");
                    byte channelCount = 0;

                    foreach (XElement c in channels)
                    {
                        channel = new EfcFixtureChannel()
                        {
                            FixtureChannel = Convert.ToByte(c.Attribute("channel").Value),
                            Type = (EfcChannelType)Convert.ToInt32(c.Attribute("type").Value),
                            Name = c.Element(ns + "name").Value,
                            Description = c.Element(ns + "description").Value
                        };
                        mode.Channels.Add(channel);
                        channelCount++;
                    }

                    mode.ChannelCount = channelCount;

                    if (channelCount > totalChannelCount)
                        totalChannelCount = channelCount;

                    if (mode.Id > maxModeId)
                        maxModeId = mode.Id;

                    fix.Modes.Add(mode);

                    Log("Mode added: " + mode.ToString());
                }

                fix.MaxModeId = maxModeId;

                fix.TotalChannelCount = totalChannelCount;

                EfcCompany comp = Companies.Where(X => X.Id == fix.Manufacturer).FirstOrDefault();

                if (comp != null)
                {
                    comp.Fixtures.Add(fix);
                    fix.Company = comp;
                }
                else
                    Log("Can't add Fixture to Company: " + fix.Manufacturer.ToString());

                FixtureModels.Add(fix);

                /*if (fix.Id > MaxFixtureID)
                    MaxFixtureID = fix.Id;*/

                Log("Fixture added: " + fix.ToString());
            }
        }

        public EfcFixtureModel GetFixtureModel(Guid guid)
        {
            return Companies.SelectMany(c => c.Fixtures).First(fix => fix.Id == guid);
        }

        internal void SaveCompanyData()
        {
            XNamespace ns = "http://diversum.se/apps/efc.xsd";
            XDocument cDoc = new XDocument();

            XElement cRootElm = new XElement(ns + "data");
            cRootElm.SetAttributeValue(XNamespace.Xmlns + "efc", "http://diversum.se/apps/efc.xsd");

            cRootElm.SetAttributeValue("type", "companies");
            cRootElm.SetAttributeValue("max_id", MaxCompanyID);

            XElement cElm;

            foreach (EfcCompany company in Companies)
            {
                cElm = new XElement(ns + "company");
                cElm.SetAttributeValue("id", company.Id);

                cElm.Add(new XElement(ns + "name", company.Name));
                cElm.Add(new XElement(ns + "url", company.Url));
                cElm.Add(new XElement(ns + "logo", company.Logo));

                cRootElm.Add(cElm);
            }

            cElm = null;

            cDoc.Add(cRootElm);
            cDoc.Save(Properties.Settings.Default.CompaniesFile);

            Log("Company info saved to: " + Properties.Settings.Default.CompaniesFile);
        }
        internal void SaveFixtureData()
        {
            XNamespace ns = "http://diversum.se/apps/efc.xsd";
            XDocument fDoc = new XDocument( );

            XElement fRootElm = new XElement(ns + "data");
            fRootElm.SetAttributeValue(XNamespace.Xmlns + "efc", "http://diversum.se/apps/efc.xsd");
            
            fRootElm.SetAttributeValue("type", "fixtures");
            //fRootElm.SetAttributeValue("max_id", MaxFixtureID);

            XElement fElm;
            XElement fModes;
            XElement fMode;
            XElement fModeChannel;
            XElement fModeChannels;

            foreach (EfcFixtureModel fixture in FixtureModels)
            {
                fElm = new XElement(ns + "fixture");
                fElm.SetAttributeValue("id", fixture.Id);
                fElm.SetAttributeValue("company_id", fixture.Manufacturer);
                fElm.SetAttributeValue("type", (int)fixture.Type);

                fElm.Add(new XElement(ns + "name", fixture.Name));
                fElm.Add(new XElement(ns + "description", fixture.Description));
                fElm.Add(new XElement(ns + "image", fixture.Image));

                fModes = new XElement(ns + "modes");

                foreach (EfcFixtureMode mode in fixture.Modes)
                {
                    fMode = new XElement(ns + "mode");

                    fMode.SetAttributeValue("id", mode.Id);
                    fMode.Add(new XElement(ns + "name", mode.Name));

                    fModeChannels = new XElement(ns + "channels");

                    foreach (EfcFixtureChannel channel in mode.Channels)
                    {
                        fModeChannel = new XElement(ns + "channel");

                        fModeChannel.SetAttributeValue("channel", channel.FixtureChannel);
                        fModeChannel.SetAttributeValue("type", (int)channel.Type);

                        fModeChannel.Add(new XElement(ns + "name", channel.Name));
                        fModeChannel.Add(new XElement(ns + "description", channel.Description));

                        fModeChannels.Add(fModeChannel);
                    }

                    fMode.Add(fModeChannels);

                    fModes.Add(fMode);
                }

                fElm.Add(fModes);

                fRootElm.Add(fElm);
            }
            

            fDoc.Add(fRootElm);
                
             fDoc.Save(Properties.Settings.Default.FixturesFile);
            //Log(fDoc.ToString());

            Log("Fixture info saved to: " + Properties.Settings.Default.FixturesFile);
        }

        public void ValuesUpdated(object sender, EfcValuesUpdatedEventArgs e)
        {
            OnValuesUpdated?.Invoke(sender, e);
            Updated(sender, EfcEventType.ValuesUpdated, e);
        }
        public void ChannelChanged(object sender, EfcChannelChangedEventArgs e)
        {
            OnChannelChanged?.Invoke(sender, e);
            Updated(sender, EfcEventType.ChannelChanged, e);
        }
        public void PatchChanged(object sender, EfcPatchChangedEventArgs e)
        {
            OnPatchChanged?.Invoke(sender, e);
            Updated(sender, EfcEventType.PatchChanged, e);
        }
        public void SoftPatchChanged(object sender, EfcSoftPatchChangedEventArgs e)
        {
            OnSoftPatchChanged?.Invoke(sender, e);
            Updated(sender, EfcEventType.SoftPatchChanged, e);
        }
        public void Updated(object sender, EfcEventType type, EventArgs e )
        {
            OnUpdate?.Invoke(sender, new EfcUpdateEventArgs(type, e));
        }

        private void GeneralCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void QuitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void PreferencesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EfcPreferencesWindow prefDlg = new EfcPreferencesWindow( this );
            prefDlg.ShowDialog();
        }

        private void NewShowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsRunning;
        }
        private void NewShowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: New show
        }

        private void SaveShowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void SaveShowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentShow.Filename == "")
                SaveShowAsCommand_Executed(sender, e);
            else
                CurrentShow.SaveToFile();
        }

        private void SaveShowAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void SaveShowAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog sfDlg = new SaveFileDialog()
            {
                InitialDirectory = Properties.Resources.Files_InitialDirectory,
                Filter = Properties.Resources.FileExtensions_Show
            };

            if (sfDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentShow.SaveToFile(sfDlg.FileName);
            }
        }

        private void OpenShowCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsRunning;
        }
        private void OpenShowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofDlg = new OpenFileDialog()
            {
                InitialDirectory = Properties.Resources.Files_InitialDirectory,
                Filter = Properties.Resources.FileExtensions_Show
            };

            if (ofDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentShow = EfcShow.OpenFromFile(ofDlg.FileName, this);
                Updated(this, EfcEventType.NewShow, new EfcNewShowEventArgs() { Show = CurrentShow });
                //FixturesCtrl.ShowUpdated();
            }
        }

        private void ModeToolBarButton_Click(object sender, RoutedEventArgs e)
        {
            IsRunning = !IsRunning;
        }
        private void RunModeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IsRunning = !IsRunning;
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
            int tab = Convert.ToInt32(name.Substring(name.IndexOf('_') + 1));

            MainTabCtrl.SelectedIndex = tab;
        }
        private void SimpleDeskCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EfcSimpleDeskWindow simpleDeskWin = new EfcSimpleDeskWindow(this);
            simpleDeskWin.Show();

        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        public void AddFixture_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ! IsRunning; // TODO: Change to check more
        }

        private void AddFixture_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FixturesCtrl?.AddFixture_Executed(sender, e);
        }

        private void ClearLogBtn_Click(object sender, RoutedEventArgs e)
        {
            logTextBox.Text = "";
        }

        private void SaveLogBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfDlg = new SaveFileDialog()
            {
                InitialDirectory = Properties.Resources.Files_InitialDirectory,
                Filter = "*|*"
            };

            if (sfDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(sfDlg.FileName, logTextBox.Text);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Xceed.Wpf.Toolkit.MessageBox.Show("Do you really want to close Eddie Fx Controller?", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }

    public static class EfcUICommands
    {
        public static readonly RoutedUICommand Quit = new RoutedUICommand
            (
                "_Quit",
                "Quit",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.Q,ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand Preferences = new RoutedUICommand
            (
                "_Preferences",
                "Preferences",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.P,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand NewShow = new RoutedUICommand
            (
                "_New Show",
                "New Show",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.N,ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand SaveShow = new RoutedUICommand
            (
                "_Save Show",
                "Save Show",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S,ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand SaveShowAs = new RoutedUICommand
            (
                "Save Show _As",
                "Save Show As",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S,ModifierKeys.Control|ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand OpenShow = new RoutedUICommand
            (
                "_Open Show",
                "Open Show",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.O,ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand RunMode = new RoutedUICommand
            (
                "_Run",
                "Run mode",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F9)
                }
            );
        public static readonly RoutedUICommand Blackout = new RoutedUICommand
            (
                "_Blackout",
                "Blackout",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.B,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        /*
        public static readonly RoutedUICommand AddFixture = new RoutedUICommand
            (
                "_Add Fixture",
                "Add Fixture",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.N,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );*/

        public static readonly RoutedUICommand ScreenFixtures = new RoutedUICommand
            (
                "_Fixtures",
                "ScreenFixtures_0",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D1,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand ScreenEffects = new RoutedUICommand
            (
                "_Effects",
                "ScreenEffects_1",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D2,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand ScreenScenes = new RoutedUICommand
            (
                "_Scenes",
                "ScreenScenes_2",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D3,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand ScreenInfo = new RoutedUICommand
            (
                "_Info",
                "ScreenInfo_3",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D4,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand ScreenOutput = new RoutedUICommand
            (
                "_Control",
                "ScreenOutput_4",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D9,ModifierKeys.Control | ModifierKeys.Shift),
                    new KeyGesture(Key.D5,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
        public static readonly RoutedUICommand SimpleDesk = new RoutedUICommand
            (
                "_Simple desk",
                "Simple Desk",
                typeof(EfcUICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D,ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
    }

}
