/*
 * Auditor3 :: PR_UDATA
 * 
 * This class defines a PR_UDATA record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_UDATA {

        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string GID;            // Ths GID of the record
        internal bool IsAuditable;      // Flag for if this record is auditable

        // Constructor for creating the record
        internal PR_UDATA(string[] prec) {
            // Store the PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');
            var line2 = prec[1].Split(' ');

            // Store the data
            UID = line1[1];
            GID = UID.Substring(0, 4);

            // Declare a variable to check if the name in this record is 'TTI USER'
            var isTTI = line1[4].Substring(0, 4) == "5454" && line2[1] == "4f502049" &&
                line2[2].Substring(4, 4) == "5452";

            // Set the auditable flag
            IsAuditable = !isTTI && GID == "0000";
        }

        // Check for if this record has a PR_STN with the same UID
        private bool _hasSTN;
        private bool _checkedSTN;
        internal bool HasSTN() {
            if (!_checkedSTN) {
                var check = Database.PR_STNs.Find(a => a.UID == UID);
                _hasSTN = check != null;
                _checkedSTN = true;
            }
            return _hasSTN;
        }

        // Check for if this record has a PR_INT_ANNC
        private bool _hasINTANNC;
        private bool _checkedINTANNC;
        internal bool HasINTANNC() {
            if (!_checkedINTANNC) {
                var pr_int_annc = Database.PR_INT_ANNCs.Find(a => a.UID == UID);
                _hasINTANNC = pr_int_annc != null;
                _checkedINTANNC = true;
            }
            return _hasINTANNC;
        }
    }
}
