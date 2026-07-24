/*
 * Auditor3 :: ToolsAConnection
 * 
 * This abstract class defines the connection to the ToolsA server.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Auditor3 {
    internal class ToolsAConnection : ConnectionBase {

        // Prefixes for determining if you are on an old or new ToolsA server
        private const string TOOLSA_OLD_SERVER_PREFIX = "st3tds";
        private const string TOOLSA_NEW_SERVER_PREFIX = "pltlavmap";

        // Flag tracking if we are connected to the new or old ToolsA
        internal bool CONNECTED_NEW;

        // Commands that need to be flexible
        internal static string GETASG = "";
        internal static string CAT = "";

        // Pairs of commands that are different on old versus new ToolsA servers
        internal const string GETASG_OLD = "/homea/bsh/bin/getasg";
        internal const string GETASG_NEW = "/app/inadsb/prog/getasg";

        internal const string CAT_OLD = "/usr/bin/cat";
        internal const string CAT_NEW = "/bin/cat";

        internal const string UPDATE_FILE_TOOLSA_OLD = "/home1/harrisb/auditor/updates/update_info.xml";
        internal const string UPDATE_FILE_TOOLSA_NEW = "/home/harrisb/auditor/updates/update_info.xml";

        internal const string CM_LABS_FILE_OLD = "/home1/harrisb/auditor/labs/cmlabs.xml";
        internal const string CM_LABS_FILE_NEW = "/home/harrisb/auditor/labs/cmlabs.xml";

        internal const string MG_LABS_FILE_OLD = "/home1/harrisb/auditor/labs/mglabs.xml";
        internal const string MG_LABS_FILE_NEW = "/home/harrisb/auditor/labs/mglabs.xml";

        internal const string CM_PATCHES_FILE_OLD = "/home1/harrisb/auditor/labs/cmpatches.xml";
        internal const string CM_PATCHES_FILE_NEW = "/home/harrisb/auditor/labs/cmpatches.xml";

        internal const string CRASH_FOLDER_TOOLSA_OLD = "/home1/harrisb/auditor/crash_reports/";
        internal const string CRASH_FOLDER_TOOLSA_NEW = "/home/harrisb/auditor/crash_reports/";

        // Constructor for creating the connection
        internal ToolsAConnection() { _name = "TOOLSA"; }        

        // Method for connecting to ToolsA
        internal override bool Connect() {
            return Connect(Globals.USER_DATA.ToolsAServer);
        }
        
        // Method for connecting to ToolsA
        internal bool Connect(string server) {
            if (Connected()) return true;

            if (string.IsNullOrEmpty(Globals.USER_DATA.ToolsAUsername) ||
                string.IsNullOrEmpty(Globals.USER_DATA.ToolsAPassword)) {
                Globals.GUI.Error("** YOU MUST PROVIDE TOOLSA USERNAME AND PASSWORD **");
                return false;
            }

            Globals.GUI.AddStatus($"Connecting to ToolsA: {server}");

            try {
                _ssh = new SshClient(server, Globals.USER_DATA.ToolsAUsername, Globals.USER_DATA.ToolsAPassword);
                _ssh.ErrorOccurred += ClientError;
                _ssh.Connect();

                _sftp = new SftpClient(server, Globals.USER_DATA.ToolsAUsername, 
                    Globals.USER_DATA.ToolsAPassword) { OperationTimeout = Globals.SFTP_TIMEOUT };
                _sftp.ErrorOccurred += ClientError;
                _sftp.Connect();

                SetToolsAPaths();

                return true;
            } catch (SshAuthenticationException) {
                Globals.GUI.Error("Authentication error on ToolsA");
                return false;
            } catch (SocketException) {
                Globals.GUI.Error($"Could not connect to ToolsA server {Globals.USER_DATA.ToolsAServer}");
                return false;
            } catch (Exception error) {
                Globals.GUI.Error("An exception occurred while connecting to ToolsA", error);
                return false;
            }
        }


        // Method for setting the ToolsA paths based on if connected to a new (Linux) or old
        // (UNIX) ToolsA server
        private void SetToolsAPaths() {
            if (Globals.USER_DATA.ToolsAServer.Contains(TOOLSA_NEW_SERVER_PREFIX)) {
                GETASG = GETASG_NEW;
                CAT = CAT_NEW;
                Globals.UPDATE_FILE_TOOLSA = UPDATE_FILE_TOOLSA_NEW;
                Globals.CM_LABS_FILE = CM_LABS_FILE_NEW;
                Globals.MG_LABS_FILE = MG_LABS_FILE_NEW;
                Globals.CM_PATCHES_FILE = CM_PATCHES_FILE_NEW;
                Globals.CRASH_FOLDER_TOOLSA = CRASH_FOLDER_TOOLSA_NEW;
                CONNECTED_NEW = true;
            } else {
                GETASG = GETASG_OLD;
                CAT = CAT_OLD;
                Globals.UPDATE_FILE_TOOLSA = UPDATE_FILE_TOOLSA_OLD;
                Globals.CM_LABS_FILE = CM_LABS_FILE_OLD;
                Globals.MG_LABS_FILE = MG_LABS_FILE_OLD;
                Globals.CM_PATCHES_FILE = CM_PATCHES_FILE_OLD;
                Globals.CRASH_FOLDER_TOOLSA = CRASH_FOLDER_TOOLSA_OLD;
                CONNECTED_NEW = false;
            }
        }

        // Method for getting an ASG reponse
        //internal string GetASG(string details) {
        //    return GetASG(details, "init");
        //}

        // Method for getting an ASG reponse
        //internal string GetASG(string details, string username) {
        //    if (!Connected())
        //        if (!Connect()) return "";
        //
        //    var challenge = Regex.Match(details, @"(Challenge: )([-0-9]+)").Groups[2].Value;
        //    var product = Regex.Match(details, @"(Product ID: )([0-9a-zA-Z]+)").Groups[2].Value;
        //    var cmd = _ssh.RunCommand($"{GETASG} {product} {username} {challenge}");
        //    return Regex.Match(cmd.Result, "(response = )([-0-9a-zA-Z]+)").Groups[2].Value;
        //}

        // Method for reading a file
        internal string Cat(string filename) {
            if (!Connected())
                if (!Connect()) return "";

            var cmd = _ssh.RunCommand($"{CAT} {filename}");
            return cmd.Result;
        }

        // Method for checking for updates
        internal void CheckUpdates() {
            if (!Connected())
                if (!Connect()) return;

            Globals.GUI.AddStatus("Checking for updates");

            UpdateInfo info = null;

            if (File.Exists(Globals.UPDATE_FILE)) File.Delete(Globals.UPDATE_FILE);
            if (File.Exists(Globals.UPDATE_PACKAGE)) File.Delete(Globals.UPDATE_PACKAGE);

            try {
                RecieveFile(Globals.UPDATE_FILE_TOOLSA, Globals.UPDATE_FILE);

                if (File.Exists(Globals.UPDATE_FILE)) {
                    var file = new StreamReader(Globals.UPDATE_FILE);
                    var xml = file.ReadToEnd();
                    file.Close();
                    info = Globals.DESERIALIZE<UpdateInfo>(xml);
                }

                if (info == null) return;
                if (info.BUILD_NUMBER <= Globals.BUILD_NUMBER) return;

                Globals.GUI.AddStatus($"Updating Corruption Auditor to {info.VERSION_MAJOR}.{info.VERSION_MINOR}");
                Globals.GUI.AddStatus("Downloading update package");

                using (var file = new FileStream(Globals.UPDATE_PACKAGE, FileMode.Create)) {
                    _sftp.BufferSize = 4 * 1024;
                    _sftp.DownloadFile(info.FILE_NAME, file);
                }

                Disconnect();

                Globals.GUI.AddStatus("Launching updater");

                System.Diagnostics.Process.Start(Globals.UPDATER);
                Environment.Exit(0);
            } catch (Exception error) {
                Globals.GUI.Error("An exception occured retrieving update file from ToolsA", error);
            }
        }

        // Method for retrieving the lab info
        internal void RetrieveLabInfo() {
            if (!Connected())
                if (!Connect()) return;

            Globals.GUI.AddStatus("Retrieving lab info");

            var cmlabs = Cat(Globals.CM_LABS_FILE);
            var mglabs = Cat(Globals.MG_LABS_FILE);
            var cmpatches = Cat(Globals.CM_PATCHES_FILE);

            if (!string.IsNullOrEmpty(cmlabs))
                Globals.CM_LABS = Globals.DESERIALIZE<List<LabInfo>>(cmlabs);
            if (!string.IsNullOrEmpty(mglabs))
                Globals.MG_LABS = Globals.DESERIALIZE<List<LabInfo>>(mglabs);
            if (!string.IsNullOrEmpty(cmpatches))
                Globals.CM_PATCHES = Globals.DESERIALIZE<List<PatchInfo>>(cmpatches);

            Globals.LABS_LOADED = true;
        }
    }
}
