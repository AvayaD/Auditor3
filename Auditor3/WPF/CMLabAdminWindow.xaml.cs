/*
 * Auditor3 :: CMLabAdminWindow
 * 
 * This class / XAML defines the window that is used to manage the list of CM labs
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System.IO;
using System.Windows;

namespace Auditor3 {
    public partial class CMLabAdminWindow {
        // Constructor for creating the window
        public CMLabAdminWindow() {
            InitializeComponent();
            LoadLabs();
        }

        // Method for loading the labs into the list
        private void LoadLabs() {
            Labs.Items.Clear();
            foreach (var lab in Globals.CM_LABS) Labs.Items.Add(lab);
        }

        // Click handler for the add button
        private void Click_Add(object sender, RoutedEventArgs args) {
            if (string.IsNullOrEmpty(Version.Text)) {
                MessageBox.Show("You did not enter a version");
                return;
            }
            if (!Globals.CHECK_IP(IP.Text)) {
                MessageBox.Show("You did not enter a valid IP");
                return;
            }
            var check = Globals.CM_LABS.Find(a => a.IP == IP.Text);
            if (check != null) {
                MessageBox.Show("Lab IP already exists in list");
                return;
            }
            var lab = new LabInfo { Version = Version.Text, IP = IP.Text, Active = true };
            Labs.Items.Add(lab);
            Globals.CM_LABS.Add(lab);
        }

        // Click handler for the remove button
        private void Click_Remove(object sender, RoutedEventArgs args) {
            if (Labs.SelectedIndex == -1) {
                MessageBox.Show("You do not have a lab selected");
                return;
            }
            var lab = (LabInfo) Labs.SelectedItem;
            Labs.Items.Remove(lab);
            var remove = Globals.CM_LABS.Find(a => a.IP == lab.IP);
            Globals.CM_LABS.Remove(remove);
        }

        // Click handler for the active button
        private void Click_Active(object sender, RoutedEventArgs args) {
            if (Labs.SelectedIndex == -1) {
                MessageBox.Show("You do not have a lab selected");
                return;
            }
            var lab = (LabInfo)Labs.SelectedItem;
            var update = Globals.CM_LABS.Find(a => a.IP == lab.IP);
            update.Active = true;
            LoadLabs();
        }

        // Click handler for the inactive button
        private void Click_Inactive(object sender, RoutedEventArgs args) {
            if (Labs.SelectedIndex == -1) {
                MessageBox.Show("You do not have a lab selected");
                return;
            }
            var lab = (LabInfo)Labs.SelectedItem;
            var update = Globals.CM_LABS.Find(a => a.IP == lab.IP);
            update.Active = false;
            LoadLabs();
        }

        // Click handler for the save button
        private void Click_Save(object sender, RoutedEventArgs args) {
            Globals.CM_LABS.Sort((x, y) => x.IP.CompareTo(y.IP));
            var labs = Globals.SERIALIZE(Globals.CM_LABS);
            var file = new StreamWriter(Globals.CM_LABS_LOCAL_FILE);
            file.Write(labs);
            file.Close();
            Globals.TOOLSA.SendFile(Globals.CM_LABS_LOCAL_FILE, Globals.CM_LABS_FILE);

            Globals.GUI.AddStatus("Updating other ToolsA server");
            var server = Globals.TOOLSA.CONNECTED_NEW ? "st3tds04.us1.avaya.com" : "pltlavmap01.avaya.com";
            var cmlabs = Globals.TOOLSA.CONNECTED_NEW ? ToolsAConnection.CM_LABS_FILE_OLD : ToolsAConnection.CM_LABS_FILE_NEW;
            var toolsa = new ToolsAConnection();

            toolsa.Connect(server);
            if (!toolsa.Connected()) { 
                Globals.GUI.Error("Failed to connect to other ToolsA server");
                toolsa = null;
                return;
            }

            toolsa.SendFile(Globals.CM_LABS_LOCAL_FILE, cmlabs);
            toolsa.Disconnect();
            toolsa = null;

            MessageBox.Show("CM lab list updated");
        }
    }
}
