/*
 * Auditor3 :: jirasearchdWindow
 * 
 * This class / XAML defines the window that is used to search for JIRA's
 * 
 * 
 * This utilizes David McNutt's 'jirasearchd' DRCCD script.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System.Windows;

namespace Auditor3 {
    public partial class jirasearchdWindow {

        internal bool IsSearching;
        internal bool IsStringSearch;
        internal string SearchString;

        // Constructor for creating the window
        public jirasearchdWindow() {
            InitializeComponent();
        }

        // Click handler for the cancel button
        private void Click_Cancel(object sender, RoutedEventArgs args) {
            IsSearching = false;
            Close();
        }

        // Click handler for the search button
        private void Click_Search(object sender, RoutedEventArgs args) {
            if (string.IsNullOrEmpty(Search.Text)) {
                MessageBox.Show("You must include search keywords");
                return;
            }
            IsSearching = true;
            IsStringSearch = StringSearch.IsChecked == true;
            SearchString = Search.Text;
            Close();
        }
    }
}
