/*
 * Auditor3 :: MissingStationType
 * 
 * This class defines the variables needed to track station types the tool does not support 
 * so they can be added.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class MissingStationType {
        internal string Type;       // The type bits for the station
        internal string UID;        // The UID of the station
        internal string MO;         // The MO for the station, if found
        internal string Port;       // The port of the station, if found
    }
}
