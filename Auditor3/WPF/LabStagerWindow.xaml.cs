/*
 * Auditor3 :: LabStagerWindow
 * 
 * This class / XAML defines the window that is used to set the options for 
 * staging a lab system
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Auditor3 {
    public partial class LabStagerWindow {

        internal bool Staging;

        // Constructor for creating the window
        public LabStagerWindow() {
            InitializeComponent();

            // Populate the releases box
            var releases = new List<string>();
            foreach (var lab in Globals.CM_LABS) {
                var check = releases.Find(a => a == lab.Version);
                if (check == null) releases.Add(lab.Version);
            }
            releases = releases.OrderBy(a => a).ToList();
            foreach (var release in releases) Release.Items.Add(release);
            Release.SelectedIndex = 0;
            Click_ReleaseChanged(this, null);
        }

        // Handler for when the CM release is changed
        private void Click_ReleaseChanged(object sender, SelectionChangedEventArgs args) {
            Lab.Items.Clear();
            Patch.Items.Clear();

            var release = Release.SelectedItem.ToString();
            if (string.IsNullOrEmpty(release)) return;

            var labs = Globals.CM_LABS.FindAll(a => a.Version == release && a.Active);
            foreach (var lab in labs) Lab.Items.Add(lab.IP);

            var patches = Globals.CM_PATCHES.FindAll(a => a.Version == release);
            foreach (var patch in patches) Patch.Items.Add($"{patch.Patch} | {patch.Release}");
        }

        // Handler for when the patch checkbox changes state
        private void Click_Patch(object sender, RoutedEventArgs args) {
            Patch.IsEnabled = PatchCheck.IsChecked == true;
        }

        // Handler for when the XLN checkbox changes state
        private void Click_XLN(object sender, RoutedEventArgs args) {
            LocalPC.IsEnabled = XLN.IsChecked == true;
            ToolsA.IsEnabled = XLN.IsChecked == true;
            XLNFile.IsEnabled = XLN.IsChecked == true;
            XLNBackup.IsEnabled = XLN.IsChecked == true;
            Filename.IsEnabled = XLN.IsChecked == true;
            Browse.IsEnabled = XLN.IsChecked == true;
        }

        // Handler for when the XLN location changes
        private void Click_XLNLocation(object sender, RoutedEventArgs args) {
            if (LocalPC.IsChecked == true) {
                Browse.Visibility = Visibility.Visible;
                Filename.Width = 250;
            } else {
                Browse.Visibility = Visibility.Collapsed;
                Filename.Width = 325;
            }
        }

        // Click handler for the cancel button
        private void Click_Cancel(object sender, RoutedEventArgs args) {
            Close();
        }

        // Click handler for the browse button
        private void Click_Browse(object sender, RoutedEventArgs args) {
            // Open a filename selector
            var selector = new OpenFileDialog();
            selector.ShowDialog();

            // Make sure a file was selected
            if (string.IsNullOrEmpty(selector.FileName)) return;

            Filename.Text = selector.FileName;
        }

        // Click handler for the stage button
        private void Click_Stage(object sender, RoutedEventArgs args) {
            if (Release.SelectedIndex == -1) {
                MessageBox.Show("Select CM release");
                return;
            }

            if (Lab.SelectedIndex == -1) {
                MessageBox.Show("Select target lab");
                return;
            }

            if (PatchCheck.IsChecked == false && XLN.IsChecked == false) {
                MessageBox.Show("Select Patch / XLN / Both");
                return;
            }

            if (PatchCheck.IsChecked == true && Patch.SelectedIndex == -1) {
                MessageBox.Show("Select patch to load");
                return;
            }

            if (XLN.IsChecked == true && string.IsNullOrEmpty(Filename.Text)) {
                MessageBox.Show("Select XLN file to load");
                return;
            }

            if (XLN.IsChecked == true && LocalPC.IsChecked == true && !File.Exists(Filename.Text)) {
                MessageBox.Show("Selected XLN file does not exist");
                return;
            }

            string patch = "";
            if (PatchCheck.IsChecked == true)
                patch = Patch.SelectedItem.ToString().Split(' ', '|', ' ')[0];

            LabStager.Initialize();

            LabStager.CMRelease = Release.SelectedItem.ToString();
            LabStager.IP = Lab.SelectedItem.ToString();
            LabStager.Patching = PatchCheck.IsChecked == true;
            LabStager.Patch = patch;
            LabStager.LoadingXLN = XLN.IsChecked == true;
            LabStager.XLNFile = Filename.Text;
            LabStager.LocalXLNFile = LocalPC.IsChecked == true;
            LabStager.XLNBackup = XLNBackup.IsChecked == true;

            Staging = true;
            Close();
        }
    }
}
