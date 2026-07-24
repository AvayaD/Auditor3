/*
 * Auditor3 :: PR_MOBD
 * 
 * This class defines a PR_MOBD record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_MOBD {
        internal string[] PREC;         // The raw PREC data
        internal string Board;          // The board address

        // Constructor for creating the record
        internal PR_MOBD(string[] prec) {
            PREC = prec;
            var line1 = prec[0].Split(' ');
            Board = line1[2];
        }
    }
}
