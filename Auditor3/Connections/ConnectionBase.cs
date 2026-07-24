/*
 * Auditor3 :: ConnectionBase
 * 
 * This abstract class defines the underlying processes for creating and managing
 * a SSH/SFTP connection to another machine.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Auditor3 {
    internal abstract class ConnectionBase {
        protected string _name = "NONE";            // The name of the connection
        protected SshClient _ssh;                   // The SSH connection
        protected SftpClient _sftp;                 // The SFTP connection

        internal abstract bool Connect();           // Abstract definition for connecting

        // This method returns the connected state
        internal bool Connected() { return _ssh != null && _ssh.IsConnected; }

        // This method returns the SSH client
        internal SshClient SSH() { return _ssh; }

        // This method returns the SFTP client
        internal SftpClient SFTP() { return _sftp; }

        // This method is for disconnecting the connection
        internal void Disconnect() {
            Globals.GUI.AddStatus($"Disconnecting from {_name}");
            if (_ssh != null) {
                if (_ssh.IsConnected) {
                    _ssh.Disconnect();
                    _ssh.Dispose();
                }
            }

            if (_sftp != null) {
                if (_sftp.IsConnected) {
                    _sftp.Disconnect();
                    _sftp.Dispose();
                }
            }

            _ssh = null;
            _sftp = null;
        }

        // This method is used to get a shell for the connection
        internal ShellStream Shell() {
            Globals.GUI.AddStatus($"Opening shell to {_name}");

            if (!Connected()) return null;

            var shell = _ssh.CreateShellStream("Tail", 80, 24, 800, 600, 1024);
            if (shell == null) {
                Globals.GUI.Error($"Failed to obtain shell to {_name}");
                return null;
            }

            // Flush two lines
            shell.WriteLine("");
            shell.WriteLine("");

            // Clear out the initial lines in the shell
            var line = "start";
            while (line != null) line = shell.ReadLine(new TimeSpan(0, 0, 1));

            return shell;
        }
        
        // Method for uploading a file
        internal bool SendFile(string filename, string target) {

            if (!File.Exists(filename)) { return false; }

            try {
                using (var file = new FileStream(filename, FileMode.Open)) {
                    _sftp.BufferSize = 4 * 1024;
                    _sftp.UploadFile(file, target, true, null);
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

        // Method for downloading a file
        internal bool RecieveFile(string filename, string target) {
            try {
                using (var file = new FileStream(target, FileMode.Create)) {
                    _sftp.BufferSize = 4 * 1024;
                    _sftp.DownloadFile(filename, file, null);
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }        

        // Method for handling client errors
        protected void ClientError(object sender, ExceptionEventArgs args) {
            Globals.GUI.Error($"Exception occured on connection {_name}", args.Exception);
        }
    }
}
