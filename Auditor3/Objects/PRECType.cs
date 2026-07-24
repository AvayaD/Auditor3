/*
 * Auditor3 :: PRECType
 * 
 * This enum defines the various PREC types supported in the application.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal enum PRECType {
        UNKNOWN,

        PR_BARRY,

        PR_AMW,
        PR_BRIDGE,
        PR_BUTTON,
        PR_EXT,
        PR_FEXT,
        PR_MOBD,
        PR_MOPORT,
        PR_OPT_STN,
        PR_PORT_UID,
        PR_ST_CPS,
        PR_STN,
        PR_TTISET,
        PR_UDATA,
        PR_XMAP,

        PR_AN_GRP,
        PR_AG_MBR,
        PR_GM_IANC_BD,
        PR_IANC_BD,
        PR_INT_ANNC,
        PR_AUDIO_GRP,

        PR_ACD_TRUNK,
        PR_TR_GRP,
        PR_TR_MBR,
        PR_TRUNK
    }
}
