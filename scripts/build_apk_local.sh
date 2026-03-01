#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

UNITY_VERSION="2022.3.20f1"
BUILD_DIR="build/local-apk"
LOG_DIR="build/logs"
LOG_FILE="$LOG_DIR/unity-android-build.log"

mkdir -p "$BUILD_DIR" "$LOG_DIR"

find_unity() {
  if [[ -n "${UNITY_PATH:-}" && -x "${UNITY_PATH}" ]]; then
    echo "$UNITY_PATH"
    return
  fi

  local candidates=(
    "/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity"
    "$HOME/Unity/Hub/Editor/${UNITY_VERSION}/Editor/Unity"
    "/opt/unity/Editor/Unity"
    "$(command -v unity-editor 2>/dev/null || true)"
    "$(command -v Unity 2>/dev/null || true)"
  )

  for c in "${candidates[@]}"; do
    if [[ -n "$c" && -x "$c" ]]; then
      echo "$c"
      return
    fi
  done

  echo ""
}

UNITY_BIN="$(find_unity)"
if [[ -z "$UNITY_BIN" ]]; then
  echo "Unity executable not found."
  echo "Set UNITY_PATH to your Unity binary, e.g.:"
  echo "  export UNITY_PATH=\"/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity\""
  exit 1
fi

if [[ ! -f ProjectSettings/ProjectVersion.txt || ! -f Packages/manifest.json ]]; then
  echo "Project files missing. Run from repository root."
  exit 1
fi

echo "Using Unity: $UNITY_BIN"
echo "Building Android APK..."

"$UNITY_BIN" \
  -batchmode \
  -quit \
  -nographics \
  -projectPath "$ROOT_DIR" \
  -buildTarget Android \
  -executeMethod OpenWorldRealisticMobileRacer.EditorTools.CliAndroidBuilder.BuildAndroidApk \
  -customBuildPath "$BUILD_DIR/OpenWorldRealisticMobileRacer.apk" \
  -logFile "$LOG_FILE" || {
    echo "Unity build failed. Check log: $LOG_FILE"
    exit 1
  }

if [[ ! -f "$BUILD_DIR/OpenWorldRealisticMobileRacer.apk" ]]; then
  echo "Build finished without APK output. Check log: $LOG_FILE"
  exit 1
fi

echo "✅ APK built: $BUILD_DIR/OpenWorldRealisticMobileRacer.apk"
echo "Build log: $LOG_FILE"
