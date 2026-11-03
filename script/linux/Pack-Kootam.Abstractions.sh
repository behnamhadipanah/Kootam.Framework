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
PROJECT="$REPO_ROOT/Extensions/Kootam.Abstractions"
SOLUTION="Kootam.Abstractions.sln"
OUTPUT_DIR="$REPO_ROOT/nugets"

step "Checking output directory..."

if [[ ! -d "$OUTPUT_DIR" ]]; then
    mkdir -p "$OUTPUT_DIR"
    echo "Output directory created."
else
    echo "Output directory already exists."
fi

step "Changing directory to project..."
cd "$PROJECT"

step "Switching to main branch..."
git checkout Main

step "Pulling latest changes from remote..."
if ! git pull origin Main; then
    echo "WARNING: git pull failed. Continuing with local code." >&2
fi

if [[ ! -f "$SOLUTION" ]]; then
    echo "Solution not found: $PROJECT/$SOLUTION" >&2
    exit 1
fi

echo "Solution : $SOLUTION"

step "Cleaning solution..."
dotnet clean "$SOLUTION" -c Release

step "Restoring NuGet packages..."
dotnet restore "$SOLUTION"

step "Building solution (Release)..."
dotnet build "$SOLUTION" -c Release

step "Creating NuGet packages..."
echo "Output directory: $OUTPUT_DIR"
dotnet pack "$SOLUTION" -c Release -o "$OUTPUT_DIR"

step "Completed successfully."
echo "NuGet packages created in:"
echo "$OUTPUT_DIR"
