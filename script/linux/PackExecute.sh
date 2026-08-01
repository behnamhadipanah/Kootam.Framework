#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

pause() {
    read -r -p "Press Enter to continue..."
}

run_script() {
    local script_name="$1"
    clear
    echo "Running $script_name..."
    echo
    bash "./$script_name"
    echo
    echo "Operation completed."
    pause
}

while true; do
    clear
    echo "============================================"
    echo "          Kootam NuGet Pack Manager"
    echo "============================================"
    echo
    echo "  1. Update nugets packages"
    echo "  2. Pack Kootam.Abstractions"
    echo "  3. Pack Extensions"
    echo "  4. Pack Kootam.Framework"
    echo "  5. Exit"
    echo
    read -r -p "Select an option [1-5]: " choice

    case "$choice" in
        1) run_script "update-nugets.sh" ;;
        2) run_script "Pack-Kootam.Abstractions.sh" ;;
        3) run_script "Pack-Extensions.sh" ;;
        4) run_script "Pack-Kootam.Framework.sh" ;;
        5)
            echo
            echo "Goodbye!"
            sleep 1
            exit 0
            ;;
        *)
            echo
            echo "Invalid option."
            pause
            ;;
    esac
done
