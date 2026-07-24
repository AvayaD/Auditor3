/*
 * Auditor3 :: ScriptGenerator
 * 
 * This class / XAML defines the window that is used to generate basic TCM scripts
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Auditor3 {
    public partial class ScriptGeneratorWindow {
        // Constructor for creating the window
        public ScriptGeneratorWindow() {
            InitializeComponent();
        }

        // Handler for when the input type is changed
        private void Click_SetType(object sender, RoutedEventArgs args) {
            BrowseButton.IsEnabled = LocalInput.IsChecked == true;
        }

        // Clck handler for the browse button
        private void Click_Browse(object sender, RoutedEventArgs args) {
            // Open a filename selector
            var selector = new OpenFileDialog();
            selector.ShowDialog();

            // Make sure a file was selected
            if (string.IsNullOrEmpty(selector.FileName)) {
                return;
            }

            Input.Text = selector.FileName;
        }

        // Click handler for the close button
        private void Click_Close(object sender, RoutedEventArgs args) {
            Close();
        }

        // Click handler for the generate button
        private void Click_Generate(object sender, RoutedEventArgs args) {
            if (string.IsNullOrEmpty(Input.Text)) {
                MessageBox.Show("You must select an input file");
                return;
            }

            if (string.IsNullOrEmpty(Script.Text)) {
                MessageBox.Show("You must enter a script line");
                return;
            }

            if (!Script.Text.Contains("$1")) {
                MessageBox.Show("Script line does not contain the replacement variable   $1");
                return;
            }

            var input = "";

            if (LocalInput.IsChecked == true) {
                if (!File.Exists(Input.Text)) {
                    MessageBox.Show("Input file does not exist");
                    return;
                }
                var file = new StreamReader(Input.Text);
                input = file.ReadToEnd();
                file.Close();
            } else {
                input = Globals.TOOLSA.Cat(Input.Text);
            }

            if (string.IsNullOrEmpty(input)) {
                MessageBox.Show("No input lines detected");
                return;
            }

            var inputs = input.Split('\n');
            var output = new List<string>();

            foreach (var line in inputs) {
                if (string.IsNullOrEmpty(line)) continue;
                var set = line.Replace('\n', ' ');
                set = set.Trim();
                var command = Script.Text.Replace("$1", set);
                output.Add(command);
            }

            var script = new StreamWriter(Globals.SCRIPT_FILE);
            foreach (var cmd in output) script.WriteLine(cmd);
            script.Close();

            MessageBox.Show($"Script generated at{Environment.NewLine}{Globals.SCRIPT_FILE}");
        }
    }
}
