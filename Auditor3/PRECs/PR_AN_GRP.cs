/*
 * Auditor3 :: PR_AN_GRP
 * 
 * This class defines a PR_AN_GRP record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_AN_GRP {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string AudioGroup;     // The audio group for the record

        // Constructor for creating the record
        internal PR_AN_GRP(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the data
            UID = line1[1];
            AudioGroup = line1[4].Substring(4, 2);
        }
    }
}
