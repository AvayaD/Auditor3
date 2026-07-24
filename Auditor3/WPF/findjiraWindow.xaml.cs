/*
 * Auditor3 :: findjiraWindow
 * 
 * This class / XAML defines the window that is used to search for code diffs
 * 
 * 
 * This utilizes David McNutt's 'findjira' DRCCD script.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System.Windows;

namespace Auditor3 {
    public partial class findjiraWindow {

        internal bool IsSearching;
        internal string CodeContext;
        internal string JIRA;

        // Constructor for creating the window
        public findjiraWindow() {
            InitializeComponent();
        }

        // Click handler for the cancel button
        private void Click_Cancel(object sender, RoutedEventArgs args) {
            IsSearching = false;
            Close();
        }

        // Click handler for the search button
        private void Click_Search(object sender, RoutedEventArgs args) {
            if (string.IsNullOrEmpty(Jira.Text)) {
                MessageBox.Show("You must include the JIRA");
                return;
            }

            if (string.IsNullOrEmpty(Context.Text)) {
                MessageBox.Show("You must include the DRCCD code context");
                return;
            }

            IsSearching = true;
            JIRA = Jira.Text;
            CodeContext = Context.Text;
            Close();
        }
    }
}
