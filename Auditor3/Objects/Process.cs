/*
 * Auditor3 :: Process
 * 
 * This enum defines the processes that are run in the application.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

namespace Auditor3 {
    internal enum Process {
        NONE,
        INITIALIZING,
        LOADPRECS,
        AUDIT,
        PARSER,
        COLLECT,
        REPAIR,
        EECCR_AUDIT,
        LOADSCRIPT,
        PULLXLN,
        STAGELAB,

        // Auditor sub-processes
        PR_AMW_LOOP,
        PR_STN_LOOP,
        PR_ST_CPS_LOOP,
        PR_EXT_LOOP,
        PR_FEXT_LOOP,
        PR_PORT_UID_LOOP,
        PR_BUTTON_LOOP,
        PR_MOPORT_LOOP,
        PR_BRIDGE_LOOP,
        PR_UDATA_LOOP,
        PR_XMAP_LOOP,
        PR_OPT_STN_LOOP,

        PR_INT_ANNC_LOOP,
        PR_IANC_BD_LOOP,
        PR_GM_IANC_BD_LOOP,
        PR_AUDIO_GRP_LOOP,

        PR_ACD_TRUNK_LOOP,
        PR_TR_GRP_LOOP,
        TRUNKS_LOOP
    }
}
