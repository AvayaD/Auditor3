/*
 * Auditor3 :: PR_EXT
 * 
 * This class defines a PR_EXT record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System.Linq;

namespace Auditor3 {
    internal class PR_EXT {

        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string GID;            // The GID for the record
        internal string Digits;         // The digits for the extension
        
        // Constructor for creating the record
        internal PR_EXT(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the basic values
            UID = line1[4];
            GID = UID.Substring(0, 4);

            // Convert the extension digits
            SetDigits(line1[1], line1[2]);
        }

        // This method is used to get the PREC fields for removing this record
        internal string PRECFields() {
            var line = PREC[0].Split(' ');
            return $"l0x{line[1]} l0x{line[2]}";
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

        // This method is used to get the extension digits from the PREC words
        private void SetDigits(string field1, string field2) {
            // Take the upper bytes from the first field so we can drop the length value
            field1 = field1.Substring(0, 4);

            // Reverse the digit strings
            field1 = new string(field1.ToCharArray().Reverse().ToArray());
            field2 = new string(field2.ToCharArray().Reverse().ToArray());

            // Combine the two fields
            var digits = field1 + field2;

            // Iterate through the digits until we hit a 0, b, or f, signifying the end of the string, 
            // then convert any 'a' to the actual digit 0, then aggregate it into a string and store it
            Digits = digits.TakeWhile(digit => digit != '0' && digit != 'b' && digit != 'f').Select(digit => digit == 'a' ? '0' : digit).Aggregate("", (current, actualDigit) => current + actualDigit);
        }
    }
}
