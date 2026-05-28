#!/usr/bin/env bash
set -euo pipefail

PORT=5149
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
API_PROJECT="$REPO_ROOT/src/ShopNest.API/ShopNest.API.csproj"

require_cmd() {
    if ! command -v "$1" >/dev/null 2>&1; then
        echo "Error: required command '$1' was not found." >&2
        exit 1
    fi
}

listener_pids() {
    lsof -tiTCP:"$PORT" -sTCP:LISTEN 2>/dev/null || true
}

is_shopnest_api_process() {
    local pid="$1"
    local cmd
    cmd="$(ps -p "$pid" -o command= 2>/dev/null || true)"
    [[ "$cmd" == *"ShopNest.API"* ]]
}

stop_existing_shopnest_api() {
    local pids=("$@")
    local pid

    for pid in "${pids[@]}"; do
        echo "Stopping existing ShopNest.API process (PID $pid)..."
        kill -TERM "$pid" 2>/dev/null || true
    done

    local attempt
    for attempt in {1..10}; do
        sleep 1
        local remaining=()
        for pid in "${pids[@]}"; do
            if kill -0 "$pid" 2>/dev/null; then
                remaining+=("$pid")
            fi
        done

        if [[ ${#remaining[@]} -eq 0 ]]; then
            return 0
        fi
    done

    local survivors=()
    for pid in "${pids[@]}"; do
        if kill -0 "$pid" 2>/dev/null; then
            survivors+=("$pid")
        fi
    done

    if [[ ${#survivors[@]} -gt 0 ]]; then
        for pid in "${survivors[@]}"; do
            echo "Force-stopping stubborn ShopNest.API process (PID $pid)..."
            kill -KILL "$pid" 2>/dev/null || true
        done
    fi
}

require_cmd lsof
require_cmd dotnet
require_cmd ps
require_cmd kill

if [[ ! -f "$API_PROJECT" ]]; then
    echo "Error: expected project file was not found at '$API_PROJECT'." >&2
    exit 1
fi

pids=()
while IFS= read -r pid; do
    [[ -n "$pid" ]] && pids+=("$pid")
done < <(listener_pids)

if [[ ${#pids[@]} -gt 0 ]]; then
    non_shopnest=()
    for pid in "${pids[@]}"; do
        if ! is_shopnest_api_process "$pid"; then
            non_shopnest+=("$pid")
        fi
    done

    if [[ ${#non_shopnest[@]} -gt 0 ]]; then
        echo "Error: port $PORT is in use by a non-ShopNest.API process." >&2
        echo "No process was terminated." >&2
        echo >&2
        echo "Inspect port owner:" >&2
        echo "  lsof -nP -iTCP:$PORT -sTCP:LISTEN" >&2
        echo >&2
        echo "If safe, stop that process manually, then run this script again." >&2
        exit 1
    fi

    stop_existing_shopnest_api "${pids[@]}"
fi

if lsof -nP -iTCP:"$PORT" -sTCP:LISTEN >/dev/null 2>&1; then
    echo "Error: port $PORT is still occupied after cleanup." >&2
    echo "Inspect port owner with: lsof -nP -iTCP:$PORT -sTCP:LISTEN" >&2
    exit 1
fi

echo "Starting ShopNest.API on http://localhost:$PORT (launch profile: http)..."
exec dotnet run --project "$API_PROJECT" --launch-profile http
