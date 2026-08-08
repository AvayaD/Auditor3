#!/bin/bash

# ============================================================================
# AREC/DREC/PREC Mapper with Hybrid Visualization
# ============================================================================

set -o pipefail
A="${1:-display}"
O="${2:-station}"
Q="${3}"

# Color codes (optional, disable with NO_COLOR=1)
if [ -z "$NO_COLOR" ]; then
    RESET='\033[0m'
    BOLD='\033[1m'
    DIM='\033[2m'
    BLUE='\033[34m'
    YELLOW='\033[33m'
    GREEN='\033[32m'
    CYAN='\033[36m'
else
    RESET='' BOLD='' DIM='' BLUE='' YELLOW='' GREEN='' CYAN=''
fi

# Output format (tree|json|graphviz|all)
OUTPUT_FORMAT="${OUTPUT_FORMAT:-tree}"

# Global state
declare -A DREC_MAP  # DREC -> count
declare -a DREC_LIST # Ordered DREC names
declare -A PREC_MAP  # DREC -> "PREC1 PREC2 ..."
AREC_NAME=""
ACTION_NAME=""
OBJECT_NAME=""

echo "?? Deriving file paths dynamically..."
STRINGS_FILE=$(cs -L -7 strings.ct 2>&1 | grep -v "tui" | grep -v "whence" | awk '{print $1}' | head -1)
if [ ! -f "$STRINGS_FILE" ]; then
    echo "âŒ FATAL: Could not find 'strings.ct'"
    exit 1
fi
echo "âœ… Found STRINGS_FILE: $STRINGS_FILE"

# ============================================================================
# VISUALIZATION FUNCTIONS
# ============================================================================

output_tree_header() {
    echo ""
    echo "${BOLD}${BLUE}â•”â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•—${RESET}"
    echo "${BOLD}${BLUE}â•‘   AREC â†’ DREC â†’ PREC Mapping Tree     â•‘${RESET}"
    echo "${BOLD}${BLUE}â•šâ•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•${RESET}"
    echo ""
    echo "${CYAN}${BOLD}ACTION:${RESET} ${ACTION_NAME}"
    echo "${CYAN}${BOLD}OBJECT:${RESET} ${OBJECT_NAME}"
    echo "${CYAN}${BOLD}AREC:${RESET}   ${YELLOW}${AREC_NAME}${RESET}"
    echo ""
}

output_tree_body() {
    local count=0
    local total=${#DREC_LIST[@]}

    for drec in "${DREC_LIST[@]}"; do
        ((count++))
        local is_last=0
        [ $count -eq $total ] && is_last=1

        # Draw DREC node
        if [ $is_last -eq 1 ]; then
            echo -n "    ${GREEN}â””â”€â”€${RESET} "
            local indent="        "
        else
            echo -n "    ${GREEN}â”œâ”€â”€${RESET} "
            local indent="    â”‚   "
        fi

        local prec_count=${DREC_MAP[$drec]}
        echo "${YELLOW}${drec}${RESET} ${DIM}(${prec_count} PRECs)${RESET}"

        # Draw PREC subnodes (expandable marker)
        local precs=(${PREC_MAP[$drec]})
        local prec_total=${#precs[@]}

        if [ $prec_total -gt 3 ]; then
            # Show first 3, then "..." for readability
            for i in {0..2}; do
                [ $i -eq 2 ] && echo -n "${indent}${GREEN}â”œâ”€â”€${RESET} ${GREEN}${precs[$i]}${RESET}" || \
                               echo -n "${indent}${GREEN}â”œâ”€â”€${RESET} ${GREEN}${precs[$i]}${RESET}"
                echo ""
            done
            echo "${indent}${CYAN}â”œâ”€â”€ ${DIM}... ${prec_total-3} more (use --full)${RESET}"
        else
            for i in $(seq 0 $((prec_total-1))); do
                if [ $i -eq $((prec_total-1)) ]; then
                    echo "${indent}${GREEN}â””â”€â”€${RESET} ${GREEN}${precs[$i]}${RESET}"
                else
                    echo "${indent}${GREEN}â”œâ”€â”€${RESET} ${GREEN}${precs[$i]}${RESET}"
                fi
            done
        fi
    done
    echo ""
}

output_tree_full() {
    output_tree_header

    local count=0
    local total=${#DREC_LIST[@]}

    for drec in "${DREC_LIST[@]}"; do
        ((count++))
        local is_last=0
        [ $count -eq $total ] && is_last=1

        if [ $is_last -eq 1 ]; then
            echo -n "    ${GREEN}â””â”€â”€${RESET} "
            local indent="        "
        else
            echo -n "    ${GREEN}â”œâ”€â”€${RESET} "
            local indent="    â”‚   "
        fi

        local prec_count=${DREC_MAP[$drec]}
        echo "${YELLOW}${drec}${RESET} ${DIM}(${prec_count} PRECs)${RESET}"

        local precs=(${PREC_MAP[$drec]})
        for i in $(seq 0 $((${#precs[@]}-1))); do
            if [ $i -eq $((${#precs[@]}-1)) ]; then
                echo "${indent}${GREEN}â””â”€â”€${RESET} ${GREEN}${precs[$i]}${RESET}"
            else
                echo "${indent}${GREEN}â”œâ”€â”€${RESET} ${GREEN}${precs[$i]}${RESET}"
            fi
        done
    done
    echo ""
}

output_table() {
    echo ""
    echo "${BOLD}${CYAN}â•”â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•—${RESET}"
    echo "${BOLD}${CYAN}â•‘         DREC â†’ PREC Summary Table                           â•‘${RESET}"
    echo "${BOLD}${CYAN}â•šâ•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•${RESET}"
    echo ""
    echo "${BOLD}$(printf '%-20s %-8s %-30s' 'DREC' 'Count' 'Sample PRECs')${RESET}"
    echo "$(printf '%0.sâ”€' {1..58})"

    for drec in "${DREC_LIST[@]}"; do
        local prec_count=${DREC_MAP[$drec]}
        local precs=(${PREC_MAP[$drec]})
        local sample="${precs[0]}"
        [ $prec_count -gt 1 ] && sample="${precs[0]}, ${precs[1]}"
        [ $prec_count -gt 2 ] && sample="${sample}, ..."

        printf '%-20s %-8d %-30s\n' "$drec" "$prec_count" "$sample"
    done
    echo ""
}

output_json() {
    local precs_json="{"
    local first=1

    for drec in "${DREC_LIST[@]}"; do
        [ $first -eq 0 ] && precs_json="${precs_json},"
        precs_json="${precs_json}\"${drec}\":["

        local precs=(${PREC_MAP[$drec]})
        local prec_first=1
        for prec in "${precs[@]}"; do
            [ $prec_first -eq 0 ] && precs_json="${precs_json},"
            precs_json="${precs_json}\"${prec}\""
            prec_first=0
        done
        precs_json="${precs_json}]"
        first=0
    done
    precs_json="${precs_json}}"

    cat <<EOF
{
  "action": "${ACTION_NAME}",
  "object": "${OBJECT_NAME}",
  "arec": {
    "name": "${AREC_NAME}",
    "drec_count": ${#DREC_LIST[@]},
    "prec_total": $(for drec in "${DREC_LIST[@]}"; do echo ${DREC_MAP[$drec]}; done | awk '{s+=$1} END {print s}'),
    "drecs": ${precs_json}
  }
}
EOF
}

output_graphviz() {
    local dot_file="/tmp/arec_${ACTION_NAME}_${OBJECT_NAME}.dot"

    cat > "$dot_file" << 'DOTEOF'
digraph AREC_DREC_PREC {
    rankdir=LR;
    splines=ortho;

    node [fontname="Arial", fontsize=9];
    edge [color="#666666"];

    // AREC node
    arec [label="AREC\n{AREC_NAME}", shape=box, style="rounded,filled", fillcolor="#ADD8E6", color="#0066CC", penwidth=2];
DOTEOF

    # Add DREC nodes
    local count=0
    for drec in "${DREC_LIST[@]}"; do
        ((count++))
        cat >> "$dot_file" << DOTEOF
    drec_${count} [label="${drec}\n(${DREC_MAP[$drec]})", shape=box, style="rounded,filled", fillcolor="#FFFFE0", color="#999900"];
    arec -> drec_${count};
DOTEOF
    done

    # Add PREC nodes (sample - first 3 per DREC to avoid clutter)
    count=0
    for drec in "${DREC_LIST[@]}"; do
        ((count++))
        local precs=(${PREC_MAP[$drec]})
        local prec_shown=0
        for prec in "${precs[@]}"; do
            [ $prec_shown -lt 3 ] || break
            cat >> "$dot_file" << DOTEOF
    prec_${count}_${prec_shown} [label="${prec}", shape=ellipse, style="filled", fillcolor="#90EE90"];
    drec_${count} -> prec_${count}_${prec_shown};
DOTEOF
            ((prec_shown++))
        done
        [ ${#precs[@]} -gt 3 ] && echo "    prec_${count}_more [label=\"... +$((${#precs[@]}-3)) more\", shape=ellipse, style=\"filled,dashed\", fillcolor=\"#CCCCCC\"];" >> "$dot_file"
    done

    cat >> "$dot_file" << 'DOTEOF'
}
DOTEOF

    # Try to render
    if command -v dot >/dev/null 2>&1; then
        dot -Tpng "$dot_file" -o "${dot_file%.dot}.png" 2>/dev/null
        echo "ðŸ“Š Graph rendered: ${dot_file%.dot}.png"
    fi

    echo "ðŸ“„ DOT file: $dot_file"
    cat "$dot_file"
}

# ============================================================================
# CORE MAPPING FUNCTIONS (unchanged from original)
# ============================================================================

find_precs() {
  local DRECS="$1"
  [ -z "$DRECS" ] && return

  for DREC in $DRECS; do
    MAP_FILE=$(cs -L -0 "$DREC" 2>&1 | grep -v "fmap" | grep -v "whence" | grep "map_tbl" | awk '{print $1}' | uniq | head -1)
    if [ -z "$MAP_FILE" ] || [ ! -f "$MAP_FILE" ]; then
      continue
    fi

    TNUM=$(echo "$MAP_FILE" | sed -n 's/.*map_tbl\([0-9]*\)\.c/\1/p')
    if [ -z "$TNUM" ]; then
        PGET_NAME="Pget_"
    else
        PGET_NAME="Pget_${TNUM}"
    fi

    PSTART=$(grep -n "${PGET_NAME}\[" "$MAP_FILE" | head -1 | cut -d: -f1)
    if [ -z "$PSTART" ]; then
        PSTART=$(grep -n "${PGET_NAME}" "$MAP_FILE" | grep "struct" | head -1 | cut -d: -f1)
        [ -z "$PSTART" ] && continue
    fi

    PCONTENT=$(sed -n "${PSTART},/};/p" "$MAP_FILE")
    DLINE=$(echo "$PCONTENT" | grep -n "FAKEDMBASE.*${DREC}" | head -1 | cut -d: -f1)
    if [ -z "$DLINE" ]; then
        continue
    fi

    # Collect PRECs in a temp file (avoid subshell issue)
    local prec_temp=$(mktemp)
    echo "$PCONTENT" | sed -n "$((DLINE + 1)),\$p" | while read -r line; do
        if echo "$line" | grep "FAKEDMBASE" >/dev/null 2>&1 || \
           echo "$line" | grep "DMTBLEND" >/dev/null 2>&1 || \
           echo "$line" | grep "};" >/dev/null 2>&1; then
            break
        fi
        if echo "$line" | grep "{" | grep "PR_" >/dev/null 2>&1; then
            PREC=$(echo "$line" | sed 's/{//' | awk '{print $1}' | sed 's/,$//')
            [ -n "$PREC" ] && echo "$PREC" >> "$prec_temp"
        fi
    done

    # Read back and populate maps
    if [ -s "$prec_temp" ]; then
        DREC_MAP[$DREC]=$(wc -l < "$prec_temp")
        DREC_LIST+=("$DREC")
        PREC_MAP[$DREC]=$(tr '\n' ' ' < "$prec_temp" | sed 's/ $//')
    fi
    rm -f "$prec_temp"
  done
}

find_drecs() {
  local AID="$1"
  AFILE=$(cs -L -0 "$AID" 2>&1 | grep -i "arec" | grep -v "whence" | grep "_tbl" | awk '{print $1}' | head -1)
  if [ -z "$AFILE" ] || [ ! -f "$AFILE" ]; then
      return 1
  fi

  AENTRY=$(grep -n "$AID," "$AFILE" | head -1)
  [ -z "$AENTRY" ] && return

  ALINE=$(echo "$AENTRY" | cut -d: -f1)
  ASTRUCT=$(sed -n "${ALINE},/}/p" "$AFILE")
  GTABLE=$(echo "$ASTRUCT" | grep "gtbl_dr" | head -1 | awk '{print $1}' | sed 's/,$//')
  [ -z "$GTABLE" ] && return

  GFILE=$(cs -L -1 "$GTABLE" 2>&1 | grep -v "^Using" | grep -v "whence" | awk '{print $1}' | head -1)
  if [ -z "$GFILE" ] || [ ! -f "$GFILE" ]; then
    return
  fi

  TSTART=$(grep -n "^.*${GTABLE}\[\]" "$GFILE" | head -1 | cut -d: -f1)
  [ -z "$TSTART" ] && return

  DTABLE=$(sed -n "${TSTART},/^};/p" "$GFILE")
  DRECS=$(echo "$DTABLE" | sed -n '/{/,/DM_/p' | grep "DM_" | awk '{print $1}' | sed 's/,$//')
  find_precs "$DRECS"
}

process_parameter() {
  local PNAME="$1"
  local CTABLE="$3"
  local CFILE="$4"
  local DEPTH="$5"

  [ $DEPTH -gt 10 ] && return

  PVAR=$(grep -i "\"${PNAME}\"" "$STRINGS_FILE" | awk '{print $2}' | sed 's/\[\].*//' | head -1)
  [ -z "$PVAR" ] && return

  TSTART=$(grep -n "^C_WORDS ${CTABLE}\[\]" "$CFILE" | head -1 | cut -d: -f1)
  [ -z "$TSTART" ] && return

  ENTRY=$(sed -n "${TSTART},/^};/p" "$CFILE" | grep -n "$PVAR" | cut -d: -f1 | head -1)
  [ -z "$ENTRY" ] && return

  ETEXT=$(sed -n "${TSTART},/^};/p" "$CFILE" | sed -n "$((ENTRY-1)),$((ENTRY+2))p")

  if echo "$ETEXT" | grep "CC_OBJECT" >/dev/null 2>&1 || echo "$ETEXT" | grep "CC_ACTION" >/dev/null 2>&1; then
    AREC_ID=$(echo "$ETEXT" | sed -n '3p' | awk '{print $1}' | sed 's/,$//')
    find_drecs "$AREC_ID"
    return 0
  fi

  if echo "$ETEXT" | grep "IOBJECT" >/dev/null 2>&1 || echo "$ETEXT" | grep "IACTION" >/dev/null 2>&1; then
    NTABLE=$(echo "$ETEXT" | sed -n 's/.*OBJ_NODE(\([^)]*\)).*/\1/p')
    if [ -n "$NTABLE" ] && [ "$NTABLE" != "NULL" ]; then
      NFILE=$(cs -L -1 "$NTABLE" 2>&1 | grep -v "^Using" | grep -v "whence" | awk '{print $1}' | head -1)
      if [ -n "$NFILE" ] && [ -f "$NFILE" ]; then
        process_parameter "$PNAME" "" "$NTABLE" "$NFILE" $((DEPTH+1))
      fi
    fi
  fi
}

# ============================================================================
# MAIN
# ============================================================================

main() {
    [ -z "$1" ] && echo "âŒ Usage: $0 <action> [object] [--full|--json|--graphviz|--all]" && return 1

    # Parse output format from args
    for arg in "$@"; do
        case "$arg" in
            --full) OUTPUT_FORMAT="tree_full" ;;
            --json) OUTPUT_FORMAT="json" ;;
            --graphviz) OUTPUT_FORMAT="graphviz" ;;
            --all) OUTPUT_FORMAT="all" ;;
            --table) OUTPUT_FORMAT="table" ;;
        esac
    done

    ACTION_NAME="$1"
    OBJECT_NAME="${2:-station}"

    local AVAR="S_${ACTION_NAME}"
    local SFILE=$(cs -L -0 "$AVAR" 2>&1 | grep action | grep -v "whence" | awk '{print $1}' | head -1)
    if [ -z "$SFILE" ] || [ ! -f "$SFILE" ]; then
        echo "âŒ Starting action variable '$AVAR' not found"
        return 1
    fi

    local TNAME=$(sed -n "/$AVAR/{p;N;N;N;p;}" "$SFILE" | grep "CC_ACTION" | sed -n 's/.*OBJ_NODE(\([^)]*\)).*/\1/p' | head -1)
    [ -z "$TNAME" ] && echo "âŒ No starting table" && return 1

    local TFILE=$(cs -L -1 "$TNAME" 2>&1 | grep -v "^Using" | grep -v "whence" | awk '{print $1}' | head -1)
    if [ -z "$TFILE" ] || [ ! -f "$TFILE" ]; then
        echo "âŒ File for table '$TNAME' not found"
        return 1
    fi

    AREC_NAME="$TNAME"
    process_parameter "${OBJECT_NAME}" "" "$TNAME" "$TFILE" 1

    # Output in selected format(s)
    case "$OUTPUT_FORMAT" in
        tree)
            output_tree_header
            output_tree_body
            output_table
            ;;
        tree_full)
            output_tree_full
            ;;
        table)
            output_table
            ;;
        json)
            output_json
            ;;
        graphviz)
            output_graphviz
            ;;
        all)
            output_tree_header
            output_tree_body
            output_table
            echo ""
            output_json
            echo ""
            output_graphviz
            ;;
    esac
}

main "$@"
