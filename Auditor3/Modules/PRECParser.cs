/*
 * Auditor3 :: PRECParser
 * 
 * This class defines the parser that takes the raw PREC data and generates objects in the database.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace Auditor3 {
    internal static class PRECParser {
        internal static string InputData;               // The raw PREC data
        internal static List<string> ParserDebugInfo;   // Any debug information from the parser

        // This method is used to start the parsing
        internal static void Start() {
            // Set the state and add a status message
            Globals.PROCESS = Process.PARSER;
            Globals.GUI.AddStatus("PREC Parser is starting");
            ParserDebugInfo = new List<string>();

            // Split the input lines and null out the input data
            var lines = InputData.Split('\n');
            InputData = null;

            // Define the variables we need for the parser loop
            var newPrec = true;
            var linesNeeded = 0;
            var index = 0;
            var storage = new string[1];

            // Loop on every line
            foreach (var line in lines) {
                // Check for cancel flag and null lines
                if (Globals.CANCEL) return;

                // Get the PREC type for the line and make sure it is valid
                var type = GetType(line);
                if (type == PRECType.UNKNOWN) continue;

                // Check if this is a new PREC
                if (newPrec) {
                    // Switch on the type and either setup to collect additional lines, or process
                    // single line PRECs
                    switch (type) {

                        // Single line precs
                        case PRECType.PR_AMW:
                        case PRECType.PR_AG_MBR:
                        case PRECType.PR_BRIDGE:
                        case PRECType.PR_EXT:
                        case PRECType.PR_FEXT:
                        case PRECType.PR_GM_IANC_BD:
                        case PRECType.PR_IANC_BD:
                        case PRECType.PR_MOBD:
                        case PRECType.PR_MOPORT:
                        case PRECType.PR_PORT_UID:
                        case PRECType.PR_TTISET:
                        case PRECType.PR_AN_GRP:
                        case PRECType.PR_BARRY:
                            storage = new string[1];
                            storage[0] = line;
                            ProcessPREC(storage);
                            break;

                        // Two line precs
                        case PRECType.PR_ACD_TRUNK:
                        case PRECType.PR_AUDIO_GRP:
                        case PRECType.PR_BUTTON:
                        case PRECType.PR_OPT_STN:
                        case PRECType.PR_TR_MBR:
                            newPrec = false;
                            linesNeeded = 1;
                            storage = new string[2];
                            storage[0] = line;
                            index = 1;
                            break;

                        // Three line precs
                        case PRECType.PR_INT_ANNC:
                            newPrec = false;
                            linesNeeded = 2;
                            storage = new string[3];
                            storage[0] = line;
                            index = 1;
                            break;

                        // Four line PRECs
                        case PRECType.PR_UDATA:
                            newPrec = false;
                            linesNeeded = 3;
                            storage = new string[4];
                            storage[0] = line;
                            index = 1;
                            break;

                        // Seven line precs
                        case PRECType.PR_TRUNK:
                            newPrec = false;
                            linesNeeded = 6;
                            storage = new string[7];
                            storage[0] = line;
                            index = 1;
                            break;

                        // Special cases
                        case PRECType.PR_TR_GRP:
                            if (Globals.CM_RELEASE <= CMRelease.CM7_0) {
                                newPrec = false;
                                linesNeeded = 23;
                                storage = new string[24];
                                storage[0] = line;
                                index = 1;
                                break;
                            } else {
                                newPrec = false;
                                linesNeeded = 22;
                                storage = new string[23];
                                storage[0] = line;
                                index = 1;
                                break;
                            }

                        case PRECType.PR_STN:
                            newPrec = false;



                            if (Globals.CM_RELEASE == CMRelease.CM8_1) {
                                linesNeeded = 10;
                                storage = new string[11];
                            }
                            else if (Globals.CM_RELEASE == CMRelease.CM8_0) {
                                linesNeeded = 7;
                                storage = new string[8];
                            }
                            else if (Globals.CM_RELEASE == CMRelease.CM6_2 || Globals.CM_RELEASE == CMRelease.CM6_0_1 ||
                                Globals.CM_RELEASE == CMRelease.CM5_2_1) {
                                linesNeeded = 2;
                                storage = new string[3];
                            }

                            else if (Globals.CM_RELEASE == CMRelease.CM10_1 || Globals.CM_RELEASE == CMRelease.CM10_2) {
                                linesNeeded = 12;
                                storage = new string[13];

                            } else {
                                linesNeeded = 3;
                                storage = new string[4];
                            }

                            storage[0] = line;
                            index = 1;

                            break;

                        case PRECType.PR_XMAP:
                            if (Globals.CM_RELEASE == CMRelease.CM10_1 || Globals.CM_RELEASE == CMRelease.CM10_2)
                            {
                                newPrec = false;
                                linesNeeded = 1;
                                storage = new string[2];
                                storage[0] = line;
                                index = 1;
                                break;
                            } else {
                                storage = new string[1];
                                storage[0] = line;
                                ProcessPREC(storage);
                                break;
                            }

                        case PRECType.PR_ST_CPS:
                            if (Globals.CM_RELEASE == CMRelease.CM6_2 || Globals.CM_RELEASE == CMRelease.CM6_0_1 ||
                                Globals.CM_RELEASE == CMRelease.CM5_2_1) {
                                storage = new string[1];
                                storage[0] = line;
                                ProcessPREC(storage);
                            } else {
                                newPrec = false;
                                linesNeeded = 1;
                                storage = new string[2];
                                storage[0] = line;
                                index = 1;
                            }
                            break;
                    }
                } 
                // Not a new PREC
                else {
                    // Add the line and update the counters
                    storage[index] = line;
                    index++;
                    linesNeeded--;

                    // Check if we have all the lines and process the PREC if we do
                    if (linesNeeded == 0) {
                        ProcessPREC(storage);
                        newPrec = true;
                    }
                }
            }

            Globals.WORKING_PREC = new string[0];
            Globals.GUI.AddOutput(string.Join(Environment.NewLine, ParserDebugInfo.ToArray()));
            Globals.GUI.AddStatus("PREC Parser has completed");
        }

        // This method processes the raw PRECs into database objects
        private static void ProcessPREC(string[] prec) {
            Globals.WORKING_PREC = prec;

            // Get the PREC type and switch on it to create the proper object and
            // store it in the database
            switch (GetType(prec[0])) {

                //case PRECType.PR_BARRY:
                //    var pr_barry = new PR_BARRY(prec);
                //    Database.PR_BARRYs.Add(pr_barry);
                //    break;

                case PRECType.PR_AMW:
                    var pr_amw = new PR_AMW(prec);
                    Database.PR_AMWs.Add(pr_amw);
                    break;

                case PRECType.PR_ACD_TRUNK:
                    var pr_acd_trunk = new PR_ACD_TRUNK(prec);
                    Database.PR_ACD_TRUNKs.Add(pr_acd_trunk);
                    break;

                case PRECType.PR_AN_GRP:
                    var pr_an_grp = new PR_AN_GRP(prec);
                    Database.PR_AN_GRPs.Add(pr_an_grp);
                    break;

                case PRECType.PR_AG_MBR:
                    var pr_ag_mbr = new PR_AG_MBR(prec);
                    Database.PR_AG_MBRs.Add(pr_ag_mbr);
                    break;

                case PRECType.PR_AUDIO_GRP:
                    var pr_audio_grp = new PR_AUDIO_GRP(prec);
                    Database.PR_AUDIO_GRPs.Add(pr_audio_grp);
                    break;

                case PRECType.PR_BRIDGE:
                    var pr_bridge = new PR_BRIDGE(prec);
                    Database.PR_BRIDGEs.Add(pr_bridge);
                    break;

                case PRECType.PR_BUTTON:
                    var pr_button = new PR_BUTTON(prec);
                    Database.PR_BUTTONs.Add(pr_button);
                    break;

                case PRECType.PR_EXT:
                    var pr_ext = new PR_EXT(prec);
                    Database.PR_EXTs.Add(pr_ext);
                    break;

                case PRECType.PR_FEXT:
                    var pr_fext = new PR_FEXT(prec);
                    Database.PR_FEXTs.Add(pr_fext);
                    break;

                case PRECType.PR_GM_IANC_BD:
                    var pr_gm_ianc_bd = new PR_GM_IANC_BD(prec);
                    Database.PR_GM_IANC_BDs.Add(pr_gm_ianc_bd);
                    break;

                case PRECType.PR_IANC_BD:
                    var pr_ianc_bd = new PR_IANC_BD(prec);
                    Database.PR_IANC_BDs.Add(pr_ianc_bd);
                    break;

                case PRECType.PR_INT_ANNC:
                    var pr_int_annc = new PR_INT_ANNC(prec);
                    Database.PR_INT_ANNCs.Add(pr_int_annc);
                    break;

                case PRECType.PR_MOBD:
                    var pr_mobd = new PR_MOBD(prec);
                    Database.PR_MOBDs.Add(pr_mobd);
                    break;

                case PRECType.PR_MOPORT:
                    var pr_moport = new PR_MOPORT(prec);
                    Database.PR_MOPORTs.Add(pr_moport);
                    break;

                case PRECType.PR_OPT_STN:
                    var pr_opt_stn = new PR_OPT_STN(prec);
                    Database.PR_OPT_STNs.Add(pr_opt_stn);
                    break;

                case PRECType.PR_PORT_UID:
                    var pr_port_uid = new PR_PORT_UID(prec);
                    Database.PR_PORT_UIDs.Add(pr_port_uid);
                    break;

                case PRECType.PR_ST_CPS:
                    var pr_st_cps = new PR_ST_CPS(prec);
                    Database.PR_ST_CPSs.Add(pr_st_cps);
                    break;

                case PRECType.PR_STN:
                    var pr_stn = new PR_STN(prec);
                    Database.PR_STNs.Add(pr_stn);
                    break;

                case PRECType.PR_TR_GRP:
                    var pr_tr_grp = new PR_TR_GRP(prec);
                    Database.PR_TR_GRPs.Add(pr_tr_grp);
                    break;

                case PRECType.PR_TR_MBR:
                    var pr_tr_mbr = new PR_TR_MBR(prec);
                    Database.PR_TR_MBRs.Add(pr_tr_mbr);
                    break;

                case PRECType.PR_TRUNK:
                    var pr_trunk = new PR_TRUNK(prec);
                    // Ignore non ISDN type trunks
                    if (pr_trunk.TrunkGroup.Substring(0, 4) != "0083" && 
                        pr_trunk.TrunkGroup.Substring(0, 4) != "0830") break;
                    Database.PR_TRUNKs.Add(pr_trunk);
                    break;

                case PRECType.PR_TTISET:
                    var pr_ttiset = new PR_TTISET(prec);
                    Database.PR_TTISETs.Add(pr_ttiset);
                    break;

                case PRECType.PR_UDATA:
                    var pr_udata = new PR_UDATA(prec);
                    Database.PR_UDATAs.Add(pr_udata);
                    break;

                case PRECType.PR_XMAP:
                    var pr_xmap = new PR_XMAP(prec);
                    Database.PR_XMAPs.Add(pr_xmap);
                    break;
            }
        }

        // This method gets the PREC type of a line
        private static PRECType GetType(string input) {
            // Make sure this is not a blank line
            if (string.IsNullOrEmpty(input)) return PRECType.UNKNOWN;

            // Split the line into fields
            var fields = input.Split(' ');

            // Switch on the first value to match to a valid PREC type
            switch (fields[0]) {

                case "PR_BARRY": return PRECType.PR_BARRY;

                case "PR_AMW":
                    return PRECType.PR_AMW;

                case "PR_ACD_TRUNK":
                    return PRECType.PR_ACD_TRUNK;

                case "PR_AN_GRP":
                    return PRECType.PR_AN_GRP;

                case "PR_BRIDGE":
                    return PRECType.PR_BRIDGE;

                case "PR_BUTTON":
                    return PRECType.PR_BUTTON;

                case "PR_EXT":
                    return PRECType.PR_EXT;

                case "PR_FEXT":
                    return PRECType.PR_FEXT;

                case "PR_GM_IANC_BD":
                    return PRECType.PR_GM_IANC_BD;

                case "PR_IANC_BD":
                    return PRECType.PR_IANC_BD;

                case "PR_INT_ANNC":
                    return PRECType.PR_INT_ANNC;

                case "PR_MOBD":
                    return PRECType.PR_MOBD;

                case "PR_MOPORT":
                    return PRECType.PR_MOPORT;

                case "PR_OPT_STN":
                    return PRECType.PR_OPT_STN;

                case "PR_PORT_UID":
                    return PRECType.PR_PORT_UID;

                case "PR_STN":
                    return PRECType.PR_STN;

                case "PR_TR_GRP":
                    return PRECType.PR_TR_GRP;

                case "PR_TR_MBR":
                    return PRECType.PR_TR_MBR;

                case "PR_TRUNK":
                    return PRECType.PR_TRUNK;

                case "PR_TTISET":
                    return PRECType.PR_TTISET;

                case "PR_UDATA":
                    return PRECType.PR_UDATA;

                case "PR_ST_CPS":
                    return PRECType.PR_ST_CPS;

                case "PR_XMAP":
                    return PRECType.PR_XMAP;

                // Special handling for unnamed PRECs
                case "PREC":
                    switch (fields[1]) {

                        case "x4c90":
                            return PRECType.PR_AUDIO_GRP;

                        case "x4c91":
                            return PRECType.PR_AG_MBR;

                        default:
                            return PRECType.UNKNOWN;
                    }

                default:
                    return PRECType.UNKNOWN;
            }
        }
    }
}
