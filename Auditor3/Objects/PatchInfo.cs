/*
 * Auditor3 :: PatchInfo
 * 
 * This class defines a patch that can be loaded onto a lab system
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    public class PatchInfo {
        public string Version { get; set; }
        public string Patch { get; set; }
        public string Release { get; set; }
        public string URL { get; set; }
    }
}
