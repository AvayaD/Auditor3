/*
 * Auditor3 :: PR_AG_MBR
 * 
 * This class defines a PR_AG_MBR record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_AG_MBR {
        internal string[] PREC;         // The raw PREC data
        internal string AudioGroup;     // The audio group for the record
        internal string MemberNumber;   // The member number for this record
        internal string Board;          // The board for this record

        // Constructor for creating the record
        internal PR_AG_MBR(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the basic values
            AudioGroup = line1[2].Substring(6, 2);
            MemberNumber = line1[2].Substring(0, 4);
            Board = line1[3];
        }
    }
}
