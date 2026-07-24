/*
 * Auditor3 :: PR_TRUNK
 * 
 * This class defines a PR_TRUNK record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_TRUNK {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string TrunkGroup;     // The trunk group for the record
        internal string Port;           // The port for this record

        // Constructor for creating the record
        internal PR_TRUNK(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the basic values
            UID = line1[1];
            TrunkGroup = line1[2];
            Port = line1[3];
        }

        // Check for if there is a PR_TR_MBR record
        private bool _hasTRMBR;
        private bool _checkedTRMBR;
        internal bool HasTRMBR() {
            if (!_checkedTRMBR) {
                var pr_tr_mbr = Database.PR_TR_MBRs.Find(a => a.UID == UID);
                _hasTRMBR = pr_tr_mbr != null;
                _checkedTRMBR = true;
            }
            return _hasTRMBR;
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

        // Check for if this record has a PR_PORT_UID with the same UID
        private bool _hasPORTUID;
        private bool _checkedPORTUID;
        internal bool HasPORTUID() {
            if (!_checkedPORTUID) {
                var check = Database.PR_PORT_UIDs.Find(a => a.UID == UID);
                _hasPORTUID = check != null;
                _checkedPORTUID = true;
            }
            return _hasPORTUID;
        }
    }
}
