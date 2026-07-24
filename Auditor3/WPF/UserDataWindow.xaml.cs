/*
 * Auditor3 :: UserDataWindow
 * 
 * This class / XAML defines the window that is used to collect and manage
 * user data.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Windows;
using Auditor3.Locations;

namespace Auditor3 {
    public partial class UserDataWindow {

        internal UserData UserData;
        internal bool Saved;

        // Constructor for creating the window
        public UserDataWindow(UserData data) {
            InitializeComponent();
            UserData = data;
            ToolsAServer.Text = data.ToolsAServer;
            ToolsAUsername.Text = data.ToolsAUsername;
            ToolsAPassword.Password = data.ToolsAPassword;
            DRCCDUsername.Text = data.DRCCDUsername;
            DRCCDPassword.Password = data.DRCCDPassword;
            DefaultLabIP.Text = data.DefaultLabIP;
            DefaultLivePort.Text = data.DefaultLivePort;

            foreach (var location in Enum.GetNames(typeof(LocationID)))
                Location.Items.Add(location);
            Location.SelectedItem = data.Location.ToString();
        }
        
        // Click handler for the cancel button
        private void Click_Cancel(object sender, RoutedEventArgs args) {
            Saved = false;
            Close();
        }

        // Click handler for the save button
        private void Click_Save(object sender, RoutedEventArgs args) {
            Saved = true;
            UserData.ToolsAServer = ToolsAServer.Text;
            UserData.ToolsAUsername = ToolsAUsername.Text;
            UserData.ToolsAPassword = ToolsAPassword.Password;
            UserData.DRCCDUsername = DRCCDUsername.Text;
            UserData.DRCCDPassword = DRCCDPassword.Password;
            UserData.DefaultLabIP = DefaultLabIP.Text;
            UserData.DefaultLivePort = DefaultLivePort.Text;
            UserData.Location = (LocationID)Enum.Parse(typeof(LocationID), Location.SelectedItem.ToString());
            Close();
        }
    }
}
