/*
 * Auditor3 :: PR_INT_ANNC
 * 
 * This class defines a PR_INT_ANNC record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_INT_ANNC {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string Board;          // The board for the record
        internal string LName;          // The lname for the board
        internal string FileIndex;      // The file index for the board
        internal string AudioGroup;     // The audio group for the record
        internal string IndexLName;     // The combined lname/index fields
        
        // Constructor for creating the record
        internal PR_INT_ANNC(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');
            var line3 = prec[2].Split(' ');

            // Parse out the basic values
            UID = line1[1];
            Board = line1[2];
            LName = line1[3].Substring(4, 4);
            FileIndex = line1[3].Substring(0, 4);
            IndexLName = line1[3];
            AudioGroup = line3[4].Substring(6, 2);
        }


        // Check for if there is a PR_GM_IANC_BD record
        private bool _hasGMIANCBD;
        private bool _checkedGMIANCBD;
        internal bool HasGMIANCBD() {
            if (!_checkedGMIANCBD) {
                var pr_gm_ianc_bd = Database.PR_GM_IANC_BDs.Find(a => a.Board == Board && a.UID == UID);
                _hasGMIANCBD = pr_gm_ianc_bd != null;
                _checkedGMIANCBD = true;
            }
            return _hasGMIANCBD;
        }

        // Check for if there is a PR_IANC_BD record
        private bool _hasIANCBD;
        private bool _checkedIANCBD;
        internal bool HasIANCBD() {
            if (!_checkedIANCBD) {
                var pr_ianc_bd = Database.PR_IANC_BDs.Find(a => a.Board == Board && a.UID == UID);
                _hasIANCBD = pr_ianc_bd != null;
                _checkedIANCBD = true;
            }
            return _hasIANCBD;
        }

        // Check for if there is a PR_EXT record
        private bool _hasEXT;
        private bool _checkedEXT;
        internal bool HasEXT() {
            if (!_checkedEXT) {
                var pr_ext = Database.PR_EXTs.Find(a => a.UID == UID);
                _hasEXT = pr_ext != null;
                _checkedEXT = true;
            }
            return _hasEXT;
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

        // Check for if this record has an AG mismatch
        private bool _hasAGMismatch;
        private bool _checkedAGMismatch;
        internal bool HasAGMismatch() {
            if (!_checkedAGMismatch) {
                var pr_an_grp = Database.PR_AN_GRPs.Find(a => a.UID == UID);
                if (pr_an_grp != null)
                    if (pr_an_grp.AudioGroup != AudioGroup) _hasAGMismatch = true;
                _checkedAGMismatch = true;
            }
            return _hasAGMismatch;
        }

        // Check for if this record has an PR_AN_GRP
        private bool _hasANGRP;
        private bool _checkedANGRP;
        internal bool HasANGRP() {
            if (!_checkedANGRP) {
                var pr_an_grp = Database.PR_AN_GRPs.Find(a => a.UID == UID);
                _hasANGRP = pr_an_grp != null;
                _checkedANGRP = true;
            }
            return _hasANGRP;
        }
    }
}
