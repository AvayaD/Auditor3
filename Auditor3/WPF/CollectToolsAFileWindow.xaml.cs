/*
 * Auditor3 :: CollectToolsAFileWindow
 * 
 * This class / XAML defines the window that is used to let the user provide a 
 * ToolsA filename and path
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System.Windows;

namespace Auditor3 {
    public partial class CollectToolsAFileWindow {

        internal string File;

        // Constructor for initialzing the window
        public CollectToolsAFileWindow() {
            InitializeComponent();
        }

        // Click handler for the OK button
        private void Click_OK(object sender, RoutedEventArgs args) {
            if (string.IsNullOrEmpty(Filename.Text)) {
                MessageBox.Show("You have not provided a file", "Error");
                return;
            }
            File = Filename.Text;
            Close();
        }

        // Click handler for the cancel button
        private void Click_Cancel(object sender, RoutedEventArgs args) {
            File = null;
            Close();
        }
    }
}
