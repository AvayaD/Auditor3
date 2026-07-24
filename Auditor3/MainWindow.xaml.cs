/*
 * Auditor3 :: MainWindow
 * 
 * This class / XAML defines the primary GUI and user interaction functionality.
 * 
 * Auditor3 is developed and maintained by David McNutt
 */

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

namespace Auditor3 {
    public partial class MainWindow {

        private static Timer _refreshTimer;
        private string LoadPRECsFile;
        private string OutputText;
        private object OutputLock;

        // Constructor for creating the window
        public MainWindow() {
            // Initialize the GUI elements and start the GUI initialization
            InitializeComponent();
            Startup();
        }

        // Constructor for creating the window with passed in precs
        public MainWindow(string precs) {
            // Initialize the GUI elements
            InitializeComponent();

            // Set the PRECs and flag
            LoadPRECsFile = precs;

            // Start the GUI initialization
            Startup();
        }

        // This method handles the common startup
        private void Startup() {
            // Perform basic GUI setup
            WindowState = WindowState.Maximized;
            Title = $"Corruption Auditor v{Globals.VERSION()}";

            // Populate the CM release dropdown and select CM 6.3 as default selected
            foreach (var release in Enum.GetNames(typeof(CMRelease)))
                CMReleaseBox.Items.Add(release);
            CMReleaseBox.SelectedItem = CMRelease.CM6_3.ToString();

            OutputText = "";
            OutputLock = new object();

            // Start the background initialization
            var init = new Task(Initialize);
            init.Start();
        }

        // This method adds a message to the status box
        internal void AddStatus(string status) {
            // Add an asterisk to the message
            status = $"* {status}";

            // Create a local function and to add the message to the status box 
            // and pass it to the dispatcher
            void addStatus() {
                OutputLabel.Focus();
                StatusBox.Text += status + Environment.NewLine;
                StatusBox.ScrollToEnd();
            }
            Dispatcher.Invoke(addStatus);
        }

        // Tnis method adds a message to the output box
        internal void AddOutput(string message) {
            void addOutput() {
                lock (OutputLock) { OutputText += message + Environment.NewLine; }
            }
            Dispatcher.Invoke(addOutput);
        }

        // This method gets the output details
        internal string GetOutput() {
            // Create a variable to store the output
            var output = "";

            // Use the dispatcher to get the output data
            void getOutput() { output = OutputBox.Text; }
            Dispatcher.Invoke(getOutput);

            // Return the output
            return output;
        }

        // This method displays an error message
        internal void Error(string message) {
            AddStatus(message);
        }

        // This method is used to handle errors when there is an exception
        internal void Error(string message, Exception error) {
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

            while (error.InnerException != null) {
                error = error.InnerException;
                log.AppendLine($"INNER   : {error.Message}");
            }

            var logfile = Globals.REPORT("crash");
            var writer = new StreamWriter(logfile);
            writer.Write(log.ToString());
            writer.Close();

            AddStatus(message);
            AddStatus($"Crash report generated at {logfile}");

            // Don't upload a crash report if this is a dev build
            if (!Globals.VERSION_DEV && Globals.TOOLSA.Connected()) {
                var report = $"{Globals.CRASH_FOLDER_TOOLSA}crash_{Globals.TIMESLICE()}_{Globals.USER_DATA.ToolsAUsername}.log";
                if (Globals.TOOLSA.SendFile(logfile, report)) {
                    AddStatus($"Crash report uploaded to {report}");
                }
            }
        }

        // This method sets all the variables to an idle state
        internal void Idle() {
            // Set the state and process
            Globals.STATE = State.IDLE;
            Globals.PROCESS = Process.NONE;

            // Reset values
            Globals.START_TIME = DateTime.MinValue;
            Audits.Checked = 0;
            Audits.ToCheck = 0;
            Globals.CANCEL = false;

            // Set the button states
            void setButtons() { SetButtons(false); }
            Dispatcher.Invoke(setButtons);
        }

        // This method is used to update the output box with collected PRECs
        internal void SetPRECs(string precs) {
            void update() { DataBox.Text = precs; }
            Dispatcher.Invoke(update);
        }

        // This method handles all the non-GUI thread related initialization and is 
        // run in as a background task
        private void Initialize() {
            // Add status message and set state/process to initializing
            AddStatus($"Auditor is initializing on v{Globals.VERSION()}");
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.INITIALIZING;

            // Set reference to the GUI
            Globals.GUI = this;

            // Ensure all necessary folders exist
            if (!Directory.Exists(Globals.REPORT_DIR)) Directory.CreateDirectory(Globals.REPORT_DIR);

            // Default values
            Globals.USER_DATA = null;
            Globals.MODE = Mode.OFFLINE;
            Globals.PRECS_LOADED = false;
            Globals.AUDIT_COMPLETE = false;
            Globals.CANCEL = false;
            Globals.START_TIME = DateTime.MinValue;
            Audits.ResetCounters();
            Locations.Locations.INITIALIZE();

            // Initialize the database and userdata
            Database.Initialize();
            InitializeUserData();

            // Set values from the user data
            void setUserValues() {
                SitePort.Text = Globals.USER_DATA.DefaultLivePort;
                LabIP.Text = Globals.USER_DATA.DefaultLabIP;
            }
            Dispatcher.Invoke(setUserValues);

            // Initialize the connections
            Globals.TOOLSA = new ToolsAConnection();
            Globals.DRCCD = new DRCCDConnection();
            Globals.CM = new CMConnection();

            Globals.CM_LABS = new List<LabInfo>();
            Globals.MG_LABS = new List<LabInfo>();
            Globals.CM_PATCHES = new List<PatchInfo>();

            if (CURRENT().HasToolsA()) {
                // Connect to ToolsA
                Globals.TOOLSA.Connect();
                Globals.TOOLSA.CheckUpdates();
                Globals.TOOLSA.RetrieveLabInfo();
            }

            // Connect to DRCCD if we are not in the AWS environmnet
            if (CURRENT().HasDRCCD()) {
                Globals.DRCCD.Connect();
            }

            // Initialize and start the GUI refresh timer
            _refreshTimer = new Timer(Globals.REFRESH_TIMER);
            _refreshTimer.Elapsed += Fired_RefreshTimer;
            _refreshTimer.Start();

            // Handle loading PRECs during startup if we are loading a .corr directly
            if (!string.IsNullOrEmpty(LoadPRECsFile)) {
                LoadPRECs(LoadPRECsFile);
                LoadPRECsFile = null;
            }

            // Add a status message indicating initialization is complete and go idle
            AddStatus("Auditor is now ready for use");
            Idle();
        }

        // This method initialized the user data
        private void InitializeUserData() {
            if (File.Exists(Globals.USER_DATA_FILE)) { LoadUserData(); }
            if (Globals.USER_DATA == null) {
                Globals.USER_DATA = new UserData();
                CollectUserData();
            }
        }

        // This method loads the user data from file
        private void LoadUserData() {
            try {
                AddStatus("Reading user data file");
                var file = new StreamReader(Globals.USER_DATA_FILE);
                var data = file.ReadToEnd();
                file.Close();
                var user = Encrypt.DecryptString(data);
                Globals.USER_DATA = Globals.DESERIALIZE<UserData>(user);
                var check = Globals.USER_DATA.ToolsAUsername;
                Globals.IS_ADMIN = check == "harrisb" || check == "mcnuttd" || check == "nordwell" ||
                    check == "carls" || check == "sethwalt";
            } catch (Exception error) {
                Error("An exception occured while loading user data", error);
                Globals.USER_DATA = null;
            }
        }

        // This method opens the user data window to collect details from the user
        private void CollectUserData() {
            bool saved = false;
            UserData data = new UserData();
            void ShowCollect() {
                var collect = new UserDataWindow(Globals.USER_DATA);
                collect.ShowDialog();
                saved = collect.Saved;
                data = collect.UserData;
            }
            Dispatcher.Invoke(ShowCollect);

            if (saved) {
                try {
                    Globals.USER_DATA = data;
                    var xml = Globals.SERIALIZE(Globals.USER_DATA);
                    var encrypt = Encrypt.EncryptString(xml);
                    var file = new StreamWriter(Globals.USER_DATA_FILE);
                    file.Write(encrypt);
                    file.Close();
                } catch (Exception error) {
                    Error("An exception occured while saving user data", error);
                }
            }
        }

        // This method sets the button states, you pass in whether the cancel button
        // should be active or not
        private void SetButtons(bool cancel) {
            void setButtons() {
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

        // This method cleans up when the application is closing
        private void WindowClosing(object sender, CancelEventArgs args) {
            // Stop the refresh timer
            if (_refreshTimer.Enabled) _refreshTimer.Stop();

            // Check and close the connections to CM/ToolsA
            if (Globals.TOOLSA.Connected()) Globals.TOOLSA.Disconnect();
            if (Globals.CM.Connected()) Globals.CM.Disconnect();
            if (Globals.DRCCD.Connected()) Globals.DRCCD.Disconnect();

            // Exit the environment
            Environment.Exit(0);
        }

        // This method is used to load PRECs from a file and is run in a background task
        private void LoadPRECs(string filename) {
            try {
                // Set the state and add a status message
                Globals.STATE = State.RUNNING;
                Globals.PROCESS = Process.LOADPRECS;
                AddStatus($"Loading PRECs from {filename}");

                // Read the data from the file
                var reader = new StreamReader(filename);
                var precs = reader.ReadToEnd();
                reader.Close();

                // Add a status message and clean the data
                AddStatus("Cleaning loaded data");
                precs = Globals.CLEAN(precs);

                // Put the PRECs into the data box using a dispatcher action
                void setData() { DataBox.Text = precs; }
                Dispatcher.Invoke(setData);

                // Set the flags, add a status message, and go idle
                Globals.PRECS_LOADED = true;
                Globals.AUDIT_COMPLETE = false;
                AddStatus("Done loading PRECs");
            } catch (Exception error) {
                Error("Exception occured while loading PRECs", error);
            }

            Idle();
        }

        // This method is used to set the environment values from the form
        private void SetFormValues() {
            Globals.CM_RELEASE = (CMRelease)Enum.Parse(typeof(CMRelease), (string)CMReleaseBox.SelectedItem);
            Globals.STATION_AUDITS = StationAuditsCheck.IsChecked == true;
            Globals.TRUNK_AUDITS = TrunkAuditsCheck.IsChecked == true;
            Globals.ANNOUNCEMENT_AUDITS = AnnouncementAuditsCheck.IsChecked == true;
            Globals.CONNECT_PORT = SitePort.Text;
            Globals.CONNECT_IP = LabIP.Text;
            Globals.WYLD_STALLYN = WyldStallyn.IsChecked == true;
        }

        // Click handler for when Wyld Stallyn Mode menu item is clicked
        private void Click_WyldStallynMode(object sender, RoutedEventArgs args) {
            if (WyldStallyn.IsChecked == true)
                MessageBox.Show($"Wyld Stallyn Mode should only be used when you have a very large fixscriptcONLY USE IN LAB ENVIRONMENT{Environment.NewLine}You will not get any terminal output. You will hammer TCM with commands.{Environment.NewLine}BE CAUTIOUS - Wyld Stallyns Rule!", "WARNING");
        }

        // Click handler for the exit menu item
        private void Click_Exit(object sender, RoutedEventArgs args) {
            // Add a status message and close the window
            AddStatus("[CLICK] Exit");
            Close();
        }

        // Click handler for the mode radio buttons
        private void Click_SetMode(object sender, RoutedEventArgs args) {
            // Set the mode variable and show the correct options panel
            if (LiveMode.IsChecked == true) {
                Globals.MODE = Mode.LIVE;
                CorruptionLiveModeOptions.Visibility = Visibility.Visible;
                CorruptionLabModeOptions.Visibility = Visibility.Collapsed;
            } else if (LabMode.IsChecked == true) {
                Globals.MODE = Mode.LAB;
                CorruptionLiveModeOptions.Visibility = Visibility.Collapsed;
                CorruptionLabModeOptions.Visibility = Visibility.Visible;
            } else {
                Globals.MODE = Mode.OFFLINE;
                CorruptionLiveModeOptions.Visibility = Visibility.Collapsed;
                CorruptionLabModeOptions.Visibility = Visibility.Collapsed;
            }

            // Set the button states
            SetButtons(false);

            // Toss a warning message up if connected to a CM already
            if (Globals.CM.Connected()) MessageBox.Show("You are connected to a system already, please disconnect if connection is no longer required", "Warning");
        }

        // Click handler for the cancel button
        private void Click_Cancel(object sender, RoutedEventArgs args) {
            // Add a status message and set the cancel flag
            AddStatus("[CLICK] Cancel");
            Globals.CANCEL = true;
        }

        // Click handler for the load button
        private void Click_Load(object sender, RoutedEventArgs args) {
            // Add a status message
            AddStatus("[CLICK] Load");

            // Set the state and process, and set the button states
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.LOADPRECS;
            SetButtons(false);

            // Open a filename selector
            var selector = new OpenFileDialog { Filter = "PREC Data | *.corr" };
            selector.ShowDialog();

            // Make sure a file was selected
            if (string.IsNullOrEmpty(selector.FileName)) {
                Idle();
                return;
            }

            DataBox.Text = "";

            // Create and start the background task to load the PRECs
            var load = new Task(() => LoadPRECs(selector.FileName));
            load.Start();
        }

        // Click handler for the audit button
        private void Click_Audit(object sender, RoutedEventArgs args) {
            // Add a status message
            AddStatus("[CLICK] Audit");

            // Set the state/process and button states
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.AUDIT;
            SetButtons(true);

            // Set the form values and clear the counters
            SetFormValues();
            Audits.ResetCounters();

            // Clear the output box and copy the PREC data to the parser
            OutputBox.Text = "";
            PRECParser.InputData = DataBox.Text;

            // Create and start a background task for the auditor
            var auditor = new Task(Auditor.Start);
            auditor.Start();
        }

        // Click handler for the collect button
        private void Click_Collect(object sender, RoutedEventArgs args) {
            // Add a status message
            AddStatus("[CLICK] Collect");

            // Set the state/process and button states
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.COLLECT;
            SetButtons(true);

            // Set the form values and clear the counters
            SetFormValues();
            OutputBox.Text = "";

            // Start the collector in a background task
            var collector = new Task(Collector.Start);
            collector.Start();
        }

        // Click handler for the repair button
        private void Click_Repair(object sender, RoutedEventArgs args) {
            // Add a status message
            AddStatus("[CLICK] Repair");

            // Set the state/process and button states
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.REPAIR;
            SetButtons(true);

            // Set the form values and clear the counters
            SetFormValues();
            OutputBox.Text = "";

            var repair = new Task(Repairer.Start);
            repair.Start();
        }

        // Click handler for the EECCRs button
        private void Click_EECCR(object sender, RoutedEventArgs args) {
            // Add a status message
            AddStatus("[CLICK] EECCRs");

            // Set the state/process and button states
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.REPAIR;
            SetButtons(true);

            // Set the form values and clear the counters
            SetFormValues();
            OutputBox.Text = "";

            var eeccrAudit = new Task(EECCRAudit.Start);
            eeccrAudit.Start();
        }

        // Click handler for the PREC list button
        private void Click_PRECList(object sender, RoutedEventArgs args) {
            var preclist = new PRECListWindow();
            preclist.ShowDialog();
        }

        // Click handler for the user data button
        private void Click_UserData(object sender, RoutedEventArgs args) {
            var collect = new Task(CollectUserData);
            collect.Start();
        }

        // Click handler for the connect ToolsA menu item
        private void Click_ConnectToolsA(object sender, RoutedEventArgs args) {
            if (Globals.TOOLSA.Connected()) return;
            void connectFunc() { Globals.TOOLSA.Connect(); }
            var connect = new Task(connectFunc);
            connect.Start();
        }

        // Click handler for the disconnect ToolsA menu item
        private void Click_DisconnectToolsA(object sender, RoutedEventArgs args) {
            if (!Globals.TOOLSA.Connected()) return;
            void connectFunc() { Globals.TOOLSA.Disconnect(); }
            var connect = new Task(connectFunc);
            connect.Start();            
        }

        // Click handler for the connect DRCCD menu item
        private void Click_ConnectDRCCD(object sender, RoutedEventArgs args) {
            if (Globals.DRCCD.Connected()) return;
            void connectFunc() { Globals.DRCCD.Connect(); }
            var connect = new Task(connectFunc);
            connect.Start();
        }

        // Click handler for the disconnect DRCCD menu item
        private void Click_DisconnectDRCCD(object sender, RoutedEventArgs args) {
            if (!Globals.DRCCD.Connected()) return;
            void connectFunc() { Globals.DRCCD.Disconnect(); }
            var disconnect = new Task(connectFunc);
            disconnect.Start();
        }

        // Click handler for the disconnect CM menu item
        private void Click_DisconnectCM(object sender, RoutedEventArgs args) {
            if (!Globals.CM.Connected()) return;
            void connectFunc() { Globals.CM.Disconnect(); }
            var connect = new Task(connectFunc);
            connect.Start();            
        }

        // Click handler for the run local script menu item
        private void Click_RunLocalScript(object sender, RoutedEventArgs args) {
            // Add a status message
            AddStatus("[CLICK] Run Local Script");
            
            // Set the state and process, and set the button states
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.LOADSCRIPT;
            SetButtons(false);
            SetFormValues();

            // Open a filename selector
            var selector = new OpenFileDialog();
            selector.ShowDialog();

            // Make sure a file was selected
            if (string.IsNullOrEmpty(selector.FileName)) {
                Idle();
                return;
            }

            DataBox.Text = "";

            // Create and start the background task to run the script
            var run = new Task(() => ScriptRunner.RunLocal(selector.FileName));
            run.Start();
        }

        // Click handler for the run ToolsA script menu item
        private void Click_RunToolsAScript(object sender, RoutedEventArgs args) {
            // Add a status message
            AddStatus("[CLICK] Run ToolsA Script");

            // Set the state and process, and set the button states
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.LOADSCRIPT;
            SetButtons(false);
            SetFormValues();

            // Open a filename selector
            var selector = new CollectToolsAFileWindow();
            selector.ShowDialog();

            // Make sure a file was selected
            if (string.IsNullOrEmpty(selector.File)) {
                Idle();
                return;
            }

            DataBox.Text = "";

            // Create and start the background task to run the script
            var run = new Task(() => ScriptRunner.RunToolsA(selector.File));
            run.Start();
        }

        // Click handler for the labs menu item
        private void Click_Labs(object sender, RoutedEventArgs args) {
            if (!Globals.LABS_LOADED) return;
            var output = new StringBuilder();
            output.AppendLine();
            output.AppendLine("CM Corruption Team Labs");
            output.AppendLine();
            output.AppendLine("VER   IP               ACTIVE");
            output.AppendLine("===   ===============  ======");
            foreach (var cm in Globals.CM_LABS) {
                output.AppendLine($"{cm.Version.PadRight(6, ' ')}{cm.IP.PadRight(17, ' ')}{cm.Active}");
            }
            output.AppendLine();
            output.AppendLine();
            output.AppendLine("MG CPE Team Labs");
            output.AppendLine();
            output.AppendLine("TYPE  IP               ACTIVE");
            output.AppendLine("====  ===============  ======");
            foreach (var mg in Globals.MG_LABS) {
                output.AppendLine($"{mg.Version.PadRight(6, ' ')}{mg.IP.PadRight(17, ' ')}{mg.Active}");
            }
            OutputBox.Text = output.ToString();
        }
        
        // Click handler for the pull XLN button
        private void Click_PullXLN(object sender, RoutedEventArgs args) {
            // Add a status message
            AddStatus("[CLICK] Pull XLN");

            // Set the state and process, and set the button states
            Globals.STATE = State.INIT;
            Globals.PROCESS = Process.PULLXLN;
            SetButtons(false);
            SetFormValues();

            var run = new Task(PullXLN.Start);
            run.Start();
        }

        // Click handler for the stage lab button
        private void Click_StageLab(object sender, RoutedEventArgs args) {
            // Open a lab stager window to collect the details
            var stager = new LabStagerWindow();
            stager.ShowDialog();

            if (!stager.Staging) return;

            OutputBox.Text = "";
            SetButtons(false);

            var run = new Task(LabStager.Start);
            run.Start();
        }

        // Click handler for the CM lab admin button
        private void Click_CMLabAdmin(object sender, RoutedEventArgs args) {
            var admin = new CMLabAdminWindow();
            admin.ShowDialog();
        }

        // Click handler for the MG lab admin button
        private void Click_MGLabAdmin(object sender, RoutedEventArgs args) {
            var admin = new MGLabAdminWindow();
            admin.ShowDialog();
        }

        // Click handler for the CM patch admin button
        private void Click_CMPatchAdmin(object sender, RoutedEventArgs args) {
            var admin = new CMPatchAdminWindow();
            admin.ShowDialog();
        }

        // Click handler for the reinit lab info button
        private void Click_ReinitLabInfo(object sender, RoutedEventArgs args) {
            Globals.TOOLSA.RetrieveLabInfo();
        }

        // Click handler for the jirasearchd menu item
        private void Click_JiraSearchd(object sender, RoutedEventArgs args) {
            var jirasearchd = new jirasearchdWindow();
            jirasearchd.ShowDialog();
            if (!jirasearchd.IsSearching) return;
            OutputBox.Text = "";
            void search() {
                var result = Globals.DRCCD.JiraSearchd(jirasearchd.SearchString, jirasearchd.IsStringSearch);
                AddOutput(result);
            }
            var searcher = new Task(search);
            searcher.Start();
        }

        // Click handler for the findjira menu item
        private void Click_FindJira(object sender, RoutedEventArgs args) {
            var findjira = new findjiraWindow();
            findjira.ShowDialog();
            if (!findjira.IsSearching) return;
            OutputBox.Text = "";
            void search() {
                var result = Globals.DRCCD.FindJira(findjira.JIRA, findjira.CodeContext);
                AddOutput(result);
            }
            var searcher = new Task(search);
            searcher.Start();
        }

        // Click handler for the script generator menu item
        private void Click_ScriptGenerator(object sender, RoutedEventArgs args) {
            var scriptGenerator = new ScriptGeneratorWindow();
            scriptGenerator.ShowDialog();
        }

        // This method handles updating the GUI when the timer fires
        private void Fired_RefreshTimer(object sender, ElapsedEventArgs args) {
            // Temporarily stop the refresh timer
            _refreshTimer.Stop();

            // Create a string for the runtime
            var runtime = "";

            if (Globals.START_TIME == DateTime.MinValue) {
                runtime = "00:00:00";
            } else {
                var timer = DateTime.Now - Globals.START_TIME;
                runtime = $"{timer.Hours.ToString().PadLeft(2, '0')}:{timer.Minutes.ToString().PadLeft(2, '0')}:" +
                    $"{timer.Seconds.ToString().PadLeft(2, '0')}";
            }

            // Update the GUI elements using the dispatcher
            void update() {
                StateLabel.Content = Globals.STATE;
                ProcessLabel.Content = Globals.PROCESS;
                CorruptedLabel.Content = Audits.Corrupted;
                CorruptedStationsLabel.Content = Audits.CorruptedStations;
                CorruptedTrunksLabel.Content = Audits.CorruptedTrunks;
                CorruptedAnnouncementsLabel.Content = Audits.CorruptedAnnouncements;
                ManualFixesLabel.Content = Audits.ManualFixes;
                RuntimeLabel.Content = $"{runtime}";
                ProgressLabel.Content = $"{Audits.Checked} / {Audits.ToCheck}";
                ToolsAStateLabel.Content = Globals.TOOLSA.Connected() ? "UP" : "DOWN";
                DRCCDStateLabel.Content = Globals.DRCCD.Connected() ? "UP" : "DOWN";
                CMStateLabel.Content = Globals.CM.Connected() ? "UP" : "DOWN";
                ConnectToolsAMenu.IsEnabled = !Globals.TOOLSA.Connected();
                DisconnectToolsAMenu.IsEnabled = Globals.TOOLSA.Connected();
                DisconnectCMMenu.IsEnabled = Globals.CM.Connected();
                ConnectDRCCDMenu.IsEnabled = !Globals.DRCCD.Connected();
                DisconnectDRCCDMenu.IsEnabled = Globals.DRCCD.Connected();

                if (!string.IsNullOrEmpty(OutputText)) {
                    lock (OutputLock) {
                        OutputBox.Text += OutputText;
                        OutputText = "";
                    }

                    OutputLabel.Focus();
                    OutputBox.ScrollToEnd();
                }
            }
            Dispatcher.Invoke(update);

            // Restart the refresh timer
            _refreshTimer.Start();
        }

        // Event handler when the text in the LAB IP textbox changes
        private void LabIPChanged(object sender, TextChangedEventArgs args) {            
            if (Globals.STATE == State.IDLE && Globals.CM != null && Globals.CM.Connected()) {
                var disconnect = new Task(Globals.CM.Disconnect);
                disconnect.Start();
            }
        }

        // WebASG test
        private void Click_WebASGTest(object sender, RoutedEventArgs args) {
            Connections.WebASGConnection.GetResponse("Challenge: 10024-67323278              Product ID: 51bf031f13fc4fa0b5b3205952c2e82101");
        }
    }
}
