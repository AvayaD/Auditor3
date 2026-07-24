/*
 * Auditor3 :: PR_TR_GRP
 * 
 * This class defines a PR_TR_GRP record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_TR_GRP {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID of the group
        internal string MeasBy;         // Value for the measured by field
        internal bool Measured;         // Flag for if this is a measured group

        // Constructor for creating the record
        internal PR_TR_GRP(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');
            var line6 = prec[5].Split(' ');

            // Parse out the basic values
            UID = line1[1];
                if (Globals.CM_RELEASE <= CMRelease.CM7_0) {
                    MeasBy = line6[4].Substring(2, 2);
                } else {
                    MeasBy = line6[4].Substring(6, 2);
                }

            Measured = MeasBy != "00";
        }

        // Method for getting the measured by value for SAT
        internal string MeasuredBy() {
            switch (MeasBy) {
                case "01":
                    return "internal";

                case "02":
                    return "external";

                case "03":
                    return "both";
                
                default:
                    return "none";
            }
        }

        // Check for if there are any PR_ACD_TRUNK records
        private bool _hasACDTRUNK;
        private bool _checkedACDTRUNK;
        internal bool HasACDTRUNK() {
            if (!_checkedACDTRUNK) {
                var pr_acd_trunks = Database.PR_ACD_TRUNKs.FindAll(a => a.TrunkGroupUID == UID);
                _hasACDTRUNK = pr_acd_trunks.Count > 0;
                _checkedACDTRUNK = true;
            }
            return _hasACDTRUNK;
        }
    }
}
