/*
 * Auditor3 :: PR_BRIDGE
 * 
 * This class defines a PR_BRIDGE record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_BRIDGE {
        internal string[] PREC;         // The raw PREC data
        internal string PrimaryUID;     // The primary UID of the record
        internal string BridgedUID;     // The bridged UID of the record
        internal string BridgeID;       // The ID of the record

        // Constructor for creating the records
        internal PR_BRIDGE(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the basic values
            PrimaryUID = line1[1];
            BridgedUID = line1[2];
            BridgeID = line1[3].Substring(4, 4);
        }

        // Check for if this record has a valid primary UID
        private bool _hasValidPrimaryUID;
        private bool _checkedValidPrimaryUID;
        internal bool HasValidPrimaryUID() {
            if (!_checkedValidPrimaryUID) {
                var pr_stn = Database.PR_STNs.Find(a => a.UID == PrimaryUID);
                _hasValidPrimaryUID = pr_stn != null;
                _checkedValidPrimaryUID = true;
            }
            return _hasValidPrimaryUID;
        }
    }
}
