/*
 * Auditor3 :: Location
 * 
 * This class defines the parameters of a 'location', which is actually just an Avaya
 * environment, but do not want to conflict with default Envronment class.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3.Locations {
    internal class Location {
        internal LocationID ID;
        internal string Description;
        internal string[] AvailableToolsAServers;
        internal string[] AvailableDRCCDServers;
        internal string WebASGURL;

        internal bool HasToolsA() { return AvailableToolsAServers?.Length > 0; }
        internal bool HasDRCCD() { return AvailableDRCCDServers?.Length > 0; }

        internal Location(LocationID id, string description, string webasgurl, string[] toolsa = null, 
            string[] drccd = null) {
            ID = id;
            Description = description;
            WebASGURL = webasgurl;
            AvailableToolsAServers = toolsa;
            AvailableDRCCDServers = drccd;
        }
    }
}
