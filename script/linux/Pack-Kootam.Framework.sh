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
    mkdir -p "$OUTPUT_DIR"
fi

cd "$REPO_ROOT"

step "Switching to main branch..."
git checkout Main

step "Pulling latest changes..."
git pull origin Main

step "Finding solutions..."

mapfile -t solutions < <(
    find "$REPO_ROOT" -type f \( -name '*.sln' -o -name '*.slnx' \) \
        ! -path "$REPO_ROOT/Extensions/*" \
        | sort
)

for solution in "${solutions[@]}"; do
    name="$(basename "$solution")"
    step "Processing solution: $name"

    echo "Cleaning..."
    if ! dotnet clean "$solution" -c Release; then
        echo "WARNING: Clean failed. Skipping..." >&2
        continue
    fi

    echo "Restoring packages..."
    if ! dotnet restore "$solution" --configfile "$TEMP_NUGET_CONFIG"; then
        echo "WARNING: Restore failed. Skipping..." >&2
        continue
    fi

    echo "Building..."
    if ! dotnet build "$solution" -c Release --no-restore; then
        echo "WARNING: Build failed. Skipping..." >&2
        continue
    fi

    echo "Packing..."
    if ! dotnet pack "$solution" -c Release --no-build -o "$OUTPUT_DIR" --configfile "$TEMP_NUGET_CONFIG"; then
        echo "WARNING: Pack failed. Skipping..." >&2
        continue
    fi

    echo "$name completed successfully."
done

step "Finished."
echo "Packages saved to:"
echo "$OUTPUT_DIR"
