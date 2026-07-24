/*
 * Auditor3 :: PR_BUTTON
 * 
 * This class defines a PR_BUTTON record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System.Text;

namespace Auditor3 {
    internal class PR_BUTTON {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string Number;         // The number for this button
        internal string Type;           // The button type
        internal bool Bridged;          // Flag for if this is a bridged appearance
        internal string BridgedUID;     // The UID for the bridged appearance
        internal string BridgedID;      // The ID for the bridged appearance
        internal bool AMW;              // Flag for if this is an AMW button
        internal string AMWExt;         // The extension for the AMW

        // Constructor for creating the record
        internal PR_BUTTON(string[] prec) {
            // Store the PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the basic values
            UID = line1[1];
            Number = line1[2].Substring(4, 4);
            Type = line1[2].Substring(0, 4);

            // Bridged appearances need additional handling
            if (Type == "0049") {
                Bridged = true;
                BridgedID = line1[3].Substring(0, 4);
                BridgedUID = $"0000{line1[3].Substring(4,4)}";
            }

            // AMW handling
            if (Type == "0046") {
                AMW = true;
                AMWExt = Globals.AMW_EXT(line1[3], line1[4]);
            }
        }

        // Check for if this button has a valid bridged UID
        private bool _hasValidBridgedUID;
        private bool _checkedValidBridgedUID;
        internal bool HasValidBridgedUID() {
            if (!_checkedValidBridgedUID) {
                var pr_stn = Database.PR_STNs.Find(a => a.UID == BridgedUID);
                _hasValidBridgedUID = Bridged && pr_stn != null;
                _checkedValidBridgedUID = true;
            }
            return _hasValidBridgedUID;
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
    }
}
