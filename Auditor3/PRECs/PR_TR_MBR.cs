/*
 * Auditor3 :: PR_TR_MBR
 * 
 * This class defines a PR_TR_MBR record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_TR_MBR {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string TrunkGroup;     // The trunk group for the record
        internal string Port;           // The port for the record
        internal bool Flagged;          // Flag for if we need to skip AUDIT-T06 for same trunk group

        // Constructor for creating the record
        internal PR_TR_MBR(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the basic values
            UID = line1[2];
            TrunkGroup = line1[1];
            Port = line1[4];
        }

        // Check for if this record has a PR_TRUNK
        private bool _hasTRUNK;
        private bool _checkedTRUNK;
        internal bool HasTRUNK() {
            if (!_checkedTRUNK) {
                var pr_trunk = Database.PR_TRUNKs.Find(a => a.UID == UID);
                _hasTRUNK = pr_trunk != null;
                _checkedTRUNK = true;
            }
            return _hasTRUNK;
        }

        // Check for if this record has a PR_ACD_TRUNK
        private bool _hasACDTRUNK;
        private bool _checkedACDTRUNK;
        internal bool HasACDTRUNK() {
            if (!_checkedACDTRUNK) {
                var pr_acd_trunk = Database.PR_ACD_TRUNKs.Find(a => a.TrunkGroupUID == TrunkGroup &&
                    a.TrunkMemberUID == UID);
                _hasACDTRUNK = pr_acd_trunk != null;
                _checkedACDTRUNK = true;
            }
            return _hasACDTRUNK;
        }
    }
}
