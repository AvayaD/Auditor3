/*
 * Auditor3 :: PR_XMAP
 * 
 * This class defines a PR_XMAP record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_XMAP {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record

        // Constructor for creating the records
        internal PR_XMAP(string[] prec) {
            // Store the PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');
            UID = line1[1];
        }

        // Check for if this is an XMOBILE record
        private bool _isXMOBILE;
        private bool _checkedXMOBILE;
        internal bool IsXMOBILE() {
            if (!_checkedXMOBILE) {
                var pr_stn = Database.PR_STNs.Find(a => a.UID == UID);
                _isXMOBILE = pr_stn != null && pr_stn.Type == StationType.Xmobile;
                _checkedXMOBILE = true;
            }
            return _isXMOBILE;
        }

        // Check for if this record has a PR_OPT_STN record
        private bool _hasOPTSTN;
        private bool _checkedOPTSTN;
        internal bool HasOPTSTN() {
            if (!_checkedOPTSTN) {
                var pr_opt_stn = Database.PR_OPT_STNs.Find(a => a.UID == UID);
                _hasOPTSTN = pr_opt_stn != null;
                _checkedOPTSTN = true;
            }
            return _hasOPTSTN;
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
