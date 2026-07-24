/*
 * Auditor3 :: App
 * 
 * This class defines the startup handling when the application launches.
 * 
 * Auditor3 is developed and maintained by David McNutt 
 * 
 */

using System.Windows;
using System.IO;
using System;
using Microsoft.Win32;
using System.Runtime.Versioning;

namespace Auditor3 {
    [SupportedOSPlatform("windows")]
    public partial class App : Application {
        // Import DLL functionality
        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        // Override method to manually handle the startup
        protected override void OnStartup(StartupEventArgs args) {
            // Handle base startup
            base.OnStartup(args);

            // Declare the application path
            var appPath = AppDomain.CurrentDomain.BaseDirectory + "CorruptionAuditor.exe \"%1\"";

            // Check the registry and ensure we have the keys defined to associate .corr files to the auditor
            if (OperatingSystem.IsWindows()) {
                if (Registry.GetValue("HKEY_CLASSES_ROOT\\CorruptionAuditor", string.Empty, string.Empty) == null) {
                    Registry.SetValue("HKEY_CLASSES_ROOT\\CorruptionAuditor", "", "CorruptionAuditor");
                    Registry.SetValue("HKEY_CLASSES_ROOT\\CorruptionAuditor\\shell\\open\\command", "",
                        appPath);
                    Registry.SetValue("HKEY_CLASSES_ROOT\\.corr", "", "CorruptionAuditor");

                    SHChangeNotify(0x08000000, 0x2000, IntPtr.Zero, IntPtr.Zero);
                } else if (Registry.GetValue("HKEY_CLASSES_ROOT\\CorruptionAuditor\\shell\\open\\command",
                      string.Empty, string.Empty).ToString() != appPath) {
                    Registry.SetValue("HKEY_CLASSES_ROOT\\CorruptionAuditor\\shell\\open\\command", "",
                        appPath);
                }
            }

            // Declare the main window
            MainWindow window;

            // Try block just in case something happens here, we want to make sure
            // the window gets created
            try {
                // Check if we have any startup args, which should be a .corr file
                // to preload during the startup
                if (args.Args.Length > 0) {
                    // Check if the file actually exists
                    if (File.Exists(args.Args[0])) {
                        window = new MainWindow(args.Args[0]);
                    } else { window = new MainWindow(); }
                } else { window = new MainWindow(); }
            } catch (Exception) { window = new MainWindow(); }

            // Show the window and after it's closed shut it down
            window.ShowDialog();
            Shutdown();
        }
    }
}
