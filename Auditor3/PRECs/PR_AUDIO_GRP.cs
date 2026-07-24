/*
 * Auditor3 :: PR_AUDIO_GRP
 * 
 * This class defines a PR_AUDIO_GRP record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_AUDIO_GRP {
        internal string[] PREC;         // The raw PREC data
        internal string AudioGroup;     // The audio group for the record

        // Constructor for creating the record
        internal PR_AUDIO_GRP(string[] prec) {
            PREC = prec;
            
            var line1 = prec[0].Split(' ');

            AudioGroup = line1[2].Substring(6, 2);
        }
    }
}
