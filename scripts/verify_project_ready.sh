#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

echo "[1/6] Checking Unity project structure..."
[[ -f ProjectSettings/ProjectVersion.txt ]]
[[ -f Packages/manifest.json ]]
[[ -f Packages/packages-lock.json ]]
[[ -d Assets/Scripts ]]

echo "[2/6] Checking Android CI workflow..."
[[ -f .github/workflows/unity-android-build.yml ]]
grep -q "name: Compile Android APK" .github/workflows/unity-android-build.yml
grep -q "androidExportType: androidPackage" .github/workflows/unity-android-build.yml

echo "[3/6] Checking key gameplay scripts..."
for f in \
  Assets/Scripts/Car/CarController.cs \
  Assets/Scripts/Input/TouchInputProvider.cs \
  Assets/Scripts/Audio/EngineAudioController.cs \
  Assets/Scripts/Camera/ChaseCameraController.cs \
  Assets/Scripts/UI/HUDController.cs \
  Assets/Scripts/World/WorldBootstrap.cs
  do
  [[ -f "$f" ]]
done

echo "[4/6] Checking Unity version pin..."
UNITY_VERSION=$(awk -F': ' '/m_EditorVersion:/ {print $2}' ProjectSettings/ProjectVersion.txt)
echo "Detected Unity version: $UNITY_VERSION"

echo "[5/6] Optional environment check (for CI secrets)..."
if [[ -n "${UNITY_LICENSE:-}" && -n "${UNITY_EMAIL:-}" && -n "${UNITY_PASSWORD:-}" ]]; then
  echo "Unity secrets are present in current environment."
else
  echo "Unity secrets not set in current shell (this is okay for local editor work)."
  echo "For GitHub Actions APK build, set UNITY_LICENSE / UNITY_EMAIL / UNITY_PASSWORD in repository secrets."
fi

echo "[6/6] Project readiness check complete ✅"
