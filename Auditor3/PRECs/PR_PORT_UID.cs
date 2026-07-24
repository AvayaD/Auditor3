/*
 * Auditor3 :: PR_PORT_UID
 * 
 * This class defines a PR_PORT_UID record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_PORT_UID {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string GID;            // The GID for the record
        internal string Port;           // The port for the record
        internal bool IsAuditable;      // Flag for if audits run against this record
        internal bool IsTrunk;          // Flag for if this is a trunk record

        // Constructor for creating the record
        internal PR_PORT_UID(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the basic values
            UID = line1[2];
            GID = UID.Substring(0, 4);
            Port = line1[1];

            // Set the auditable flag for GIDs for STA_USER, ATT_USER, and
            // SSTA_USER groups
            IsAuditable = GID == "0000" || GID == "0002" || GID == "0034" || GID == "0020" || GID == "0340";
            IsTrunk = GID == "0005" || GID == "0083" || GID == "0050" || GID == "0830";
        }

        // Check for a valid owner of this record
        private string _validOwner;
        private bool _checkedValidOwner;
        internal string ValidOwner() {
            if (!_checkedValidOwner) {
                var pr_st_cps = Database.PR_ST_CPSs.Find(a => a.Port == Port);
                _validOwner = pr_st_cps != null ? pr_st_cps.UID : null;
                _checkedValidOwner = true;
            }
            return _validOwner;
        }

        // Check for if the UID owns a different port
        private bool _uidOwnsAnotherPort;
        private bool _checkUIDOwnsAnotherPort;
        internal bool UIDOwnsAnotherPort() {
            if (!_checkUIDOwnsAnotherPort) {
                var check1 = Database.PR_PORT_UIDs.FindAll(a => a.UID == UID);
                if (check1.Count != 1) {
                    var check2 = Database.PR_ST_CPSs.Find(a => a.UID == UID);
                    if (check2 != null) {
                        foreach (var port in check1) {
                            if (port.Port == Port) continue;
                            if (port.Port == check2.Port) _uidOwnsAnotherPort = true;
                        }
                    }
                }
                _checkUIDOwnsAnotherPort = true;
            }
            return _uidOwnsAnotherPort;
        }

        // Check for if there is a duplicate port on the UID
        private bool _uidHasDuplicatePort;
        private bool _checkUIDDuplicatePort;
        internal bool UIDHasDuplicatePort() {
            if (!_checkUIDDuplicatePort) {
                var check1 = Database.PR_ST_CPSs.Find(a => a.UID == UID);
                if (check1 != null) {
                    var check2 = Database.PR_ST_CPSs.FindAll(a => a.Port == check1.Port);
                    _uidHasDuplicatePort = check2.Count > 1;
                } else {
                    _uidHasDuplicatePort = false;
                }
                _checkUIDDuplicatePort = true;
            }
            return _uidHasDuplicatePort;
        }

        // Check for if there are multiple PR_ST_CPS that are using the same
        // port as this record
        private bool _hasDuplicateSTCPS;
        private bool _checkedDuplicateSTCPS;
        internal bool HasDuplicateSTCPS() {
            if (!_checkedDuplicateSTCPS) {
                var check = Database.PR_ST_CPSs.FindAll(a => a.Port == Port);
                _hasDuplicateSTCPS = check.Count > 1;
                _checkedDuplicateSTCPS = true;
            }
            return _hasDuplicateSTCPS;
        }

        // Check for if this record has a PR_MOPORT with the same port
        private bool _hasMOPORT;
        private bool _checkedMOPORT;
        internal bool HasMOPORT() {
            if (!_checkedMOPORT) {
                var check = Database.PR_MOPORTs.Find(a => a.Port == Port);
                _hasMOPORT = check != null;
                _checkedMOPORT = true;
            }
            return _hasMOPORT;
        }

        // Check for if this record has a PR_ST_CPS with the same port
        private bool _hasSTCPS;
        private bool _checkedSTCPS;
        internal bool HasSTCPS() {
            if (!_checkedSTCPS) {
                var check = Database.PR_ST_CPSs.Find(a => a.UID == UID);
                _hasSTCPS = check != null;
                _checkedSTCPS = true;
            }
            return _hasSTCPS;
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
    }
}
