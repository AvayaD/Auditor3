/*
 * Auditor3 :: CMConnection
 * 
 * This class defines the connection to the CM.
 * 
 * Auditor3 python version was developed by David McNutt - mcnuttd@avaya.com
 * 
 */

using System;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Auditor3 {
    internal class CMConnection : ConnectionBase {
        private bool _labPassword;                          // Flag for if we are using a lab password or not
        internal const string Username = "init";            // The username to use to login
        internal const string LabPassword = "itsgrand3";    // The password if we are using a lab password
        private bool _inOSSI;                               // Flag tracking if we are in OSSI
        private bool _inTCM;                                // Flag tracking if we are in TCM

        // Constructor for creating the connection
        internal CMConnection() { _name = "CM"; }

        // Method for connecting to CM
        internal override bool Connect() {
            if (Connected()) return true;

            switch (Globals.MODE) {
                case Mode.LIVE:
                    return ConnectLive();

                case Mode.LAB:
                    return ConnectLab();

                case Mode.OFFLINE:
                default:
                    return false;
            }
        }

        // Method for connecting to a lab system
        internal bool ConnectLab() {
            if (!Globals.CHECK_IP(Globals.CONNECT_IP)) {
                Globals.GUI.Error("** INVALID CONNECTION IP **");
                return false;
            }
            return HandleConnect(Globals.CONNECT_IP, 22);
        }

        // Method for connecting to a live system
        internal bool ConnectLive() {
            if (!Globals.CHECK_PORT(Globals.CONNECT_PORT)) {
                Globals.GUI.Error("** INVALID CONNECTION PORT **");
                return false;
            }

            return HandleConnect("127.0.0.1", Convert.ToUInt16(Globals.CONNECT_PORT));
        }

        // This method is for opening an OSSI terminal on a shell
        internal bool OpenOSSI(ShellStream shell) {
            if (_inOSSI) return true;
            if (shell == null) return false;
            Globals.GUI.AddStatus("Opening OSSI");

            // Run the dsat command
            shell.WriteLine("dsat");

            if (_labPassword) {
                // If we are using a lab password, we can expect a password prompt
                var first = shell.Expect("Password:", Globals.CLI_CMD_TIMEOUT);
                if (string.IsNullOrEmpty(first)) {
                    Globals.GUI.Error("** SAT LOGIN FAILED - ARE YOU CONNECTED TO THE STANDBY **");
                    return false;
                }
                shell.WriteLine(LabPassword);
            } else {
                // If we are not using a lab password, we can expect an ASG challenge
                var first = shell.Expect("Response:", Globals.CLI_CMD_TIMEOUT);
                if (string.IsNullOrEmpty(first)) {
                    Globals.GUI.Error("** SAT LOGIN FAILED - ARE YOU CONNECTED TO THE STANDBY **");
                    return false;
                }
                shell.WriteLine(Connections.WebASGConnection.GetResponse(first));
                //shell.WriteLine(Globals.TOOLSA.GetASG(first));
            }

            // Make sure we get the terminal type prompt next
            var second = shell.Expect("SUNT)", Globals.CLI_CMD_TIMEOUT);
            if (string.IsNullOrEmpty(second)) {
                Globals.GUI.Error("** SAT LOGIN FAILED - DID NOT GET TERMINAL CHOICE RESPONSE **");
                return false;
            }

            // Send ossi as our terminal type
            shell.WriteLine("ossi");

            // Make sure we get the OSSI prompt back
            var third = shell.Expect("t", Globals.CLI_CMD_TIMEOUT);
            if (string.IsNullOrEmpty(third)) {
                Globals.GUI.Error("** SAT LOGIN FAILED - DID NOT GET OSSI PROMPT **");
                return false;
            }

            _inOSSI = true;
            return true;
        }

        // This method is for closing an OSSI connection
        internal bool CloseOSSI(ShellStream shell) {
            if (_inTCM) {
                Globals.GUI.Error("ERROR: Cannot exit OSSI from TCM");
                return false;
            }
            if (!_inOSSI) return true;
            _inOSSI = false;
            Globals.GUI.AddStatus("Closing OSSI");

            shell.WriteLine("clogoff");
            shell.WriteLine("t");
            shell.WriteLine("y");
            return true;
        }

        // This method is for opening a TCM connection on a shell that is already
        // connected to OSSI
        internal bool OpenTCM(ShellStream shell) {
            if (_inTCM) return true;
            if (!_inOSSI) {
                Globals.GUI.Error("Cannot open TCM without opening OSSI first");
                return false;
            }
            if (shell == null) return false;
            Globals.GUI.AddStatus("Opening TCM");

            shell.WriteLine("cgo tcm");
            shell.WriteLine("t");

            var tcm = shell.Expect(new Regex("tcm[0-9]+>|Debugger currently being used, please try later"), Globals.SAT_CMD_TIMEOUT);
            if (string.IsNullOrEmpty(tcm)) {
                Globals.GUI.Error("** TCM LOGIN FAILED **");
                return false;
            } else if (tcm.Contains("Debugger currently being used, please try later")) {
                Globals.GUI.Error("** TCM ALREADY IN USE **");
                return false;
            }

            tcm = RunTCMCommand(shell, "set prec long");
            if (tcm == null) return false;

            tcm = RunTCMCommand(shell, $"set op {Globals.TCM_PAGINATION}");
            if (tcm == null) return false;

            _inTCM = true;
            return true;
        }

        // Method for closing TCM
        public bool CloseTCM(ShellStream shell) {
            if (!_inTCM) return true;
            Globals.GUI.AddStatus("Closing TCM");

            shell.WriteLine("quit");

            var exittcm = shell.Expect(new Regex("t", RegexOptions.Multiline), Globals.TCM_CMD_TIMEOUT);
            if (string.IsNullOrEmpty(exittcm)) {
                Globals.GUI.Error("TCM quit failed", null);
                return false;
            }

            _inTCM = false;
            return true;
        }

        // This method is for running a TCM command
        internal string RunTCMCommand(ShellStream shell, string command) {
            shell.WriteLine(command);
            var tcm = shell.Expect(new Regex("tcm[0-9]+>|<CR> to continue, q<CR> to quit"), Globals.TCM_CMD_TIMEOUT);
            if (string.IsNullOrEmpty(tcm)) {
                Globals.GUI.Error("TCM COMMAND FAILED");
                return null;
            }

            // Flush the buffer when this is the Collector running
            if (Globals.PROCESS == Process.COLLECT) {
                var flush = "start";

                while (!string.IsNullOrEmpty(flush)) {
                    flush = shell.Expect(new Regex("tcm[0-9]+>"), new TimeSpan(0, 0, 1));
                }
            }

            if (tcm.Contains("<CR> to continue, q<CR> to quit")) return tcm + RunTCMCommand(shell, "");
            else return tcm;
        }

        // Method for running a SAT command on a shell
        internal string RunSATCommand(ShellStream shell, string command) {
            shell.WriteLine("c" + command);
            shell.WriteLine("t");

            var sat = shell.Expect(new Regex("t$"), Globals.SAT_CMD_TIMEOUT);
            if (string.IsNullOrEmpty(sat)) {
                Globals.GUI.Error("SAT command failed", null);
                return null;
            }

            return sat;
        }

        // Method for copying a file
        internal void CopyFile(string source, string target) {
            if (!Connected()) Connect();
            if (!Connected()) return;
            _ssh.RunCommand($"/bin/cp {source} {target}");
        }

        // Method for gzipping a file
        internal void GZipFile(string filename) {
            if (!Connected()) Connect();
            if (!Connected()) return;
            _ssh.RunCommand($"/bin/gzip {filename}");
        }
        
        // Method for gunzipping a file
        internal void GUnZipFile(string filename) {
            if (!Connected()) Connect();
            if (!Connected()) return;
            _ssh.RunCommand($"/bin/gunzip -f {filename}");
        }

        // Method for extracting a compressed tar file
        internal void ExtractCompressedTAR(string filename, string destination) {
            if (!Connected()) Connect();
            if (!Connected()) return;
            _ssh.RunCommand($"/bin/tar xvfz {filename} -C {destination}");
        }

        // Method for creating a directory
        internal void CreateDirectory(string directory) {
            if (!Connected()) Connect();
            if (!Connected()) return;
            _ssh.RunCommand($"/bin/mkdir {directory}");
        }

        // Method for getting the swversion
        internal string Swversion() {
            if (!Connected()) Connect();
            if (!Connected()) return "";
            return _ssh.RunCommand($"/opt/ecs/bin/swversion").Result;
        }

        // Method for using wget to retrieve a file
        internal void Wget(string url, string target) {
            if (!Connected()) Connect();
            if (!Connected()) return;
            url = url.Replace("info.dr.avaya.com", "135.9.1.4");
            _ssh.RunCommand($"/usr/bin/wget {url} {target}");
        }

        // Method for getting the statapp
        internal string Statapp() {
            if (!Connected()) Connect();
            if (!Connected()) return "";
            return _ssh.RunCommand("/opt/ecs/bin/statapp").Result;
        }

        // Method for actually connecting to a site
        protected bool HandleConnect(string ip, ushort port) {
            Globals.GUI.AddStatus($"Connecting to {_name} {ip}:{port}");
            try {
                var connect = new KeyboardInteractiveConnectionInfo(ip, port, Username);
                connect.AuthenticationPrompt += HandleAuthenticate;

                _ssh = new SshClient(connect);
                _ssh.ErrorOccurred += ClientError;

                _ssh.Connect();

                _sftp = new SftpClient(connect) { OperationTimeout = Globals.SFTP_TIMEOUT };
                _sftp.ErrorOccurred += ClientError;
                
                _sftp.Connect();

                return true;
            } catch (SocketException) {
                Globals.GUI.Error("** COULD NOT CONNECT TO CM - IS RAUI CONNECTION OPEN **");
                return false;
            } catch (Exception error) {
                Globals.GUI.Error("An exception occured while connecting to CM", error);
                return false;
            }
        }

        // Method for handling CM authentication
        private void HandleAuthenticate(object sender, AuthenticationPromptEventArgs args) {
            var asg = "";
            foreach (var prompt in args.Prompts) {
                if (prompt.Request.Contains("Challenge:")) {
                    asg = Connections.WebASGConnection.GetResponse(prompt.Request);
                    //asg = Globals.TOOLSA.GetASG(prompt.Request);
                    _labPassword = false;
                } 
                else if (prompt.Request.Contains("Password:")) {
                    asg = LabPassword;
                    _labPassword = true;
                }
                prompt.Response = asg;
            }
        }
    }
}
