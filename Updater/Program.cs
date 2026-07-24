/*
 * Updater
 * 
 * This console application is used to extract an update package.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace Updater {
    public static class Program {
        public static void Main(string[] args) {
            try {
                // Clear the console and display the header
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Corruption Auditor Updater");
                Console.WriteLine();

                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "update.zip")) {
                    Console.WriteLine("Update package does not exist");
                    Console.WriteLine();
                    Console.WriteLine("** PRESS ANY KEY TO EXIT **");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                Console.Write("Waiting for auditor to close........");

                // Pause for 5 seconds to give the auditor time to close, and kill it
                // if it still hasnt
                Thread.Sleep(5000);
                var process = Process.GetProcesses().Where(a => a.ProcessName == "CorruptionAuditor");
                if (process.Count() > 0) { foreach (var proc in process) { proc.Kill(); } }

                Console.WriteLine("[DONE]");
                Console.Write("Extracting update package...........");

                using (var archive = ZipFile.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "update.zip")) {
                    foreach (var file in archive.Entries) {
                        if (string.IsNullOrEmpty(file.Name) || file.Name == "updater.exe") continue;
                        try {
                            file.ExtractToFile(AppDomain.CurrentDomain.BaseDirectory + file.FullName, true);
                        } catch (Exception) { }
                    }
                }

                Console.WriteLine("[DONE]");

                Console.Write("Starting Corruption Auditor.........");
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "CorruptionAuditor.exe");
                Console.WriteLine("[DONE]");

                Thread.Sleep(5000);
                Environment.Exit(0);
            } catch (Exception error) {
                Console.WriteLine("An exception occured while running the updater");
                Console.WriteLine(ConvertException(error));
                Console.WriteLine();
                Console.WriteLine("** PRESS ANY KEY TO EXIT **");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        // Method for converting an exception to text for output
        private static string ConvertException(Exception error) {
            var text = new StringBuilder();

            text.AppendLine();
            text.AppendLine("Exception Details::");
            text.AppendLine("TYPE: " + error.GetType().Name);
            text.AppendLine("MESSAGE: " + error.Message);
            text.AppendLine("DATA   : " + error.Data);
            text.AppendLine("SOURCE : " + error.Source);
            text.AppendLine("STACK  : " + error.StackTrace);
            text.AppendLine("TARGET : " + error.TargetSite);

            while (error.InnerException != null) {
                error = error.InnerException;
                text.AppendLine("INNER : " + error.Message);
            }

            return text.ToString();
        }
    }
}
