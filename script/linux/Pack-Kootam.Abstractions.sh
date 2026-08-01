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
git pull origin Main

step "Finding project file..."
csproj="$(find . -maxdepth 1 -name '*.csproj' | head -n 1)"

if [[ -z "$csproj" ]]; then
    echo "No .csproj found in $PROJECT" >&2
    exit 1
fi

echo "Project : $(basename "$csproj")"

step "Cleaning project..."
dotnet clean "$csproj" -c Release

step "Restoring NuGet packages..."
dotnet restore "$csproj"

step "Building project (Release)..."
dotnet build "$csproj" -c Release

step "Creating NuGet package..."
echo "Output directory: $OUTPUT_DIR"
dotnet pack "$csproj" -c Release -o "$OUTPUT_DIR"

step "Completed successfully."
echo "NuGet package created in:"
echo "$OUTPUT_DIR"
