/*
 * Auditor3 :: PR_AMW
 * 
 * This class defines a PR_AMW record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System.Text;

namespace Auditor3 {
    internal class PR_AMW {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string ActualUID;      // The actual UID for the station with high bit corrected
        internal bool IsMWI;            // Flag for if this is a station MWI instead of aut_msg_wt button
        internal string Extension;      // The extension being monitored
        internal bool DupFlagged;       // Flag for if this record has been flagged as duplicate
        internal string UnpackedExt;

        // Constructor for creating the record
        internal PR_AMW(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the data
            UID = line1[3];

            if (UID.StartsWith("8")) {
                IsMWI = true;
                ActualUID = "0" + UID.Remove(0, 1);
            } else ActualUID = UID;

            Extension = Globals.AMW_EXT(line1[1], line1[2]);
            UnpackedExt = Globals.UNPACK_EXTENSION(line1[2] + line1[1]);
        }

        // Method for checking if this records has duplicates
        internal bool HasDuplicates() {
            var check = Database.PR_AMWs.FindAll(a => a.UID == UID && a.Extension == Extension);
            if (check.Count > 1) foreach (var rec in check) rec.DupFlagged = true;
            return check.Count > 1;
        }

        // Method for checking if there is a second record with a MWI
        internal bool HasMWI() {
            var check = Database.PR_AMWs.Find(a => a.ActualUID == ActualUID && a.Extension == Extension && IsMWI);
            return check != null;
        }

        // Method for checking if this record is mismatched
        internal bool IsMismatched() {
            var check = Database.PR_BUTTONs.Find(a => a.AMW && a.AMWExt == Extension && a.UID == ActualUID);

            if (IsMWI && check != null && !HasMWI()) return true;
            if (!IsMWI && check == null) return true;
            return false;
        }

        // Method for checking if MWL_EXT is mismatched
        internal bool MwlExtMismatch() {
            if (!IsMWI && !IsMismatched()) return false;
            var pr_stn = Database.PR_STNs.Find(a => a.UID == ActualUID);
            if (pr_stn == null) return false;
            return pr_stn.MWLExt != UnpackedExt;
        }
    }
}
