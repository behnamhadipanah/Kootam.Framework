#!/usr/bin/env bash
set -euo pipefail

step() {
    echo
    echo "=================================================="
    echo "$1"
    echo "=================================================="
}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
OUTPUT_DIR="$REPO_ROOT/nugets"
ROOT="$REPO_ROOT/Extensions"
EXCLUDE="$ROOT/Kootam.Abstractions"
TEMP_NUGET_CONFIG="$(mktemp /tmp/Kootam.NuGet.Config.XXXXXX)"

cleanup() {
    rm -f "$TEMP_NUGET_CONFIG"
}
trap cleanup EXIT

cat > "$TEMP_NUGET_CONFIG" <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$OUTPUT_DIR" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
EOF

if [[ ! -d "$OUTPUT_DIR" ]]; then
    step "Creating output directory..."
    mkdir -p "$OUTPUT_DIR"
fi

cd "$ROOT"

step "Switching to main branch..."
git checkout Main

step "Pulling latest changes from remote..."
git pull origin Main

step "Finding solutions..."

mapfile -t solutions < <(
    find "$ROOT" -type f \( -name '*.sln' -o -name '*.slnx' \) \
        ! -path "$EXCLUDE/*" \
        ! -path "$EXCLUDE" \
        ! -path '*/Sample/*' \
        ! -path '*.Sample*' \
        | sort
)

for solution in "${solutions[@]}"; do
    name="$(basename "$solution")"
    step "Processing solution: $name"

    echo "Restoring packages..."
    if ! dotnet restore "$solution" --configfile "$TEMP_NUGET_CONFIG"; then
        echo "WARNING: Restore failed. Skipping $name" >&2
        continue
    fi

    echo "Cleaning..."
    if ! dotnet clean "$solution" -c Release; then
        echo "WARNING: Clean failed. Skipping $name" >&2
        continue
    fi

    echo "Building..."
    if ! dotnet build "$solution" -c Release --no-restore; then
        echo "WARNING: Build failed. Skipping $name" >&2
        continue
    fi

    echo "Packing..."
    if ! dotnet pack "$solution" -c Release --no-build -o "$OUTPUT_DIR" --configfile "$TEMP_NUGET_CONFIG"; then
        echo "WARNING: Pack failed. Skipping $name" >&2
        continue
    fi

    echo "$name completed successfully."
done

step "All operations completed."
echo "NuGet packages saved to:"
echo "$OUTPUT_DIR"
