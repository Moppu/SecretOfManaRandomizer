#!/usr/bin/env bash
#
# Launch SoMRandomizer.exe under Mono on macOS or Linux.
#
# Usage:
#   ./run-mono.sh [path/to/SoMRandomizer.exe] [randomizer arguments...]
#
# With no randomizer arguments the Windows Forms UI opens, with the exe's
# folder as the working directory. With arguments, the headless command-line
# mode runs instead in the caller's directory (see README.md).
#
# If no executable path is given, the script looks next to itself, then in the
# Release and Debug build output folders.
#
# macOS workarounds, applied only on Darwin and only when not already set:
#   MONO_MWF_MAC_FORCE_X11=1
#     Mono defaults to its Carbon Windows Forms driver on macOS. That driver was
#     never ported to 64-bit and crashes on the first control
#     (EntryPointNotFoundException: HIViewPlaceInSuperviewAt). Forcing the X11
#     driver requires an X server such as XQuartz or the MacPorts xorg-server.
#   MONO_WINFORMS_XIM_STYLE=disabled
#     With the X input method enabled, Mono crashes when the main window takes
#     focus (ExecutionEngineException: String conversion error in
#     X11Keyboard.XGetIMValues). Disabling XIM avoids that call. Input method
#     composition, for example Japanese text entry, is unavailable as a result.
#   DISPLAY=:0
#     Used only if DISPLAY is unset, which is the default display for XQuartz.

set -euo pipefail

if ! command -v mono >/dev/null 2>&1; then
    echo "error: mono not found on PATH" >&2
    exit 127
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

exe=""
if [[ $# -gt 0 && "$1" == *.exe ]]; then
    exe="$1"
    shift
else
    for candidate in \
        "$script_dir/SoMRandomizer.exe" \
        "$script_dir/SoMRandomizer/bin/Release/net462/SoMRandomizer.exe" \
        "$script_dir/SoMRandomizer/bin/Debug/net462/SoMRandomizer.exe"; do
        if [[ -f "$candidate" ]]; then
            exe="$candidate"
            break
        fi
    done
fi

if [[ -z "$exe" || ! -f "$exe" ]]; then
    echo "error: SoMRandomizer.exe not found; pass its path as the first argument" >&2
    exit 1
fi

# Logs are written to, and the optional music pack is read from, the current
# directory. Double-clicking the exe on Windows makes that the exe's folder, so
# match it for the UI. Command-line mode keeps the caller's directory so that
# relative srcRom= and dstRom= paths still resolve.
exe="$(cd "$(dirname "$exe")" && pwd)/$(basename "$exe")"
if [[ $# -eq 0 ]]; then
    cd "$(dirname "$exe")"
fi

if [[ "$(uname -s)" == "Darwin" ]]; then
    export MONO_MWF_MAC_FORCE_X11="${MONO_MWF_MAC_FORCE_X11:-1}"
    export MONO_WINFORMS_XIM_STYLE="${MONO_WINFORMS_XIM_STYLE:-disabled}"
    export DISPLAY="${DISPLAY:-:0}"
fi

exec mono "$exe" "$@"
