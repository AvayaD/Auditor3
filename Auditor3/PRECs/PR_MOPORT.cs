/*
 * Auditor3 :: PR_MOPORT
 * 
 * This class defines a PR_MOPORT record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_MOPORT {
        internal string[] PREC;         // The raw PREC data
        internal string Port;           // The port for the record
        internal string MO;             // The MO for the record
        internal bool IsAuditable;      // Flag tracking if this is an auditable record

        // Constructor for creating the record
        internal PR_MOPORT(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the basic values
            Port = line1[3];
            MO = line1[1].Substring(4, 4);

            // Set the auditable flag
            IsAuditable = MO != "0c13";
        }

        // This is used to retrive the UID for this from the PR_ST_CPS record
        private string _uid;
        private bool _checkedUID;
        internal string UID() {
            if (!_checkedUID) {
                var pr_st_cps = Database.PR_ST_CPSs.Find(a => a.Port == Port);
                _uid = pr_st_cps != null ? pr_st_cps.UID : Globals.NULL_UID;
                _checkedUID = true;
            }
            return _uid;
        }

        private string _tguid;
        private bool _checkedtguid;
        internal string TGUID() {
            if (!_checkedtguid) {
                var pr_port_uid = Database.PR_PORT_UIDs.Find(a => a.Port == Port);
                _tguid = pr_port_uid != null ? pr_port_uid.UID : Globals.NULL_UID;
                _checkedtguid = true;
            }
            return _tguid;
        }

        // Check for if this record has a PR_ST_CPS with the same port
        private bool _hasSTCPS;
        private bool _checkedSTCPS;
        internal bool HasSTCPS() {
            if (!_checkedSTCPS) {
                var check = Database.PR_PORT_UIDs.Find(a => a.Port == Port);
                _hasSTCPS = check != null;
                _checkedSTCPS = true;
            }
            return _hasSTCPS;
        }

        // Check for if this record has a PR_PORT_UID with the same UID
        private bool _hasPORTUID;
        private bool _checkedPORTUID;
        internal bool HasPORTUID() {
            if (!_checkedPORTUID) {
                var check = Database.PR_PORT_UIDs.Find(a => a.Port == Port);
                _hasPORTUID = check != null;
                _checkedPORTUID = true;
            }
            return _hasPORTUID;
        }

        // Check for if this record has a PR_TRUNK
        private bool _hasTRUNK;
        private bool _checkedTRUNK;
        internal bool HasTRUNK() {
            if (!_checkedTRUNK) {
                var check = Database.PR_TRUNKs.Find(a => a.Port == Port);
                _hasTRUNK = check != null;
                _checkedTRUNK = true;
            }
            return _hasTRUNK;
        }

        // Method for getting the trunk UID
        internal string TrunkUID() {
            var check = Database.PR_TRUNKs.Find(a => a.Port == Port);
            return check != null ? check.UID : Globals.NULL_UID;
        }
    }
}