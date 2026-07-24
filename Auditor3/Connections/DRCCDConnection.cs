/*
 * Auditor3 :: DRCCDConnection
 * 
 * This abstract class defines the connection to the DRCCD server.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Net.Sockets;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Auditor3 {
    internal class DRCCDConnection : ConnectionBase {

        // Constructor for creating the connection
        internal DRCCDConnection() { _name = "DRCCD"; }

        // Method for connecting to ToolsA
        internal override bool Connect() {
            if (Connected()) return true;

            if (string.IsNullOrEmpty(Globals.USER_DATA.DRCCDUsername) ||
                string.IsNullOrEmpty(Globals.USER_DATA.DRCCDPassword)) {
                Globals.GUI.Error("** YOU MUST PROVIDE DRCCD USERNAME AND PASSWORD **");
                return false;
            }

            Globals.GUI.AddStatus($"Connecting to DRCCD");

            try {
                _ssh = new SshClient("drccd.dr.avaya.com", Globals.USER_DATA.DRCCDUsername, Globals.USER_DATA.DRCCDPassword);
                _ssh.ConnectionInfo.Timeout = TimeSpan.FromSeconds(10);
                _ssh.ErrorOccurred += ClientError;
                _ssh.Connect();

                _sftp = new SftpClient("drccd.dr.avaya.com", Globals.USER_DATA.DRCCDUsername,
                    Globals.USER_DATA.DRCCDPassword) { OperationTimeout = Globals.SFTP_TIMEOUT };
                _sftp.ErrorOccurred += ClientError;
                _sftp.Connect();

                return true;
            } catch (SshAuthenticationException) {
                Globals.GUI.Error("Authentication error on DRCCD");
                return false;
            } catch (SocketException) {
                Globals.GUI.Error($"Could not connect to DRCCD");
                return false;
            } catch (Exception error) {
                Globals.GUI.Error("An exception occurred while connecting to DRCCD", error);
                return false;
            }
        }

        // Method for running David McNutt's jirasearchd script
        internal string JiraSearchd(string search, bool stringsearch) {
            if (!Connected()) Connect();
            if (!Connected()) return "NOT CONNECTED";
            var cmd = "/home/mcnuttd/jirasearchd ";
            if (stringsearch) cmd += "-ss ";
            cmd += search;
            return _ssh.RunCommand(cmd).Result;
        }

        // Method for running David McNutt's findjira script
        internal string FindJira(string jira, string context) {
            if (!Connected()) Connect();
            if (!Connected()) return "NOT CONNECTED";
            var shell = Shell();
            shell.WriteLine($". /usr/add-on/definity/bin/pjenvir {context}");
            shell.WriteLine($"/home/mcnuttd/findjira {jira}");
            var output = "";
            var line = "start";
            while (line != null) {
                line = shell.ReadLine(new TimeSpan(0, 0, 5));
                output += line + Environment.NewLine;
            }
            return output;
        }
    }
}
