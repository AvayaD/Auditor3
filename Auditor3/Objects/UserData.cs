/*
 * Auditor3 :: UserData
 * 
 * This class defines the user configurable options. This class is public to allow for XML
 * serialization.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using Auditor3.Locations;

namespace Auditor3 {
    public class UserData {
        public string ToolsAServer = "st3tds04.us1.avaya.com";
        public string ToolsAUsername = "";
        public string ToolsAPassword = "";
        public string DRCCDUsername = "";
        public string DRCCDPassword = "";
        public string DefaultLabIP = "";
        public string DefaultLivePort = "22";
        public LocationID Location = LocationID.CORPORATE;
    }
}
