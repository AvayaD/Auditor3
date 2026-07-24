/*
 * Auditor3 :: PR_IANC_BD
 * 
 * This class defines a PR_IANC_BD record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal class PR_IANC_BD {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string Board;          // The board for the record

        // Constructor for creating the record
        internal PR_IANC_BD(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');

            // Parse out the basic values
            UID = line1[1];
            Board = line1[2];
        }

        // Check for if there are duplicate records
        private bool _hasDuplicates;
        private bool _checkedDuplicates;
        internal bool HasDuplicates() {
            if (!_checkedDuplicates) {
                var pr_ianc_bds = Database.PR_IANC_BDs.FindAll(a => a.UID == UID && a.Board == Board);
                _hasDuplicates = pr_ianc_bds.Count > 1;
                _checkedDuplicates = true;
            }
            return _hasDuplicates;
        }
    }
}
