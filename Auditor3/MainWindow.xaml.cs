using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Timer = System.Timers.Timer;
using Auditor3.Locations;
using static Auditor3.Locations.Locations;

namespace Auditor3
{
    public partial class MainWindow
    {
        // ... (Constructors and other methods remain unchanged) ...
        private static Timer _refreshTimer;
        private string LoadPRECsFile;
        private string OutputText;
        private object OutputLock;
        private AssistantContext _selectedAssistantContext;
        public MainWindow()
        {
            InitializeComponent();
            Startup();
        }

        public MainWindow(string precs)
        {
            InitializeComponent();
            LoadPRECsFile = precs;
            Startup();
        }

        private void Startup()
        {
            WindowState = WindowState.Maximized;
            Title = $"Corruption Auditor v{Globals.VERSION()}";

            foreach (var release in Enum.GetNames(typeof(CMRelease)))
                CMReleaseBox.Items.Add(release);

            CMReleaseBox.SelectedItem = CMRelease.CM6_3.ToString();
            OutputText = "";
            OutputLock = new object();

            var init = new Task(Initialize);
            init.Start();
        }


        // UPDATED: Now sets the ThemeInputBrush color for both modes
        

        // UPDATED: Now sets the ThemeInputBrush color for both modes
        private void Click_ToggleTheme(object sender, RoutedEventArgs args)
        {
            var resources = Application.Current.Resources;

            if (DarkModeToggle.IsChecked == true)
            {
                // Dark Mode Palette
                resources["ThemeBackgroundBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#111827"));
                resources["ThemeSurfaceBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1F2937"));
                resources["ThemePrimaryBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#38BDF8"));
                resources["ThemePrimaryHoverBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0EA5E9"));
                resources["ThemeTextBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F9FAFB"));
                resources["ThemeTextInverseBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0F172A"));
                resources["ThemeBorderBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#374151"));
                resources["ThemeInputBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#374151")); // Darker input background
                resources["ThemeShadowColor"] = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFFFFF");
                resources["ThemeAlternateRowBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#111827"));
                resources["ThemeHoverBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#374151"));
                resources["ThemeSelectedBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E3A8A"));
            }
            else
            {
                // Light Mode Palette - DARK TEXT ON LIGHT BACKGROUND ✅
                resources["ThemeBackgroundBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F3F4F6"));
                resources["ThemeSurfaceBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFFFFF"));
                resources["ThemePrimaryBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0284C7"));
                resources["ThemePrimaryHoverBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0369A1"));
                resources["ThemeTextBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1F2937"));  // ✅ DARK TEXT
                resources["ThemeTextInverseBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFFFFF"));
                resources["ThemeBorderBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#9CA3AF"));
                resources["ThemeInputBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E5E7EB")); // Light gray input background
                resources["ThemeShadowColor"] = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF000000");
                resources["ThemeAlternateRowBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F9FAFB"));
                resources["ThemeHoverBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#EFF6FF"));
                resources["ThemeSelectedBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#DBEAFE"));
            }
        }

        // ... (The rest of the file remains unchanged) ...
        internal void AddStatus(string status)
        {
            status = $"* {status}";
            void addStatus()
            {
                OutputLabel.Focus();
                StatusBox.Text += status + Environment.NewLine;
                StatusBox.ScrollToEnd();
            }
            Dispatcher.Invoke(addStatus);
        }

        internal void AddOutput(string message)
        {
            void addOutput()
            {
                lock (OutputLock) { OutputText += message + Environment.NewLine; }
            }
            Dispatcher.Invoke(addOutput);
        }

        internal string GetOutput()
        {
            var output = "";
            void getOutput() { output = OutputBox.Text; }
            Dispatcher.Invoke(getOutput);
            return output;
        }

        internal void Error(string message)
        {
            AddStatus(message);
        }

        internal void Error(string message, Exception error)
        {
            var log = new StringBuilder();
            log.AppendLine("ERROR REPORT");
            log.AppendLine();
            log.AppendLine(string.Join(Environment.NewLine, Globals.WORKING_PREC));
            log.AppendLine();
            log.AppendLine($"OCCURED : {Globals.TIMESTAMP()}");
            log.AppendLine($"MESSAGE : {message}");
            log.AppendLine();
            log.AppendLine($"TYPE    : {error.GetType().Name}");
            log.AppendLine($"ERROR   : {error.Message}");
            log.AppendLine($"DATA    : {error.Data}");
            log.AppendLine($"SOURCE  : {error.Source}");
            log.AppendLine($"STACK   : {error.StackTrace}");
            log.AppendLine($"TARGET  : {error.TargetSite}");

            while (error.InnerException != null)
            {
                error = error.InnerException;
                log.AppendLine($"INNER   : {error.Message}");
            }

            var logfile = Globals.REPORT("crash");
            var writer = new StreamWriter(logfile);
            writer.Write(log.ToString());
            writer.Close();

            AddStatus(message);
            AddStatus($"Crash report generated at {logfile}");

            if (!Globals.VERSION_DEV && Globals.TOOLSA.Connected())
            {
                var report = $"{Globals.CRASH_FOLDER_TOOLSA}crash_{Globals.TIMESLICE()}_{Globals.USER_DATA.ToolsAUsername}.log";
                if (Globals.TOOLSA.SendFile(logfile, report))
                {
                    AddStatus($"Crash report uploaded to {report}");
                }
            }
        }

        internal void Idle()
        {
            Globals.STATE = State.IDLE;
            Globals.PROCESS = Process.NONE;
            Globals.START_TIME = DateTime.MinValue;
            Audits.Checked = 0;
            Audits.ToCheck = 0;
            Globals.CANCEL = false;

            void setButtons() { SetButtons(false); }
            Dispatcher.Invoke(setButtons);
        }

        internal void SetPRECs(string precs)
        {
            void update() { DataBox.Text = precs; }
            Dispatcher.Invoke(update);
        }

        private void Initialize()
        {
            AddStatus($"Auditor is initializing on v{Globals.VERSION()}");
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.INITIALIZING;
            Globals.GUI = this;

            if (!Directory.Exists(Globals.REPORT_DIR)) Directory.CreateDirectory(Globals.REPORT_DIR);

            Globals.USER_DATA = null;
            Globals.MODE = Mode.OFFLINE;
            Globals.PRECS_LOADED = false;
            Globals.AUDIT_COMPLETE = false;
            Globals.CANCEL = false;
            Globals.START_TIME = DateTime.MinValue;
            Audits.ResetCounters();

            Locations.Locations.INITIALIZE();
            Database.Initialize();
            InitializeUserData();

            void setUserValues()
            {
                SitePort.Text = Globals.USER_DATA.DefaultLivePort;
                LabIP.Text = Globals.USER_DATA.DefaultLabIP;
            }
            Dispatcher.Invoke(setUserValues);

            Globals.TOOLSA = new ToolsAConnection();
            Globals.DRCCD = new DRCCDConnection();
            Globals.CM = new CMConnection();
            Globals.CM_LABS = new List<LabInfo>();
            Globals.MG_LABS = new List<LabInfo>();
            Globals.CM_PATCHES = new List<PatchInfo>();

            if (CURRENT().HasToolsA())
            {
                Globals.TOOLSA.Connect();
                Globals.TOOLSA.CheckUpdates();
                Globals.TOOLSA.RetrieveLabInfo();
            }

            if (CURRENT().HasDRCCD())
            {
                Globals.DRCCD.Connect();
            }

            _refreshTimer = new Timer(Globals.REFRESH_TIMER);
            _refreshTimer.Elapsed += Fired_RefreshTimer;
            _refreshTimer.Start();

            if (!string.IsNullOrEmpty(LoadPRECsFile))
            {
                LoadPRECs(LoadPRECsFile);
                LoadPRECsFile = null;
            }

            AddStatus("Auditor is now ready for use");
            Idle();
        }

        private void InitializeUserData()
        {
            if (File.Exists(Globals.USER_DATA_FILE)) { LoadUserData(); }
            if (Globals.USER_DATA == null)
            {
                Globals.USER_DATA = new UserData();
                CollectUserData();
            }
        }

        private void LoadUserData()
        {
            try
            {
                AddStatus("Reading user data file");
                var file = new StreamReader(Globals.USER_DATA_FILE);
                var data = file.ReadToEnd();
                file.Close();
                var user = Encrypt.DecryptString(data);
                Globals.USER_DATA = Globals.DESERIALIZE<UserData>(user);
                var check = Globals.USER_DATA.ToolsAUsername;
                Globals.IS_ADMIN = check == "harrisb" || check == "mcnuttd" || check == "nordwell" ||
                    check == "carls" || check == "sethwalt";
            }
            catch (Exception error)
            {
                Error("An exception occured while loading user data", error);
                Globals.USER_DATA = null;
            }
        }

        private void CollectUserData()
        {
            bool saved = false;
            UserData data = new UserData();

            void ShowCollect()
            {
                var collect = new UserDataWindow(Globals.USER_DATA);
                collect.ShowDialog();
                saved = collect.Saved;
                data = collect.UserData;
            }

            Dispatcher.Invoke(ShowCollect);

            if (saved)
            {
                try
                {
                    Globals.USER_DATA = data;
                    var xml = Globals.SERIALIZE(Globals.USER_DATA);
                    var encrypt = Encrypt.EncryptString(xml);
                    var file = new StreamWriter(Globals.USER_DATA_FILE);
                    file.Write(encrypt);
                    file.Close();
                }
                catch (Exception error)
                {
                    Error("An exception occured while saving user data", error);
                }
            }
        }

        private void SetButtons(bool cancel)
        {
            void setButtons()
            {
                CollectButton.IsEnabled = Globals.MODE != Mode.OFFLINE && Globals.STATE == State.IDLE;
                EECCRButton.IsEnabled = Globals.MODE != Mode.OFFLINE && Globals.STATE == State.IDLE;
                AuditButton.IsEnabled = Globals.PRECS_LOADED && Globals.STATE == State.IDLE;
                RepairButton.IsEnabled = Globals.AUDIT_COMPLETE && Globals.MODE != Mode.OFFLINE &&
                    Globals.STATE == State.IDLE;
                LoadButton.IsEnabled = Globals.MODE == Mode.OFFLINE && Globals.STATE == State.IDLE;
                CancelButton.IsEnabled = cancel;

                LabsMenuItem.IsEnabled = Globals.STATE == State.IDLE;
                PullXLNButton.IsEnabled = Globals.MODE == Mode.LIVE && Globals.STATE == State.IDLE;
                RunLocalScriptButton.IsEnabled = Globals.MODE != Mode.OFFLINE && Globals.STATE == State.IDLE;
                RunToolsAScriptButton.IsEnabled = Globals.MODE != Mode.OFFLINE && Globals.STATE == State.IDLE;
                StageLabButton.IsEnabled = Globals.STATE == State.IDLE;
                AdminMenu.Visibility = Globals.IS_ADMIN ? Visibility.Visible : Visibility.Collapsed;
                ReinitLabInfo.IsEnabled = Globals.TOOLSA.Connected();
                JiraSearchdMenu.IsEnabled = Globals.DRCCD.Connected();
                FindJiraMenu.Visibility = Globals.IS_ADMIN ? Visibility.Visible : Visibility.Collapsed;
                FindJiraMenu.IsEnabled = Globals.DRCCD.Connected();
            }

            Dispatcher.Invoke(setButtons);
        }

        private void WindowClosing(object sender, CancelEventArgs args)
        {
            if (_refreshTimer.Enabled) _refreshTimer.Stop();

            if (Globals.TOOLSA.Connected()) Globals.TOOLSA.Disconnect();
            if (Globals.CM.Connected()) Globals.CM.Disconnect();
            if (Globals.DRCCD.Connected()) Globals.DRCCD.Disconnect();

            Environment.Exit(0);
        }

        private void LoadPRECs(string filename)
        {
            try
            {
                Globals.STATE = State.RUNNING;
                Globals.PROCESS = Process.LOADPRECS;
                AddStatus($"Loading PRECs from {filename}");

                var reader = new StreamReader(filename);
                var precs = reader.ReadToEnd();
                reader.Close();

                AddStatus("Cleaning loaded data");
                precs = Globals.CLEAN(precs);

                void setData() { DataBox.Text = precs; }
                Dispatcher.Invoke(setData);

                Globals.PRECS_LOADED = true;
                Globals.AUDIT_COMPLETE = false;
                AddStatus("Done loading PRECs");
            }
            catch (Exception error)
            {
                Error("Exception occured while loading PRECs", error);
            }
            Idle();
        }

        private void SetFormValues()
        {
            Globals.CM_RELEASE = (CMRelease)Enum.Parse(typeof(CMRelease), (string)CMReleaseBox.SelectedItem);
            Globals.STATION_AUDITS = StationAuditsCheck.IsChecked == true;
            Globals.TRUNK_AUDITS = TrunkAuditsCheck.IsChecked == true;
            Globals.ANNOUNCEMENT_AUDITS = AnnouncementAuditsCheck.IsChecked == true;
            Globals.CONNECT_PORT = SitePort.Text;
            Globals.CONNECT_IP = LabIP.Text;
            Globals.WYLD_STALLYN = WyldStallyn.IsChecked == true;
        }

        private void Click_WyldStallynMode(object sender, RoutedEventArgs args)
        {
            if (WyldStallyn.IsChecked == true)
                MessageBox.Show($"Wyld Stallyn Mode should only be used when you have a very large fixscript\nONLY USE IN LAB ENVIRONMENT\nYou will not get any terminal output. You will hammer TCM with commands.\nBE CAUTIOUS - Wyld Stallyns Rule!", "WARNING");
        }

        private void Click_Exit(object sender, RoutedEventArgs args)
        {
            AddStatus("[CLICK] Exit");
            Close();
        }

        private void Click_SetMode(object sender, RoutedEventArgs args)
        {
            if (LiveMode.IsChecked == true)
            {
                Globals.MODE = Mode.LIVE;
                CorruptionLiveModeOptions.Visibility = Visibility.Visible;
                CorruptionLabModeOptions.Visibility = Visibility.Collapsed;
            }
            else if (LabMode.IsChecked == true)
            {
                Globals.MODE = Mode.LAB;
                CorruptionLiveModeOptions.Visibility = Visibility.Collapsed;
                CorruptionLabModeOptions.Visibility = Visibility.Visible;
            }
            else
            {
                Globals.MODE = Mode.OFFLINE;
                CorruptionLiveModeOptions.Visibility = Visibility.Collapsed;
                CorruptionLabModeOptions.Visibility = Visibility.Collapsed;
            }

            SetButtons(false);
            if (Globals.CM.Connected()) MessageBox.Show("You are connected to a system already, please disconnect if connection is no longer required", "Warning");
        }

        private void Click_Cancel(object sender, RoutedEventArgs args)
        {
            AddStatus("[CLICK] Cancel");
            Globals.CANCEL = true;
        }

        private void Click_Load(object sender, RoutedEventArgs args)
        {
            AddStatus("[CLICK] Load");
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.LOADPRECS;
            SetButtons(false);

            var selector = new OpenFileDialog { Filter = "PREC Data | *.corr" };
            selector.ShowDialog();

            if (string.IsNullOrEmpty(selector.FileName))
            {
                Idle();
                return;
            }

            DataBox.Text = "";
            var load = new Task(() => LoadPRECs(selector.FileName));
            load.Start();
        }

        private void Click_Audit(object sender, RoutedEventArgs args)
        {
            AddStatus("[CLICK] Audit");
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.AUDIT;
            SetButtons(true);
            SetFormValues();
            Audits.ResetCounters();

            OutputBox.Text = "";
            PRECParser.InputData = DataBox.Text;

            var auditor = new Task(Auditor.Start);
            auditor.Start();
        }

        private void Click_Collect(object sender, RoutedEventArgs args)
        {
            AddStatus("[CLICK] Collect");
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.COLLECT;
            SetButtons(true);
            SetFormValues();
            OutputBox.Text = "";

            var collector = new Task(Collector.Start);
            collector.Start();
        }

        private void Click_Repair(object sender, RoutedEventArgs args)
        {
            AddStatus("[CLICK] Repair");
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.REPAIR;
            SetButtons(true);
            SetFormValues();
            OutputBox.Text = "";

            var repair = new Task(Repairer.Start);
            repair.Start();
        }

        private void Click_EECCR(object sender, RoutedEventArgs args)
        {
            AddStatus("[CLICK] EECCRs");
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.REPAIR;
            SetButtons(true);
            SetFormValues();
            OutputBox.Text = "";

            var eeccrAudit = new Task(EECCRAudit.Start);
            eeccrAudit.Start();
        }

        private void Click_PRECList(object sender, RoutedEventArgs args)
        {
            var preclist = new PRECListWindow();
            preclist.ShowDialog();
        }

        private void Click_OpenAssistant(
            object sender,
            RoutedEventArgs args)
        {
            var settings = new AssistantSettings
            {
                Enabled = true
            };

            var service = AssistantServiceFactory.Create(
                AssistantMode.Local,
                settings);

            var coordinator = new AssistantCoordinator(
                service,
                new AssistantRedactor(),
                settings);

            var context = _selectedAssistantContext ??
                new AssistantContext
                {
                    ApplicationVersion = Globals.VERSION(),
                    CmRelease = Globals.CM_RELEASE.ToString(),
                    RecordSizeStatus = "Unknown"
                };


            var assistant = new AssistantWindow(
                coordinator,
                context)
            {
                Owner = this
            };

            assistant.ShowDialog();
        }

        private void Click_FindPrec(
            object sender,
            RoutedEventArgs args)
        {
            var searchText = PrecSearchBox.Text?.Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                AddStatus("Enter a PREC type, UID, or search value.");
                PrecSearchBox.Focus();
                return;
            }

            var start = DataBox.Text.IndexOf(
                searchText,
                StringComparison.OrdinalIgnoreCase);

            if (start < 0)
            {
                AddStatus($"No PREC match found for: {searchText}");
                return;
            }

            var lineStart = DataBox.Text.LastIndexOf(
                '\n',
                start);

            lineStart = lineStart < 0
                ? 0
                : lineStart + 1;

            var lineEnd = DataBox.Text.IndexOf(
                '\n',
                start);

            lineEnd = lineEnd < 0
                ? DataBox.Text.Length
                : lineEnd;

            DataBox.Focus();
            DataBox.Select(
                lineStart,
                lineEnd - lineStart);

            DataBox.ScrollToLine(
                DataBox.GetLineIndexFromCharacterIndex(lineStart));
                AddStatus($"PREC match found for: {searchText}");
        }

        private void Click_UseSelectedPrec(
            object sender,
            RoutedEventArgs args)
        {
            var selectedPrec = DataBox.SelectedText;

            if (string.IsNullOrWhiteSpace(selectedPrec))
            {
                AddStatus("Select one or more PREC lines first.");
                DataBox.Focus();
                return;
            }

            var firstLine = selectedPrec
                .Split(new[] { "\r\n", "\n" },
                    StringSplitOptions.RemoveEmptyEntries)[0]
                .Trim();

            var fields = firstLine.Split(
                new[] { ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            var precType = fields.Length > 0
                ? fields[0]
                : "UNKNOWN";

            var settings = new AssistantSettings
            {
                Enabled = true
            };

            var service = AssistantServiceFactory.Create(
                AssistantMode.Local,
                settings);

            var coordinator = new AssistantCoordinator(
                service,
                new AssistantRedactor(),
                settings);

            var context = new AssistantContext
            {
                ApplicationVersion = Globals.VERSION(),
                CmRelease = Globals.CM_RELEASE.ToString(),
                PrecType = precType,
                StructureName = precType.ToLowerInvariant(),
                RawPrec = selectedPrec,
                RecordSizeStatus = "Unknown"
            };

            _selectedAssistantContext = context;

            var assistant = new AssistantWindow(
                coordinator,
                context)
            {
                Owner = this
            };

            assistant.ShowDialog();
        }

        private void Click_UserData(object sender, RoutedEventArgs args)
        {
            var collect = new Task(CollectUserData);
            collect.Start();
        }

        private void Click_ConnectToolsA(object sender, RoutedEventArgs args)
        {
            if (Globals.TOOLSA.Connected()) return;
            void connectFunc() { Globals.TOOLSA.Connect(); }
            var connect = new Task(connectFunc);
            connect.Start();
        }

        private void Click_DisconnectToolsA(object sender, RoutedEventArgs args)
        {
            if (!Globals.TOOLSA.Connected()) return;
            void connectFunc() { Globals.TOOLSA.Disconnect(); }
            var connect = new Task(connectFunc);
            connect.Start();
        }

        private void Click_ConnectDRCCD(object sender, RoutedEventArgs args)
        {
            if (Globals.DRCCD.Connected()) return;
            void connectFunc() { Globals.DRCCD.Connect(); }
            var connect = new Task(connectFunc);
            connect.Start();
        }

        private void Click_DisconnectDRCCD(object sender, RoutedEventArgs args)
        {
            if (!Globals.DRCCD.Connected()) return;
            void connectFunc() { Globals.DRCCD.Disconnect(); }
            var disconnect = new Task(connectFunc);
            disconnect.Start();
        }

        private void Click_DisconnectCM(object sender, RoutedEventArgs args)
        {
            if (!Globals.CM.Connected()) return;
            void connectFunc() { Globals.CM.Disconnect(); }
            var connect = new Task(connectFunc);
            connect.Start();
        }

        private void Click_RunLocalScript(object sender, RoutedEventArgs args)
        {
            AddStatus("[CLICK] Run Local Script");
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.LOADSCRIPT;
            SetButtons(false);
            SetFormValues();

            var selector = new OpenFileDialog();
            selector.ShowDialog();

            if (string.IsNullOrEmpty(selector.FileName))
            {
                Idle();
                return;
            }

            DataBox.Text = "";
            var run = new Task(() => ScriptRunner.RunLocal(selector.FileName));
            run.Start();
        }

        private void Click_RunToolsAScript(object sender, RoutedEventArgs args)
        {
            AddStatus("[CLICK] Run ToolsA Script");
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.LOADSCRIPT;
            SetButtons(false);
            SetFormValues();

            var selector = new CollectToolsAFileWindow();
            selector.ShowDialog();

            if (string.IsNullOrEmpty(selector.File))
            {
                Idle();
                return;
            }

            DataBox.Text = "";
            var run = new Task(() => ScriptRunner.RunToolsA(selector.File));
            run.Start();
        }

        private void Click_Labs(object sender, RoutedEventArgs args)
        {
            if (!Globals.LABS_LOADED) return;

            var output = new StringBuilder();
            output.AppendLine();
            output.AppendLine("CM Corruption Team Labs");
            output.AppendLine();
            output.AppendLine("VER   IP               ACTIVE");
            output.AppendLine("===   ===============  ======");

            foreach (var cm in Globals.CM_LABS)
            {
                output.AppendLine($"{cm.Version.PadRight(6, ' ')}{cm.IP.PadRight(17, ' ')}{cm.Active}");
            }

            output.AppendLine();
            output.AppendLine();
            output.AppendLine("MG CPE Team Labs");
            output.AppendLine();
            output.AppendLine("TYPE  IP               ACTIVE");
            output.AppendLine("====  ===============  ======");

            foreach (var mg in Globals.MG_LABS)
            {
                output.AppendLine($"{mg.Version.PadRight(6, ' ')}{mg.IP.PadRight(17, ' ')}{mg.Active}");
            }

            OutputBox.Text = output.ToString();
        }

        private void Click_PullXLN(object sender, RoutedEventArgs args)
        {
            AddStatus("[CLICK] Pull XLN");
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.PULLXLN;
            SetButtons(false);
            SetFormValues();

            var run = new Task(PullXLN.Start);
            run.Start();
        }

        private void Click_StageLab(object sender, RoutedEventArgs args)
        {
            var stager = new LabStagerWindow();
            stager.ShowDialog();

            if (!stager.Staging) return;

            OutputBox.Text = "";
            SetButtons(false);

            var run = new Task(LabStager.Start);
            run.Start();
        }

        private void Click_CMLabAdmin(object sender, RoutedEventArgs args)
        {
            var admin = new CMLabAdminWindow();
            admin.ShowDialog();
        }

        private void Click_MGLabAdmin(object sender, RoutedEventArgs args)
        {
            var admin = new MGLabAdminWindow();
            admin.ShowDialog();
        }

        private void Click_CMPatchAdmin(object sender, RoutedEventArgs args)
        {
            var admin = new CMPatchAdminWindow();
            admin.ShowDialog();
        }

        private void Click_ReinitLabInfo(object sender, RoutedEventArgs args)
        {
            Globals.TOOLSA.RetrieveLabInfo();
        }

        private void Click_JiraSearchd(object sender, RoutedEventArgs args)
        {
            var jirasearchd = new jirasearchdWindow();
            jirasearchd.ShowDialog();

            if (!jirasearchd.IsSearching) return;

            OutputBox.Text = "";
            void search()
            {
                var result = Globals.DRCCD.JiraSearchd(jirasearchd.SearchString, jirasearchd.IsStringSearch);
                AddOutput(result);
            }

            var searcher = new Task(search);
            searcher.Start();
        }

        private void Click_FindJira(object sender, RoutedEventArgs args)
        {
            var findjira = new findjiraWindow();
            findjira.ShowDialog();

            if (!findjira.IsSearching) return;

            OutputBox.Text = "";
            void search()
            {
                var result = Globals.DRCCD.FindJira(findjira.JIRA, findjira.CodeContext);
                AddOutput(result);
            }

            var searcher = new Task(search);
            searcher.Start();
        }

        private void Click_ScriptGenerator(object sender, RoutedEventArgs args)
        {
            var scriptGenerator = new ScriptGeneratorWindow();
            scriptGenerator.ShowDialog();
        }

        private void Fired_RefreshTimer(object sender, ElapsedEventArgs args)
        {
            _refreshTimer.Stop();
            var runtime = "";

            if (Globals.START_TIME == DateTime.MinValue)
            {
                runtime = "00:00:00";
            }
            else
            {
                var timer = DateTime.Now - Globals.START_TIME;
                runtime = $"{timer.Hours.ToString().PadLeft(2, '0')}:{timer.Minutes.ToString().PadLeft(2, '0')}:{timer.Seconds.ToString().PadLeft(2, '0')}";
            }

            void update()
            {
                StateLabel.Content = Globals.STATE;
                StateLabel.Foreground = Globals.STATE == State.RUNNING ?
                    new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#10B981")) :
                    (System.Windows.Media.SolidColorBrush)Application.Current.Resources["ThemeTextBrush"];

                ProcessLabel.Content = Globals.PROCESS;
                CorruptedLabel.Content = Audits.Corrupted;
                CorruptedStationsLabel.Content = Audits.CorruptedStations;
                CorruptedTrunksLabel.Content = Audits.CorruptedTrunks;
                CorruptedAnnouncementsLabel.Content = Audits.CorruptedAnnouncements;
                ManualFixesLabel.Content = Audits.ManualFixes;
                var redBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#EF4444"));
                CorruptedLabel.Foreground = Audits.Corrupted > 0 ? redBrush : (System.Windows.Media.SolidColorBrush)Application.Current.Resources["ThemeTextBrush"];

                RuntimeLabel.Content = $"{runtime}";
                ProgressLabel.Content = $"{Audits.Checked} / {Audits.ToCheck}";

                bool toolsAUp = Globals.TOOLSA.Connected();
                ToolsAStateLabel.Content = toolsAUp ? "UP" : "DOWN";
                ToolsAStateLabel.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(toolsAUp ? "#10B981" : "#EF4444"));

                bool drccdUp = Globals.DRCCD.Connected();
                DRCCDStateLabel.Content = drccdUp ? "UP" : "DOWN";
                DRCCDStateLabel.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(drccdUp ? "#10B981" : "#EF4444"));

                bool cmUp = Globals.CM.Connected();
                CMStateLabel.Content = cmUp ? "UP" : "DOWN";
                CMStateLabel.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(cmUp ? "#10B981" : "#EF4444"));

                ConnectToolsAMenu.IsEnabled = !Globals.TOOLSA.Connected();
                DisconnectToolsAMenu.IsEnabled = Globals.TOOLSA.Connected();
                DisconnectCMMenu.IsEnabled = Globals.CM.Connected();
                ConnectDRCCDMenu.IsEnabled = !Globals.DRCCD.Connected();
                DisconnectDRCCDMenu.IsEnabled = Globals.DRCCD.Connected();

                if (!string.IsNullOrEmpty(OutputText))
                {
                    lock (OutputLock)
                    {
                        OutputBox.Text += OutputText;
                        OutputText = "";
                    }
                    OutputLabel.Focus();
                    OutputBox.ScrollToEnd();
                }
            }

            Dispatcher.Invoke(update);
            _refreshTimer.Start();
        }

        private void LabIPChanged(object sender, TextChangedEventArgs args)
        {
            if (Globals.STATE == State.IDLE && Globals.CM != null && Globals.CM.Connected())
            {
                var disconnect = new Task(Globals.CM.Disconnect);
                disconnect.Start();
            }
        }

        private void Click_WebASGTest(object sender, RoutedEventArgs args)
        {
            Connections.WebASGConnection.GetResponse("Challenge: 10024-67323278              Product ID: 51bf031f13fc4fa0b5b3205952c2e82101");
        }
    }
}
