/*
 * Auditor3 :: Globals
 * 
 * This class stores global variables, references to non-static objects, and common helper methods.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Auditor3 {
    internal static class Globals {

        internal const int VERSION_MAJOR = 4;           // The major version number
        internal const int VERSION_MINOR = 0;           // The minor version number
        internal const int BUILD_NUMBER = 35;           // The build number (from version 1.0)
        internal const bool VERSION_DEV = true;         // Flag for if this is a development build

        internal static MainWindow GUI;                 // Reference to the GUI
        internal static ToolsAConnection TOOLSA;        // The connection to the ToolsA server
        internal static DRCCDConnection DRCCD;          // The connection to the DRCCD server
        internal static CMConnection CM;                // The connection to the CM

        internal static string CONNECT_IP;              // The lab IP to connect to
        internal static string CONNECT_PORT;            // The live port to connect to

        internal static State STATE;                    // The current state of the application
        internal static Process PROCESS;                // The current process that is running
        internal static Mode MODE;                      // The current mode of the application

        internal static CMRelease CM_RELEASE;           // The CM release being worked on

        internal static DateTime START_TIME;            // The time a process started

        internal static bool PRECS_LOADED;              // Flag tracking if there are PRECs loaded
        internal static bool AUDIT_COMPLETE;            // Flag tracking if an audit has been run
        internal static bool CANCEL;                    // Flag tracking if we are cancelling
        internal static bool STATION_AUDITS;            // Flag tracking if we are doing station audits
        internal static bool TRUNK_AUDITS;              // Flag tracking if we are doing trunk audits
        internal static bool ANNOUNCEMENT_AUDITS;       // Flag tracking if we are doing announcement audits
        internal static bool WYLD_STALLYN;              // Flag tracking if Wyld Stallyn Mode is active

        internal static UserData USER_DATA;             // The user configuration
        internal static bool IS_ADMIN;                  // Flag for if the user has admin features

        internal const int REFRESH_TIMER = 250;         // Time in ms between GUI refreshes
        internal static TimeSpan SFTP_TIMEOUT = new TimeSpan(0, 0, 30);
        internal static TimeSpan CLI_CMD_TIMEOUT = new TimeSpan(0, 0, 30);
        internal static TimeSpan SAT_CMD_TIMEOUT = new TimeSpan(0, 0, 30);
        internal static TimeSpan TCM_CMD_TIMEOUT = new TimeSpan(0, 0, 30);
        internal static int TCM_PAGINATION = 50000;      

        internal const string NULL_PORT = "00000000";   // The value of a null port in CM translations
        internal const string NULL_UID = "00000000";    // The value of a null UID in CM translations

        // Directory definitions
        internal static string BASE_DIR = AppDomain.CurrentDomain.BaseDirectory;
        internal static string REPORT_DIR = BASE_DIR + "reports\\";

        // File definitions
        internal static string USER_DATA_FILE = BASE_DIR + "user.dat";
        internal static string PRECS_FILE = BASE_DIR + "precs.corr";
        internal static string XLN_FILE = BASE_DIR + "auditor_xln1.gz";
        internal static string SCRIPT_FILE = BASE_DIR + "generated_script";

        internal static string[] WORKING_PREC;

        internal static string UPDATE_FILE_TOOLSA = "/home1/harrisb/auditor/updates/update_info.xml";
        internal static string CRASH_FOLDER_TOOLSA = "/home1/harrisb/auditor/crash_reports/";
        internal static string UPDATE_FILE = BASE_DIR + "update_info.xml";
        internal static string UPDATE_PACKAGE = BASE_DIR + "update.zip";
        internal static string UPDATER = BASE_DIR + "updater.exe";

        internal static bool LABS_LOADED;
        internal static string CM_LABS_FILE = "/home1/harrisb/auditor/labs/cmlabs.xml";
        internal static string MG_LABS_FILE = "/home1/harrisb/auditor/labs/mglabs.xml";
        internal static string CM_PATCHES_FILE = "/home1/harrisb/auditor/labs/cmpatches.xml";
        internal static string CM_LABS_LOCAL_FILE = BASE_DIR + "cmlabs.xml";
        internal static string MG_LABS_LOCAL_FILE = BASE_DIR + "mglabs.xml";
        internal static string CM_PATCHES_LOCAL_FILE = BASE_DIR + "cmpatches.xml";

        internal static List<LabInfo> CM_LABS;
        internal static List<LabInfo> MG_LABS;
        internal static List<PatchInfo> CM_PATCHES;

        // This method is used to generate the version string
        internal static string VERSION() {
            var version = $"{VERSION_MAJOR}.{VERSION_MINOR}";
            if (VERSION_DEV) version += "d";
            version += $" ({BUILD_NUMBER})";
            return version;
        }

        // This method is used to generate report filenames
        internal static string REPORT(string type) {
            return $"{REPORT_DIR}{type}_{TIMESLICE()}.log";
        }

        // This method is used to clean the special characters from a string
        internal static string CLEAN(string input) {
            var clean = new StringBuilder();
            foreach (char c in input) {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' ||
                    c == ' ' || c == '\n') {
                    clean.Append(c);
                }
            }
            return clean.ToString();
        }

        // This method generates a timestamp
        internal static string TIMESTAMP() { return DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"); }

        // This method generates a timeslice for adding to filenames
        internal static string TIMESLICE() { return DateTime.Now.ToString("yyyyMMdd_HHmmss"); }

        // This method serialized an object to an XML string
        internal static string SERIALIZE(object input) {
            try {
                var writer = new StringWriter(new StringBuilder());
                var serializer = new XmlSerializer(input.GetType());
                serializer.Serialize(writer, input);
                return writer.ToString();
            } catch (Exception) {
                return null;
            }
        }

        // This method deserializes an XML string to an object
        internal static T DESERIALIZE<T>(string input) {
            try {
                var reader = XmlReader.Create(new StringReader(input));
                var deserializer = new XmlSerializer(typeof(T));
                return (T)deserializer.Deserialize(reader);
            } catch (Exception) {
                return default(T);
            }
        }

        // This method is for validating a string is a valid port number
        internal static bool CHECK_PORT(string input) {
            try {
                var port = Convert.ToUInt16(input);
                return port > 0;
            } catch (Exception) {
                return false;
            }
        }

        // This method is for validating a string is a valid IP address
        internal static bool CHECK_IP(string input) {
            try {
                var ip = IPAddress.Parse(input);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        // Method to grab the last lines of a string (for pulling EECCRs from list audits)
        internal static string LAST_LINES(string text) {
            var match = Regex.Match(text, "^.*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
            var lines = "";

            int count = 0;

            while (match.Success && count < 7) {
                if (string.IsNullOrEmpty(match.Value)) {
                    match = match.NextMatch();
                    continue;
                }

                lines = match.Value + Environment.NewLine + lines;
                match = match.NextMatch();
                count++;
            }

            return lines;
        }

        // Method to reverse a string
        internal static string REVERSE_STRING(string input) {
            var chars = input.ToCharArray();
            for (int i = 0, j = input.Length - 1; i < j; i++, j--) {
                chars[i] = input[j];
                chars[j] = input[i];
            }
            return new string(chars);
        }

        // Method to get an AMW extension
        internal static string AMW_EXT(string input1, string input2) {
            return input2 + input1;
        }

        // Method to unpack an extension
        internal static string UNPACK_EXTENSION(string input) {
            var ext = REVERSE_STRING(input);
            var index = ext.IndexOf('0');
            if (index != -1) ext = ext.Substring(0, index);
            return ext.Replace('a', '0');
        }
    }
}
