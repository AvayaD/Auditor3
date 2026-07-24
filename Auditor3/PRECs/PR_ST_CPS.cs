/*
 * Auditor3 :: PR_ST_CPS
 * 
 * This class defines a PR_ST_CPS record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_ST_CPS {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string Port;           // The port for the record

        // Constructor for creating the record
        internal PR_ST_CPS(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the basic values
            UID = line1[1];
            Port = line1[2];
        }

        // Check for if this has an IP port
        internal bool IsIPPort() {
            return Port.Substring(0, 2) == "7f";
        }

        // Check all PR_ST_CPS records to ensure only one is using this port
        private bool _hasDuplicates;
        private bool _checkedDuplicates;
        internal bool HasDuplicates() {
            if (!_checkedDuplicates) {
                var check = Database.PR_ST_CPSs.FindAll(a => a.Port == Port);
                _hasDuplicates = check.Count > 1;
                _checkedDuplicates = true;
            }
            return _hasDuplicates;
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
                var check = Database.PR_PORT_UIDs.Find(a => a.Port == Port);
                _hasPORTUID = check != null;
                _checkedPORTUID = true;
            }
            return _hasPORTUID;
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

        // Check for PR_MOBD
        private bool _hasMOBD;
        private bool _checkedMOBD;
        internal bool HasMOBD() {
            if (!_checkedMOBD) {
                var board = Port.Substring(0, 6) + "00";
                var check = Database.PR_MOBDs.Find(a => a.Board == board);
                _hasMOBD = check != null;
                _checkedMOBD = true;
            }
            return _hasMOBD;
        }
    }
}
