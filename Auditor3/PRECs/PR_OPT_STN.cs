/*
 * Auditor3 :: PR_OPT_STN
 * 
 * This class defines a PR_OPT_STN record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_OPT_STN {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record

        // Constructor for creating the records
        internal PR_OPT_STN(string[] prec) {
            // Store the PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');
            UID = line1[1];
        }

        // Check for if this record has a PR_XMAP
        private bool _hasXMAP;
        private bool _checkedXMAP;
        internal bool HasXMAP() {
            if (!_checkedXMAP) {
                var xmap = Database.PR_XMAPs.Find(a => a.UID == UID);
                _hasXMAP = xmap != null;
                _checkedXMAP = true;
            }
            return _hasXMAP;
        }

        // Check for if this record has a PR_UDATA with the same UID
        private bool _hasUDATA;
        private bool _checkedUDATA;
        internal bool HasUDATA() {
            if (!_checkedUDATA) {
                var check = Database.PR_UDATAs.Find(a => a.UID == UID);
                _hasUDATA = check != null;
                _checkedUDATA = true;
            }
            return _hasUDATA;
        }
    }
}
