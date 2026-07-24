/*
 * Auditor3 :: PR_STN
 * 
 * This class defines a PR_STN record.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Collections;

namespace Auditor3 {
    internal class PR_STN {

        internal string[] PREC;         // The raw PREC data
        internal string UID;            // The UID for the record
        internal string GID;            // The GID for the record
        internal StationType Type;      // The type of station for the record
        internal bool AWOH;             // Flag for if this is an AWOH station
        internal string MWLExt;         // The Message Waiting Lamp Extension

        internal bool IsAuditable;      // Flag for if this record should be audited - allows for
                                        // excluding things like agent records from standard audits

        // Constructor for creating the record
        internal PR_STN(string[] prec) {
            // Store the raw PREC
            PREC = prec;

            // Split the lines
            var line1 = prec[0].Split(' ');
            var line2 = prec[1].Split(' ');

            // Parse out the basic values
            UID = line1[1];
            GID = UID.Substring(0, 4);

            // Set the station type
            SetType(line1[2]);

            // Handle more_info flags
            var moreInfoByte = Convert.ToInt64(line1[5].Substring(6, 2), 16);
            var moreInfoFlags = Convert.ToString(moreInfoByte, 2).PadLeft(8, '0');
            AWOH = moreInfoFlags.Substring(3, 1) == "1";

            MWLExt = Globals.UNPACK_EXTENSION(line2[1] + line1[8]);

            // Set the auditable flag
            IsAuditable = UID.Substring(0, 4) == "0000" && Type != StationType.Agent && Type != StationType.Adjunct &&
                Type != StationType.MusicOnHold && Type != StationType.Xmobile && Type != StationType.SbsNoHardware &&
                Type != StationType.Cti && Type != StationType.AnnMusic && Type != StationType.EALERT;
        }

        internal string StationExt() {
            var pr_ext = Database.PR_EXTs.Find(a => a.UID == UID);
            return pr_ext != null ? pr_ext.Digits : null;
        }

        // Check for if this record has a PR_TTISET record
        private bool _hasTTISET;
        private bool _checkedTTISET;
        internal bool HasTTISET() {
            if (!_checkedTTISET) {
                var pr_ttiset = Database.PR_TTISETs.Find(a => a.UID == UID);
                _hasTTISET = pr_ttiset != null;
                _checkedTTISET = true;
            }
            return _hasTTISET;
        }

        // Check for if this record has an AWOH mismatch
        private bool _hasAWOHMismatch;
        private bool _checkedAWOHMismatch;
        internal bool HasAWOHMismatch() {
            if (!_checkedAWOHMismatch) {
                var pr_st_cps = Database.PR_ST_CPSs.Find(a => a.UID == UID);
                var port = pr_st_cps != null ? pr_st_cps.Port : Globals.NULL_PORT;
                _hasAWOHMismatch = false;
                if (AWOH && port != Globals.NULL_PORT && !HasTTISET()) _hasAWOHMismatch = true;
                if (!AWOH && port == Globals.NULL_PORT) _hasAWOHMismatch = true;
                _checkedAWOHMismatch = true;
            }
            return _hasAWOHMismatch;
        }

        // Check for if this record has matching digits in PR_EXT and PR_FEXT
        private bool _hasMatchingDigits;
        private bool _checkedMatchingDigits;
        internal bool HasMatchingDigits() {
            if (!_checkedMatchingDigits) {
                var pr_ext = Database.PR_EXTs.Find(a => a.UID == UID);
                var pr_fext = Database.PR_FEXTs.Find(a => a.UID == UID);
                _hasMatchingDigits = pr_ext != null && pr_fext != null && pr_ext.Digits == pr_fext.Digits;
                _checkedMatchingDigits = true;
            }
            return _hasMatchingDigits;
        }

        // Check for if this record has a PR_EXT with the same UID
        private bool _hasEXT;
        private bool _checkedEXT;
        internal bool HasEXT() {
            if (!_checkedEXT) {
                var check = Database.PR_EXTs.Find(a => a.UID == UID);
                _hasEXT = check != null;
                _checkedEXT = true;
            }
            return _hasEXT;
        }

        // Check for if this record has a PR_FEXT with the same UID
        private bool _hasFEXT;
        private bool _checkedFEXT;
        internal bool HasFEXT() {
            if (!_checkedFEXT) {
                var check = Database.PR_FEXTs.Find(a => a.UID == UID);
                _hasFEXT = check != null;
                _checkedFEXT = true;
            }
            return _hasFEXT;
        }

        // Check for if this record has a PR_UDATA with the same UID
        private bool _hasUDATA;
        private bool _checkedUDATA;
        internal bool HasUDATA() {
            if (!_checkedUDATA) {
                var check = Database.PR_UDATAs.Find(a => a.UID == UID);
                _hasUDATA = check != null;
                _checkedUDATA = true;
            }
            return _hasUDATA;
        }

        // Check for if there is a valid IP port for this station
        private bool _hasValidIPPort;
        private bool _checkedValidIPPort;
        internal bool HasValidIPPort() {
            if (!_checkedValidIPPort) {
                var pr_st_cps = Database.PR_ST_CPSs.Find(a => a.UID == UID);
                _hasValidIPPort = pr_st_cps != null && pr_st_cps.Port.Substring(0, 2) == "7f";
                _checkedValidIPPort = true;
            }
            return _hasValidIPPort;
        }

        // This method is for checking if this is an IP station type
        internal bool IsIP() {
            switch (Type) {
                case StationType.x9620Sip:
                case StationType.GenericH323:
                case StationType.x1603:
                case StationType.x1608:
                case StationType.x1616:
                case StationType.x4602p:
                case StationType.x4606:
                case StationType.x4610:
                case StationType.x4612:
                case StationType.x4620:
                case StationType.x4620Sip:
                case StationType.x4622:
                case StationType.x4624:
                case StationType.x9608:
                case StationType.x9608Sip:
                case StationType.x9608SipCc:
                case StationType.x9610:
                case StationType.x9611:
                case StationType.x9611Sip:
                case StationType.x9611SipCc:
                case StationType.x9620:
                case StationType.x9621:
                case StationType.x9621Sip:
                case StationType.x9630:
                case StationType.x9630Sip:
                case StationType.x9641:
                case StationType.x9641Sip:
                case StationType.x9650:
                case StationType.x9650Sip:
                case StationType.x9621SipCc:
                case StationType.x9641SipCc:
                case StationType.x4630:
                case StationType.x4601p:
                case StationType.J179:
                    return true;

                default:
                    return false;
            }
        }

        // This method is for getting a stations MO value for PR_MOPORT
        internal string GetMO() {
            switch (Type) {
                case StationType.x2402:
                case StationType.x8411d:
                case StationType.x8405:
                case StationType.Bp6408:
                case StationType.Dp8405:
                case StationType.x603d1:
                case StationType.x603a1:
                case StationType.Lcdt:
                case StationType.x602a1:
                case StationType.x1416:
                case StationType.x1408:
                case StationType.x9408:
                case StationType.Dp6424:
                case StationType.Dp6416:
                case StationType.Dp6408:
                case StationType.x8434d:
                case StationType.x8410b:
                case StationType.x603f1:
                case StationType.Console:
                case StationType.x606a1:
                case StationType.x2410:
                case StationType.x6402d:
                case StationType.x7444d:
                case StationType.Idt:
                case StationType.x7410p:
                case StationType.x7407p:
                case StationType.x3150d:
                case StationType.x7410d:
                case StationType.x7405d:
                case StationType.x6416cmv:
                case StationType.x9404:
                case StationType.x8411b:
                case StationType.x7401:
                case StationType.x7434d:
                case StationType.x8405bp:
                case StationType.x8405b:
                case StationType.x7401p:
                    return "0401";

                case StationType.x500Rotary:
                case StationType.x6218:
                case StationType.x3100:
                case StationType.x6210:
                case StationType.x6221:
                case StationType.x7101a:
                    return "040c";

                case StationType.BriAdjunctLink:
                case StationType.BriAsai:
                case StationType.x8503:
                case StationType.x8510:
                case StationType.WorldClassBri:
                    return "0420";

                case StationType.CallrId:
                case StationType.x8110:
                case StationType.x500:
                case StationType.x2500:
                case StationType.Vmi:
                    return "043a";

                case StationType.GenericH323:
                    return "0442";

                case StationType.x1603:
                case StationType.x7434nd:
                case StationType.x1608:
                case StationType.x4606:
                case StationType.x4624:
                case StationType.x9650:
                case StationType.x9641:
                case StationType.x9630:
                case StationType.x9621:
                case StationType.x9620:
                case StationType.x9610:
                case StationType.x9611:
                case StationType.x9608:
                case StationType.x4622:
                case StationType.x4620:
                case StationType.x4610:
                case StationType.x4602p:
                case StationType.x1616:
                case StationType.x8403b:
                case StationType.x4630:
                case StationType.x4601p:
                case StationType.J179:
                    return "0444";

                case StationType.x9650Sip:
                case StationType.x9621Sip:
                case StationType.x9608SipCc:
                case StationType.x4620Sip:
                case StationType.x9608Sip:
                case StationType.x9620Sip:
                case StationType.x9641Sip:
                case StationType.x9611Sip:
                case StationType.x9630Sip:
                case StationType.x9611SipCc:
                case StationType.x9621SipCc:
                case StationType.x9641SipCc:
                    return "044a";

                case StationType.FDVRU:
                case StationType.VRU:
                    return "0412";

                default:
                    return null;
            }
        }

        // This method is used for setting the station type
        private void SetType(string value) {
            // Define a variable to store the type bits
            string type;

            // Get the type bits depending on the release
            if (Globals.CM_RELEASE == CMRelease.CM5_2_1 || Globals.CM_RELEASE == CMRelease.CM6_0_1) {
                type = "0" + value.Substring(6, 2);
            } else {
                type = value.Substring(5, 3);
            }

            // Switch on the type bits and set the type
            switch (type) {
                case "001":
                    Type = StationType.NoSet;
                    break;

                case "005":
                    Type = StationType.x3150d;
                    break;

                case "006":
                    Type = StationType.x7405d;
                    break;

                case "007":
                    Type = StationType.Console;
                    break;

                case "008":
                    Type = StationType.x7101a;
                    break;

                case "009":
                    Type = StationType.x3100;
                    break;

                case "00a":
                    Type = StationType.x8110;
                    break;

                case "00b":
                    Type = StationType.x7211;
                    break;

                case "00e":
                    Type = StationType.x2500;
                    break;

                case "010":
                    Type = StationType.EALERT;
                    break;

                case "015":
                    Type = StationType.Idt;
                    break;

                case "017":
                    Type = StationType.x500;
                    break;

                case "022":
                    Type = StationType.MusicOnHold;
                    break;

                case "02c":
                    Type = StationType.Lcdt;
                    break;

                case "035":
                    Type = StationType.BriAsai;
                    break;

                case "038":
                    Type = StationType.x8510;
                    break;

                case "03f":
                    Type = StationType.x2500;
                    break;

                case "05e":
                    Type = StationType.x7444d;
                    break;

                case "05f":
                    Type = StationType.x7406p;
                    break;

                case "060":
                    Type = StationType.x7401p;
                    break;

                case "061":
                    Type = StationType.x7407p;
                    break;

                case "062":
                    Type = StationType.x7410p;
                    break;

                case "069":
                    Type = StationType.BriAdjunctLink;
                    break;

                case "077":
                    Type = StationType.x602a1;
                    break;

                case "078":
                    Type = StationType.x7434d;
                    break;

                case "07a":
                    Type = StationType.x7410d;
                    break;

                case "084":
                    Type = StationType.x8403b;
                    break;

                case "085":
                    Type = StationType.x8410b;
                    break;

                case "086":
                    Type = StationType.x8410d;
                    break;

                case "087":
                    Type = StationType.x8434d;
                    break;

                case "08b":
                    Type = StationType.x603a1;
                    break;

                case "08c":
                    Type = StationType.x603d1;
                    break;

                case "08e":
                    Type = StationType.Console;
                    break;

                case "08f":
                    Type = StationType.x603e1;
                    break;

                case "090":
                    Type = StationType.VRU;
                    break;

                case "094":
                    Type = StationType.AnnMusic;
                    break;

                case "095":
                    Type = StationType.WorldClassBri;
                    break;

                case "098":
                    Type = StationType.Audix;
                    break;

                case "09d":
                    Type = StationType.FDVRU;
                    break;

                case "09f":
                    Type = StationType.x8411b;
                    break;

                case "0a0":
                    Type = StationType.x8411d;
                    break;

                case "0a2":
                    Type = StationType.x7434nd;
                    break;

                case "0a4":
                    Type = StationType.x8405b;
                    break;

                case "0a5":
                    Type = StationType.x8405bp;
                    break;

                case "0a6":
                    Type = StationType.x8405;
                    break;

                case "0a7":
                    Type = StationType.Dp8405;
                    break;

                case "0a9":
                    Type = StationType.x606a1;
                    break;

                case "0ac":
                    Type = StationType.x6402b;
                    break;

                case "0ad":
                    Type = StationType.x6408b;
                    break;

                case "0ae":
                    Type = StationType.Bp6408;
                    break;

                case "0af":
                    Type = StationType.x6408d;
                    break;

                case "0b0":
                    Type = StationType.Dp6408;
                    break;

                case "0b1":
                    Type = StationType.Dp6416;
                    break;

                case "0b2":
                    Type = StationType.Dp6424;
                    break;

                case "0b3":
                    Type = StationType.Vmi;
                    break;

                case "0b7":
                    Type = StationType.x6402d;
                    break;

                case "0b8":
                    Type = StationType.AnalogAwoh;
                    break;

                case "0ca":
                    Type = StationType.x9408;
                    break;

                case "0c4":
                    Type = StationType.x603f1;
                    break;

                case "0c5":
                    Type = StationType.x9601p;
                    break;

                case "0c6":
                    Type = StationType.Xmobile;
                    break;

                case "0c7":
                    Type = StationType.x6416cmv;
                    break;

                case "0c8":
                    Type = StationType.GenericH323;
                    break;

                case "0c9":
                    Type = StationType.x9404;
                    break;

                case "0cd":
                    Type = StationType.x6210;
                    break;

                case "0ce":
                    Type = StationType.x6218;
                    break;

                case "0cf":
                    Type = StationType.x6221;
                    break;

                case "0d0":
                    Type = StationType.CallrId;
                    break;

                case "0d1":
                    Type = StationType.Cti;
                    break;

                case "0d3":
                    Type = StationType.x4612;
                    break;

                case "0d4":
                    Type = StationType.x4624;
                    break;

                case "0d5":
                    Type = StationType.x4606;
                    break;

                case "0d6":
                    Type = StationType.x4630;
                    break;

                case "0d7":
                    Type = StationType.Adjunct;
                    break;

                case "0d8":
                    Type = StationType.Adjunct;
                    break;

                case "0d9":
                    Type = StationType.x2420;
                    break;

                case "0db":
                    Type = StationType.x4620;
                    break;

                case "0dc":
                    Type = StationType.SbsNoHardware;
                    break;

                case "0df":
                    Type = StationType.x2402;
                    break;

                case "0e0":
                    Type = StationType.x4610;
                    break;

                case "0e1":
                    Type = StationType.x2410;
                    break;

                case "0e6":
                    Type = StationType.x4622;
                    break;

                case "0e7":
                    Type = StationType.x4601p;
                    break;

                case "0e8":
                    Type = StationType.x4602p;
                    break;

                case "0e9":
                    Type = StationType.x9610;
                    break;

                case "0ea":
                    Type = StationType.x9620;
                    break;

                case "0eb":
                    Type = StationType.x9630;
                    break;

                case "0ec":
                    Type = StationType.x9650;
                    break;

                case "0ed":
                    Type = StationType.x1603;
                    break;

                case "0ee":
                    Type = StationType.x1608;
                    break;

                case "0ef":
                    Type = StationType.x1616;
                    break;

                case "0f0":
                    Type = StationType.x4620Sip;
                    break;

                case "0f3":
                    Type = StationType.x9620Sip;
                    break;

                case "0f4":
                    Type = StationType.x9630Sip;
                    break;

                case "0f5":
                    Type = StationType.x9650Sip;
                    break;

                case "0f7":
                    Type = StationType.x1408;
                    break;

                case "0f8":
                    Type = StationType.x1416;
                    break;

                case "075":
                    Type = StationType.OpsPrimary;
                    break;

                case "076":
                    Type = StationType.Agent;
                    break;

                case "100":
                    Type = StationType.x9608Sip;
                    break;

                case "101":
                    Type = StationType.x9611Sip;
                    break;

                case "102":
                    Type = StationType.x9621Sip;
                    break;

                case "103":
                    Type = StationType.x9641Sip;
                    break;

                case "104":
                    Type = StationType.x9608SipCc;
                    break;

                case "105":
                    Type = StationType.x9611SipCc;
                    break;

                case "106":
                    Type = StationType.x9621SipCc;
                    break;

                case "107":
                    Type = StationType.x9641SipCc;
                    break;

                case "108":
                    Type = StationType.x9608;
                    break;

                case "109":
                    Type = StationType.x9611;
                    break;

                case "10a":
                    Type = StationType.x9621;
                    break;

                case "10b":
                    Type = StationType.x9641;
                    break;

                case "10e":
                    Type = StationType.J179;
                    break;

                default:
                    Type = StationType.Unknown;
                    Database.AddMissingStationType(type, UID);
                    break;
            }
        }
    }
}
