/*
 * Auditor3 :: PR_ACD_TRUNK
 * 
 * This class defines a PR_ACD_TRUNK record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_ACD_TRUNK {
        internal string[] PREC;         // The raw PREC data
        internal string TrunkGroupUID;  // The UID for the trunk group
        internal string TrunkMemberUID; // The UID for the trunk member
        internal string TrunkType;      // The type of trunk
        internal string Split;          // The UID of the split
        internal string MeasuredBy;     // The measured by field **(actually both meas_by and h323_pn)**
        internal bool Flagged;          // This is used to flag the UID pair as reported duplicate

        // Constructor for creating the record
        internal PR_ACD_TRUNK(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');
            var line2 = prec[1].Split(' ');

            // Parse out the basic values
            TrunkGroupUID = line1[1];
            TrunkMemberUID = line1[2];
            TrunkType = line1[3];
            Split = line1[4];
            MeasuredBy = line2[2];
        }

        // Check for if there are duplicate records
        private bool _hasDuplicates;
        private bool _checkedDuplicates;
        internal bool HasDuplicates() {
            if (!_checkedDuplicates) {
                var pr_acd_trunks = Database.PR_ACD_TRUNKs.FindAll(a => a.TrunkGroupUID == TrunkGroupUID &&
                    a.TrunkMemberUID == TrunkMemberUID);
                _hasDuplicates = pr_acd_trunks.Count > 1;
                _checkedDuplicates = true;
            }
            return _hasDuplicates;
        }
    }
}
