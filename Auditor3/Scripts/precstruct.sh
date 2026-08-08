drccd [44022]-> cat precstruct
#!/bin/bash

# ============================================================================
# precstruct
#
# Displays:
#   1. The C structure for a PREC, located through cscope
#   2. The compiled GDB layout stored in a local .ptype file
#
# Examples:
#   ./precstruct pr_ext
#   ./precstruct pr_mobd
#   ./precstruct PR_STN
#
# Layout files:
#   $HOME/precstruct_layouts/<release>/<prec>.ptype
#
# Example:
#   /home/mcnuttd/precstruct_layouts/cm10.2/pr_ext.ptype
# ============================================================================

PREC="${1:-}"

if [ -z "$PREC" ]; then
    echo "Usage: $0 <prec_name>"
    echo ""
    echo "Examples:"
    echo "  $0 pr_ext"
    echo "  $0 pr_mobd"
    echo "  $0 pr_stn"
    exit 1
fi

echo "?? Deriving file paths dynamically..."

# Normalize the requested name.
#
# PR_EXT        -> pr_ext
# Pr_Ext        -> pr_ext
# struct pr_ext -> pr_ext
PREC=$(printf '%s' "$PREC" | tr '[:upper:]' '[:lower:]')
PREC="${PREC#struct }"

# ============================================================================
# Locate strings.ct through cscope
# ============================================================================

STRINGS_RESULT=$(
    cs -L -7 strings.ct 2>&1 |
    grep "strings.ct" |
    grep -v "tui" |
    grep -v "whence" |
    grep -v "Using" |
    head -1
)

if [ -z "$STRINGS_RESULT" ]; then
    echo "? FATAL: Could not find strings.ct through cscope."
    exit 1
fi

# cscope may return:
#
# /path/to/strings.ct <unknown> 1 <unknown>
#
# The path is the first whitespace-separated field.
set -- $STRINGS_RESULT
STRINGS_FILE="$1"

if [ -z "$STRINGS_FILE" ] || [ ! -f "$STRINGS_FILE" ]; then
    echo "? FATAL: strings.ct file does not exist:"
    echo "  $STRINGS_FILE"
    exit 1
fi

echo "   Found STRINGS_FILE: $STRINGS_FILE"

# ============================================================================
# Derive release name
#
# Example:
# /usr/add-on/field_base4/cm10.2/SP/.../strings.ct
#
# becomes:
# cm10.2
# ============================================================================

RELEASE_NAME=$(
    printf '%s\n' "$STRINGS_FILE" |
    sed -n 's#^.*/field_base4/\([^/][^/]*\)/.*#\1#p'
)

if [ -z "$RELEASE_NAME" ]; then
    echo "? FATAL: Could not derive release name from:"
    echo "  $STRINGS_FILE"
    exit 1
fi

echo "   Found release: $RELEASE_NAME"

# ============================================================================
# Locate the user-writable prototype layout file
# ============================================================================

LAYOUT_ROOT="${PRECSTRUCT_LAYOUT_ROOT:-$HOME/precstruct_layouts}"
LAYOUT_DIR="${LAYOUT_ROOT}/${RELEASE_NAME}"
LAYOUT_FILE="${LAYOUT_DIR}/${PREC}.ptype"

echo "   Layout file: $LAYOUT_FILE"

# ============================================================================
# Locate the C structure through cscope
# ============================================================================
#
# Expected result:
#
# /path/to/dpm_prec.h <global> 1525 struct pr_ext
#
# Basic grep is used because this DRCCD system does not support grep -E.
# ============================================================================

echo ""
echo "?? Looking up structure: struct $PREC"

CSCOPE_RESULT=$(
    cs -L -0 "$PREC" 2>&1 |
    grep " struct ${PREC}$" |
    head -1
)

if [ -z "$CSCOPE_RESULT" ]; then
    echo "? FATAL: struct $PREC was not found in cscope."
    exit 1
fi

# Extract fields from the cscope result.
#
# Example:
#   $1 = header file
#   $3 = source line
#
# The source paths in this environment do not contain spaces.
set -- $CSCOPE_RESULT

STRUCT_FILE="$1"
STRUCT_LINE="$3"

if [ -z "$STRUCT_FILE" ] || [ ! -f "$STRUCT_FILE" ]; then
    echo "? FATAL: Structure file does not exist:"
    echo "  $STRUCT_FILE"
    exit 1
fi

case "$STRUCT_LINE" in
    ''|*[!0-9]*)
        echo "? FATAL: Invalid structure line: $STRUCT_LINE"
        echo "$CSCOPE_RESULT"
        exit 1
        ;;
esac

echo "   Found structure file: $STRUCT_FILE"
echo "   Structure line: $STRUCT_LINE"

# ============================================================================
# Print the C structure
# ============================================================================
#
# This avoids awk because the DRCCD awk implementation is limited.
# It handles both:
#
#   struct example {
#       ...
#   };
#
# and:
#
#   struct example
#   {
#       ...
#   };
# ============================================================================

echo ""
echo "======================================================================"
echo "C STRUCTURE: struct $PREC"
echo "======================================================================"

sed -n "${STRUCT_LINE},\$p" "$STRUCT_FILE" |
while IFS= read -r LINE
do
    echo "$LINE"

    # Count opening braces.
    OPEN_COUNT=$(
        printf '%s\n' "$LINE" |
        tr -cd '{' |
        wc -c
    )

    # Count closing braces.
    CLOSE_COUNT=$(
        printf '%s\n' "$LINE" |
        tr -cd '}' |
        wc -c
    )

    # Remove whitespace from wc output.
    OPEN_COUNT=$(printf '%s\n' "$OPEN_COUNT" |
        sed 's/[[:space:]]//g')

    CLOSE_COUNT=$(printf '%s\n' "$CLOSE_COUNT" |
        sed 's/[[:space:]]//g')

    [ -z "$OPEN_COUNT" ] && OPEN_COUNT=0
    [ -z "$CLOSE_COUNT" ] && CLOSE_COUNT=0

    if [ "$OPEN_COUNT" -gt 0 ]; then
        STARTED=1
    fi

    if [ "$STARTED" = "1" ]; then
        DEPTH=$((DEPTH + OPEN_COUNT - CLOSE_COUNT))
    fi

    # Stop after the outer structure closes.
    if [ "$STARTED" = "1" ] && [ "$DEPTH" -le 0 ]; then
        exit 0
    fi
done

# ============================================================================
# Print the compiled GDB layout
# ============================================================================

echo ""
echo "======================================================================"
echo "COMPILED MEMORY LAYOUT"
echo "======================================================================"

if [ ! -f "$LAYOUT_FILE" ]; then
    echo "? No GDB layout file found:"
    echo "  $LAYOUT_FILE"
    echo ""
    echo "Expected directory:"
    echo "  $LAYOUT_DIR"
    echo ""
    echo "Create it with:"
    echo "  mkdir -p \"$LAYOUT_DIR\""
    echo ""
    echo "Then save the GDB output as:"
    echo "  $LAYOUT_FILE"
    exit 0
fi

echo "Layout file: $LAYOUT_FILE"
echo ""

cat "$LAYOUT_FILE"

# ============================================================================
# Summary
# ============================================================================

echo ""
echo "======================================================================"
echo "SUMMARY"
echo "======================================================================"
echo "PREC        : $PREC"
echo "STRUCTURE   : struct $PREC"
echo "HEADER      : $STRUCT_FILE"
echo "SOURCE LINE : $STRUCT_LINE"
echo "RELEASE     : $RELEASE_NAME"
echo "LAYOUT FILE : $LAYOUT_FILE"
