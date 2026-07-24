/*
 * Auditor3 :: Locations
 * 
 * This class defines the available locations.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System.Collections.Generic;
using System.Linq;

namespace Auditor3.Locations {
    internal static class Locations {
        internal static List<Location> AVAILABLE;

        internal static void INITIALIZE() {
            AVAILABLE = new List<Location> { CORPORTE, AWS, HIPPA, PCI_DAY_2, PCI_LAB };
        }

        internal static Location CURRENT() { return AVAILABLE.FirstOrDefault(a => a.ID == Globals.USER_DATA.Location); }
        internal static Location LOCATION(LocationID id) { return AVAILABLE.FirstOrDefault(a => a.ID == id); }

        internal static Location CORPORTE = new Location(LocationID.CORPORATE, "Avaya Corporate Network", "https://c3ha.avaya.com/conninfo/rest/asg/CorruptionAudit", 
            new string[] { "st3tds04.us1.avaya.com", "st3tds05.us1.avaya.com" },
            new string[] { "drccd.dr.avaya.com" });

        internal static Location AWS = new Location(LocationID.AWS, "AWS Cloud", 
            "https://c3ha.avaya.com/conninfo/rest/asg/CorruptionAudit");

        internal static Location HIPPA = new Location(LocationID.HIPAA, "HIPPA Day 2 & Lab", 
            "https://c3ha.PCI.avaya.com/conninfo/rest/asg/CorruptionAudit");

        internal static Location PCI_DAY_2 = new Location(LocationID.PCI_DAY_2, "PCI Day 2", 
            "https://c3ha.PCI.avaya.com/conninfo/rest/asg/CorruptionAudit");

        internal static Location PCI_LAB = new Location(LocationID.PCI_LAB, "PCI Lab",
            "https://c3ha.PCI.avaya.com/conninfo/rest/asg/CorruptionAudit");
    }
}
