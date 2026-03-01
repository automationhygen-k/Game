# Final Install & Run Guide (Android)

This is the final, practical checklist to get the project running on a phone.

## 0) Plug-and-Play One-Command Options

### Local machine APK build (recommended)
```bash
./scripts/build_apk_local.sh
```

This uses a CLI build method (`CliAndroidBuilder`) and outputs:
- `build/local-apk/OpenWorldRealisticMobileRacer.apk`

If Unity is not auto-detected, set:
```bash
export UNITY_PATH="/path/to/Unity"
```

## 1) Install Requirements
- Unity Hub
- Unity Editor `2022.3.20f1` (Android Build Support module installed)
- Android SDK + NDK + OpenJDK (via Unity Hub modules)

## 2) Open Project
1. Clone/download this repository.
2. Open project root in Unity Hub (`/workspace/Game` equivalent on your machine).
3. Let Unity import packages from `Packages/manifest.json`.

## 3) Switch to Android
1. `File -> Build Settings -> Android -> Switch Platform`.
2. `Project Settings -> Player`:
   - Scripting Backend: `IL2CPP`
   - Target Architectures: `ARM64`
   - API Compatibility: `.NET Standard 2.1`

## 4) Scene Wiring (Minimum)
- Add car prefab/root with `CarController` and wheel colliders.
- Add `TouchInputProvider`, `HUDController`, `ChaseCameraController`.
- Add audio setup with `EngineAudioController` and snapshots.
- Add world systems (`WorldBootstrap`, `UltraGraphicsConfigurator`).

## 5) Local Build APK
1. `File -> Build Settings`.
2. Add your main scene(s) in build list.
3. Click `Build` or `Build And Run`.
4. Install generated APK on device.

## 6) CI Build APK (GitHub Actions)
Repository uses one workflow: `.github/workflows/unity-android-build.yml`.

Set these repository secrets:
- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

Push to GitHub. Workflow compiles APK and uploads `android-apk` artifact.
Workflow uses the same explicit CLI build method as local script for consistent output.

## 7) Quick Readiness Check Script
Run:

```bash
./scripts/verify_project_ready.sh
./scripts/build_apk_local.sh
```

It validates:
- project structure
- key scripts
- APK workflow config
- Unity version pin

