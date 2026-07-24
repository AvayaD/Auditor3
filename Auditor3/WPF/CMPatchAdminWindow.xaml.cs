/*
 * Auditor3 :: CMPatchAdminWindow
 * 
 * This class / XAML defines the window that is used to manage the list of CM patches
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System.IO;
using System.Windows;

namespace Auditor3 {
    public partial class CMPatchAdminWindow {
        // Constructor for creating the window
        public CMPatchAdminWindow() {
            InitializeComponent();
            LoadPatches();
        }

        // Method for loading the patch list
        private void LoadPatches() {
            Patches.Items.Clear();
            foreach (var patch in Globals.CM_PATCHES) Patches.Items.Add(patch);
        }

        // Click handler for the add button
        private void Click_Add(object sender, RoutedEventArgs args) {
            if (string.IsNullOrEmpty(Version.Text)) {
                MessageBox.Show("You have not entered the version");
                return;
            }
            if (string.IsNullOrEmpty(Patch.Text)) {
                MessageBox.Show("You have not entered the patch number");
                return;
            }
            if (string.IsNullOrEmpty(Release.Text)) {
                MessageBox.Show("You have not entered the release string");
                return;
            }
            if (string.IsNullOrEmpty(URL.Text)) {
                MessageBox.Show("You have not entered the URL");
                return;
            }
            var patch = new PatchInfo { Version = Version.Text, Patch = Patch.Text,
                Release = Release.Text, URL = URL.Text };
            Globals.CM_PATCHES.Add(patch);
            LoadPatches();
        }

        // Click handler for the remove button
        private void Click_Remove(object sender, RoutedEventArgs args) {
            if (Patches.SelectedIndex == -1) {
                MessageBox.Show("You have not selected a patch");
                return;
            }
            var patch = (PatchInfo) Patches.SelectedItem;
            var remove = Globals.CM_PATCHES.Find(a => a.Patch == patch.Patch);
            Globals.CM_PATCHES.Remove(remove);
            LoadPatches();
        }

        // Click handler for the save button
        private void Click_Save(object sender, RoutedEventArgs args) {
            Globals.CM_PATCHES.Sort((x, y) => x.Patch.CompareTo(y.Patch));
            var patches = Globals.SERIALIZE(Globals.CM_PATCHES);
            var file = new StreamWriter(Globals.CM_PATCHES_LOCAL_FILE);
            file.Write(patches);
            file.Close();
            Globals.TOOLSA.SendFile(Globals.CM_PATCHES_LOCAL_FILE, Globals.CM_PATCHES_FILE);

            Globals.GUI.AddStatus("Updating other ToolsA server");
            var server = Globals.TOOLSA.CONNECTED_NEW ? "st3tds04.us1.avaya.com" : "pltlavmap01.avaya.com";
            var cmpatches = Globals.TOOLSA.CONNECTED_NEW ? ToolsAConnection.CM_PATCHES_FILE_OLD : ToolsAConnection.CM_PATCHES_FILE_NEW;
            var toolsa = new ToolsAConnection();

            toolsa.Connect(server);
            if (!toolsa.Connected()) {
                Globals.GUI.Error("Failed to connect to other ToolsA server");
                toolsa = null;
                return;
            }

            toolsa.SendFile(Globals.CM_PATCHES_LOCAL_FILE, cmpatches);
            toolsa.Disconnect();
            toolsa = null;

            MessageBox.Show("CM patch list updated");
        }
    }
}
