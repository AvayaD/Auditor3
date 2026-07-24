/*
 * Auditor3 :: LabStagerConnection
 * 
 * This class defines the connection to the lab CM to be staged.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class LabStagerConnection : CMConnection {

        // Constructor for creating the connection
        internal LabStagerConnection() { _name = "LAB"; }

        // Method for connecting to CM
        internal bool Connect(string ip) {
            if (Connected()) return true;
            return HandleConnect(ip, 22);
        }
    }
}
