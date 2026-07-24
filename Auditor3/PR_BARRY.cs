using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auditor3 {
    internal class PR_BARRY {
        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string GID = string.Empty;            // The GID for the record
        internal string Digits = string.Empty;         // The digits for the extension

        internal PR_BARRY(string[] prec) {
            PREC = prec;
            var split = prec[0].Split(' ');
            UID = split[1];
        }
    }
}
