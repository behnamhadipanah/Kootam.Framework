#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

is_excluded() {
    local full_name="$1"
    shift
    local exclude_path
    for exclude_path in "$@"; do
        if [[ "$full_name" == "$exclude_path" || "$full_name" == "$exclude_path"/* ]]; then
            return 0
        fi
    done
    return 1
}

process_step() {
    local path="$1"
    shift
    local excludes=("$@")

    echo
    echo "=================================================="
    echo "Processing: $path"
    echo "=================================================="

    mapfile -t projects < <(find "$path" -type f -name '*.csproj' | sort)

    local filtered=()
    local project
    for project in "${projects[@]}"; do
        if ! is_excluded "$project" "${excludes[@]}"; then
            filtered+=("$project")
        fi
    done

    if [[ ${#filtered[@]} -eq 0 ]]; then
        echo "No projects found."
        return 0
    fi

    for project in "${filtered[@]}"; do
        echo
        echo "------------------------------------------"
        echo "Project: $project"
        echo "------------------------------------------"

        echo "Updating packages..."
        if ! dotnet package update --project "$project"; then
            echo "Package update returned a non-zero exit code"
        fi

        echo "Restoring..."
        if ! dotnet restore "$project"; then
            echo "Restore failed."
            continue
        fi

        echo "Building..."
        if dotnet build "$project" --no-restore; then
            echo "Build succeeded."
        else
            echo "Build failed."
        fi
    done
}

process_step \
    "$REPO_ROOT/Extensions/Kootam.Abstractions"

process_step \
    "$REPO_ROOT/Extensions/Authentication/Kootam.Authentication"

process_step \
    "$REPO_ROOT/Extensions" \
    "$REPO_ROOT/Extensions/Kootam.Abstractions" \
    "$REPO_ROOT/Extensions/Authentication/Kootam.Authentication"

process_step \
    "$REPO_ROOT" \
    "$REPO_ROOT/Extensions"

echo
echo "Done."
