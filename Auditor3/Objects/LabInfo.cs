/*
 * Auditor3 :: LabInfo
 * 
 * This class defines a lab system and it's IP address. The version string is used to
 * define the type of gateway (G430 versus G450, as opposed to it's firmware level)
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    public class LabInfo {
        public string Version { get; set; }
        public string IP { get; set; }
        public bool Active { get; set; }
    }
}
