drccd [43925]-> cat findprecs
#!/bin/bash
A="${1:-display}"
O="${2:-station}"
Q="${3}"

echo "?? Deriving file paths dynamically..."
STRINGS_FILE=$(cs -L -7 strings.ct 2>&1 | grep -v "tui" | grep -v "whence" | grep -v "Using" | awk '{print $1}' | head -1)
if [ ! -f "$STRINGS_FILE" ]; then
    echo "? FATAL: Could not find 'strings.ct'"
    exit 1
fi
echo "   Found STRINGS_FILE: $STRINGS_FILE"

# Temp file to store DREC/PREC pairs for tree
TREE_FILE=$(mktemp)
trap "rm -f $TREE_FILE" EXIT

# Global to store actual AREC ID
ACTUAL_AREC=""

find_precs() {
  local DRECS="$1"
  [ -z "$DRECS" ] && return
  echo ""
  echo "========== Finding PRECs from DRECs =========="
  for DREC in $DRECS; do
    echo "?? Looking up DREC: $DREC"
    MAP_FILE=$(cs -L -0 "$DREC" 2>&1 | grep -v "fmap" | grep -v "whence" | grep -v "Using" | grep "map_tbl" | awk '{print $1}' | uniq | head -1)
    if [ -z "$MAP_FILE" ] || [ ! -f "$MAP_FILE" ]; then
      echo "   ? Map file not found for $DREC"
      continue
    fi
    echo "   Found map table: $MAP_FILE"

    # Extract map_tbl number (map_tbl92.c -> 92)
    TNUM=$(echo "$MAP_FILE" | sed 's/.*map_tbl\([0-9]*\)\.c/\1/')
    if [ -z "$TNUM" ]; then
      PGET_NAME="Pget_"
    else
      PGET_NAME="Pget_${TNUM}"
    fi

    # Find the Pget_N array start using grep -n
    PSTART=$(grep -n "^struct.*${PGET_NAME}" "$MAP_FILE" | head -1)
    if [ -z "$PSTART" ]; then
      PSTART=$(grep -n "${PGET_NAME}\[" "$MAP_FILE" | head -1)
    fi

    if [ -z "$PSTART" ]; then
      echo "   ? No array ${PGET_NAME} found"
      continue
    fi

    # Extract line number from "123:content" format
    PSTART_LINE=$(echo "$PSTART" | cut -d: -f1)

    # Use sed to extract from PSTART_LINE to closing };
    PCONTENT=$(sed -n "${PSTART_LINE},/^};/p" "$MAP_FILE")

    # Find line number within PCONTENT where DREC marker exists
    # We need to find: {-FAKEDMBASE-${DREC},
    DLINE=$(echo "$PCONTENT" | grep -n "FAKEDMBASE.*${DREC}" | head -1 | cut -d: -f1)

    if [ -z "$DLINE" ]; then
      echo "   ? DREC entry for $DREC not found in map table"
      continue
    fi

    # Now extract lines AFTER the DREC marker
    # Starting from DLINE+1 until we hit next FAKEDMBASE or DMTBLEND or };
    LINE_COUNT=$(echo "$PCONTENT" | wc -l)
    REMAINING=$((LINE_COUNT - DLINE))

    # Use sed to get lines after DLINE until terminator
    echo "$PCONTENT" | sed -n "$((DLINE + 1)),\$p" | while read -r line; do
      # Check termination conditions
      if echo "$line" | grep "FAKEDMBASE" > /dev/null 2>&1; then
        break
      fi
      if echo "$line" | grep "DMTBLEND" > /dev/null 2>&1; then
        break
      fi
      if echo "$line" | grep "^};" > /dev/null 2>&1; then
        break
      fi

      # Check if this line has a PR_ entry
      if echo "$line" | grep "PR_" > /dev/null 2>&1; then
        # Extract PREC name - remove leading whitespace and {, extract until comma
        # Step 1: Remove leading whitespace
        CLEANED=$(echo "$line" | sed 's/^[[:space:]]*//')
        # Step 2: Remove opening brace
        CLEANED=$(echo "$CLEANED" | sed 's/^{//')
        # Step 3: Extract everything up to the comma
        PREC=$(echo "$CLEANED" | sed 's/,.*//')

        if [ -n "$PREC" ]; then
          echo "   ? Mapped DREC: $DREC -> PREC: $PREC"
          echo "$DREC|$PREC" >> "$TREE_FILE"
        fi
      fi
    done
  done
}

find_drecs() {
  local AID="$1"
  ACTUAL_AREC="$AID"
  echo ""
  echo "========== Finding DRECs for AREC: $AID =========="
  AFILE=$(cs -L -0 "$AID" 2>&1 | grep -v "whence" | grep -v "Using" | grep "arec" | grep "_tbl" | awk '{print $1}' | head -1)
  if [ -z "$AFILE" ] || [ ! -f "$AFILE" ]; then
      echo "? FATAL: No AREC table found via cscope."
      return 1
  fi
  echo "?? AREC table file found: $AFILE"
  AENTRY=$(grep -n "$AID," "$AFILE" | head -1)
  [ -z "$AENTRY" ] && echo "? AREC $AID not found in $AFILE" && return
  ALINE=$(echo "$AENTRY" | cut -d: -f1)
  ASTRUCT=$(sed -n "${ALINE},/^}/p" "$AFILE")

  # Determine which table to use based on action
  case "$A" in
      add)    TABLE_LINE=7 ;;
      change) TABLE_LINE=8 ;;
      remove) TABLE_LINE=9 ;;
      *)      TABLE_LINE=6 ;;  # display and others default to get table
  esac
  echo "?? Using table line $TABLE_LINE for action: $A"

  GTABLE=$(echo "$ASTRUCT" | sed -n "${TABLE_LINE}p" | awk '{print $1}' | sed 's/,$//')
  [ -z "$GTABLE" ] && echo "? Table not found for AREC $AID (line $TABLE_LINE)" && return
  echo "?? Table: $GTABLE"
  GFILE=$(cs -L -1 "$GTABLE" 2>&1 | grep -v "^Using" | grep -v "whence" | awk '{print $1}' | head -1)
  if [ -z "$GFILE" ] || [ ! -f "$GFILE" ]; then
    echo "? Table file not found via cscope."
    return
  fi
  echo "?? Table defined in: $GFILE"
  TSTART=$(grep -n "^.*${GTABLE}\[\]" "$GFILE" | head -1 | cut -d: -f1)
  [ -z "$TSTART" ] && echo "? Table $GTABLE not found in $GFILE" && return
  DTABLE=$(sed -n "${TSTART},/^};/p" "$GFILE")
  DRECS=$(echo "$DTABLE" | grep "DM_" | sed 's/^[[:space:]]*//;s/,.*//' | grep "^DM_")
  find_precs "$DRECS"
}

process_parameter() {
  local PNAME="$1"
  local NNAME="$2"
  local CTABLE="$3"
  local CFILE="$4"
  local DEPTH="$5"
  [ $DEPTH -gt 10 ] && echo "? Max recursion depth" && return
  echo ""
  echo "?? [Level $DEPTH] Processing parameter: $PNAME in table: $CTABLE"
  PVAR=$(grep -i "\"${PNAME}\"" "$STRINGS_FILE" | awk '{print $2}' | sed 's/\[\].*//' | head -1)
  [ -z "$PVAR" ] && echo "? Parameter $PNAME not found in $STRINGS_FILE" && return
  echo "   Variable: $PVAR"
  TSTART=$(grep -n "^C_WORDS ${CTABLE}\[\]" "$CFILE" | head -1 | cut -d: -f1)
  [ -z "$TSTART" ] && echo "? Table $CTABLE not found in $CFILE" && return
  ENTRY=$(sed -n "${TSTART},/^};/p" "$CFILE" | grep -n "$PVAR" | cut -d: -f1 | head -1)
  [ -z "$ENTRY" ] && echo "? Entry for $PVAR not found in $CTABLE" && return
  ETEXT=$(sed -n "${TSTART},/^};/p" "$CFILE" | sed -n "$((ENTRY-1)),$((ENTRY+2))p")
  if echo "$ETEXT" | grep "CC_OBJECT" > /dev/null 2>&1 || echo "$ETEXT" | grep "CC_ACTION" > /dev/null 2>&1; then
    echo ""
    echo "? FOUND CC_OBJECT/CC_ACTION!"
    AREC_ID=$(echo "$ETEXT" | sed -n '3p' | awk '{print $1}' | sed 's/,$//')
    echo "?? AREC ID: $AREC_ID"
    find_drecs "$AREC_ID"
    return 0
  fi
  if echo "$ETEXT" | grep "IOBJECT" > /dev/null 2>&1 || echo "$ETEXT" | grep "IACTION" > /dev/null 2>&1; then
    echo "??  Found IOBJECT/IACTION, following chain..."
    NTABLE=$(echo "$ETEXT" | sed -n 's/.*OBJ_NODE(\([^)]*\)).*/\1/p')
    if [ -n "$NTABLE" ] && [ "$NTABLE" != "NULL" ]; then
      echo "?? Next table: $NTABLE"
      NFILE=$(cs -L -1 "$NTABLE" 2>&1 | grep -v "^Using" | grep -v "whence" | awk '{print $1}' | head -1)
      if [ -n "$NFILE" ] && [ -f "$NFILE" ]; then
        if [ -n "$NNAME" ]; then
          process_parameter "$NNAME" "" "$NTABLE" "$NFILE" $((DEPTH+1))
        else
          process_parameter "$PNAME" "" "$NTABLE" "$NFILE" $((DEPTH+1))
        fi
      else
        echo "? Table file for $NTABLE not found" && return 1
      fi
    else
      echo "? No next table" && return 1
    fi
  else
    echo "? Entry is neither CC_OBJECT/CC_ACTION nor IOBJECT/IACTION" && return 1
  fi
}

print_tree() {
    echo ""
    echo "======================================================================"
    echo "                      MAPPING TREE DIAGRAM"
    echo "======================================================================"
    echo ""
    echo "ACTION: $A"
    echo "+-- OBJECT: $O"
    if [ -n "$Q" ]; then
        echo "    +-- QUALIFIER: $Q"
    fi
    echo "    +-- AREC: $ACTUAL_AREC"
    echo "        &"

    # Get unique DRECs and sort them
    if [ -s "$TREE_FILE" ]; then
        cut -d'|' -f1 "$TREE_FILE" | sort -u | while read drec; do
            [ -z "$drec" ] && continue

            # Count PRECs for this DREC
            prec_count=$(grep "^${drec}|" "$TREE_FILE" | cut -d'|' -f2 | sort -u | wc -l)

            # Format output with DREC and count
            printf "        +-- %-25s ( %d PRECs)\n" "$drec" "$prec_count"

            # Get and print all PRECs for this DREC
            grep "^${drec}|" "$TREE_FILE" | cut -d'|' -f2 | sort -u | while read prec; do
                [ -z "$prec" ] && continue
                echo "            +-- $prec"
            done
        done
    else
        echo "        (No DREC/PREC mappings found)"
    fi

    echo ""
}

main() {
    [ -z "$1" ] && echo "Usage: $0 <action> [object]" && return 1
    local AVAR="S_${1}"
    echo "?? Looking for: $AVAR (${1})"
    local SFILE=$(cs -L -0 "$AVAR" 2>&1 | grep -v "whence" | grep -v "Using" | grep action | awk '{print $1}' | head -1)
    if [ -z "$SFILE" ] || [ ! -f "$SFILE" ]; then
        echo "? Starting action variable '$AVAR' not found"
        return 1
    fi
    echo "?? Found in: $SFILE"
    local TNAME=$(sed -n "/$AVAR/{p;N;N;N;p;}" "$SFILE" | grep "CC_ACTION" | sed -n 's/.*OBJ_NODE(\([^)]*\)).*/\1/p' | head -1)
    [ -z "$TNAME" ] && echo "? No starting table" && return 1
    echo "?? Starting AREC Table: $TNAME"
    local TFILE=$(cs -L -1 "$TNAME" 2>&1 | grep -v "^Using" | grep -v "whence" | awk '{print $1}' | head -1)
    if [ -z "$TFILE" ] || [ ! -f "$TFILE" ]; then
        echo "? File for table '$TNAME' not found"
        return 1
    fi
    echo "?? Table defined in: $TFILE"
    echo ""
    echo "========== Processing OBJECT =========="
    process_parameter "${2:-station}" "${3}" "$TNAME" "$TFILE" 1

    # Print tree diagram at end
    print_tree
}

main "$A" "$O" "$Q"
