/*
 * Auditor3 :: PR_TTISET
 * 
 * This class defines a PR_TTISET record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_TTISET {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID of the record

        // Constructor for creating the record
        internal PR_TTISET(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the basic values
            UID = line1[1];
        }
    }
}
