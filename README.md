# OpenWorldRealisticMobileRacer_SUPERWORLD (Unity Android Prototype)

This repository provides a complete **project blueprint + production-ready C# gameplay systems** for a mobile-oriented open-world racing prototype with a handcrafted loop map and high-feel driving.

## Implemented Systems

- Drift-capable WheelCollider vehicle controller.
- Dynamic layered RPM engine audio (idle / low / mid / high).
- Tire skid + gear shift dip support.
- Tunnel reverb snapshot trigger zones.
- Mobile touch input (steering wheel + hold buttons).
- Third-person chase camera with speed FOV and drift shake.
- HUD (speed + RPM + pause).
- World bootstrap for fog/sky/shadow/perf defaults.
- Scenic ambient audio zones.
- Adaptive runtime quality scaler (render scale + shadows) for stable premium visuals.

## Folder Layout

```text
Assets/
 ├── Scripts/
 │    ├── Car/
 │    ├── Input/
 │    ├── Audio/
 │    ├── Camera/
 │    ├── UI/
 │    └── World/
 ├── Materials/
 ├── Models/
 ├── Textures/
 ├── Scenes/
 ├── Lighting/
 └── Audio/
```

---

## Unity Setup (Android + URP Mobile)

### 1) Create Project
1. Unity Hub → New Project → **URP (3D)**.
2. Name it `OpenWorldRealisticMobileRacer_SUPERWORLD`.
3. Replace generated `Assets/` with this repository `Assets/` folders or copy scripts in.

### 2) Android Build Settings
1. File → Build Settings → Android → Switch Platform.
2. Player Settings:
   - Color Space: **Linear**
   - Scripting Backend: **IL2CPP**
   - API Compatibility: **.NET Standard 2.1**
   - Target Architectures: **ARM64** (add ARMv7 only if needed)
   - Internet Access: Auto
3. Quality:
   - Default quality for Android: Medium/Custom Mobile
   - Disable MSAA or keep at 2x max.

### 3) URP Renderer Mobile Profile
1. Use URP Forward renderer.
2. Main directional light = real-time sun.
3. Additional lights per object: 1–2 max.
4. Shadows:
   - Shadow distance: 60–90
   - Cascades: 1–2
   - Resolution: Medium
5. Post-processing (Volume):
   - Color Adjustments
   - Bloom (low intensity)
   - Vignette (subtle)
   - Optional mild tonemapping

### 4) Global Optimization Flags
- Enable SRP Batcher.
- Static batching on static world objects.
- GPU instancing on repeated props.
- Occlusion Culling bake for city blocks/mountains.
- Use LODGroups on buildings/trees/rocks.
- Reflection probes set to baked/static.


### 5) Push Highest Visual Limits (Then Scale Intelligently)
- Add `WorldBootstrap` in startup scene and keep high defaults for:
  - Shadow distance 120–160
  - VeryHigh shadows
  - 4x MSAA
  - LOD Bias 1.8–2.2
  - Forced anisotropic filtering
- Add `UltraGraphicsConfigurator` to auto-pick Ultra/High/Balanced tiers by device memory.
- Keep Ultra only for devices with strong memory/GPU budget.

### 6) Mid → Premium Processor Graphics Tiers (Max Visual Push)
Attach `UltraGraphicsConfigurator` in your bootstrap scene and assign URP asset if needed.

Tier behavior:
- **Mid tier (6GB RAM+)**: High visuals with balanced shadows, 45 FPS target, 2x MSAA, reduced render scale.
- **High tier (8GB RAM + 3GB GPU mem)**: Very high visuals, 60 FPS target, 4x MSAA, full-res textures.
- **Premium tier (12GB RAM + 6GB GPU mem)**: Max mobile visuals, longer shadows, upscaled render scale, 90 FPS target.

This is tuned for **mid to premium processors** to push visuals while retaining stable frame pacing.

### 7) Cinematic Depth Stack (Mobile-Safe)
- Add a Global Volume and attach `MobilePostProcessingConfigurator`.
- It auto-detects Mid/High/Premium devices and tunes:
  - ACES tonemapping
  - Color adjustments + white balance
  - Bloom + vignette
  - Motion blur (high tiers)
  - Depth of field (high tiers)
  - Subtle chromatic aberration + film grain
- Goal: PC-like depth and speed sensation on mobile without brute-force rendering cost.

---

## World Construction Plan (Single Continuous Loop)

Create one scenic macro-loop that always changes mood:

1. **City/Town Hub (Start/Return)**
   - Medium-density blocks, intersections, parked cars, signage.
   - Wet-looking road material + reflection probes.
   - Subtle emissive accents (not cyberpunk).

2. **Countryside Sweep**
   - Open-radius curves, farmland decals, low fencing.
   - Sparse villages and tree clusters.
   - Warm daylight ambience.

3. **Mountain Climb**
   - Strong elevation gain.
   - Cliff turns + guardrails.
   - Fog increases by altitude (local volume).

4. **Tunnel Cut-through**
   - Dark interior, baked warm lights.
   - Use `TunnelAudioZone` to blend to reverb snapshot.
   - Brightness adaptation on exit.

5. **High Bridge Segment**
   - Over valley/river, visible vertical drop.
   - Crosswind ambience zone.

6. **Waterfall Valley Scenic Road**
   - Waterfall plane with scrolling normal + alpha foam mask.
   - Mist particle sheets (lightweight).
   - Dramatic but mobile-safe lighting contrast.

7. **Return Route to City**
   - Fast technical descent with mixed corner cadence.
   - Rejoin town through industrial outskirts.

### Distant Mountain Illusion
- Use skybox + unreachable low-poly mountain ring mesh.
- Keep collider-free, no AI pathing, no gameplay there.

---

## Car Setup in Scene

1. Add player car root with Rigidbody:
   - Mass 1200–1450
   - Interpolate: Interpolate
   - Collision Detection: Continuous Dynamic
2. Add 4 WheelColliders and 4 matching wheel meshes.
3. Add `CarController` and assign wheels + visuals + input provider.
4. Add `CarSetupValidator` and assign same wheel colliders for baseline friction.
5. Tag player root as `Player`.

---

## Input + UI Setup

1. Canvas (Screen Space Overlay).
2. Add steering wheel UI image and child foreground image.
3. Add `VirtualSteeringWheel` script to wheel object.
4. Add three large hold buttons (Throttle, Brake, Handbrake).
5. Add `HoldButtonInput` to each and set type.
6. Add `TouchInputProvider` to a GameObject (`InputRoot`) and wire all references.

---


## Driving Feel Polish (Recommended Defaults)

The car stack now includes additional feel systems:
- Traction control modulation at high slip.
- Stability assist to reduce snap-spin at speed.
- Speed-based aerodynamic downforce for planted high-speed turns.
- Torque-by-speed curve for better pull out of corners and less wheelspin spikes.

HUD polish additions:
- Smoothed speed/RPM readouts (less jitter).
- Gear indicator text support.
- Drift intensity bar (bind to `Image` fill).

Input polish additions:
- Optional swipe steering mode for phones where virtual wheel feels heavy.
- Safe `EventSystem` handling for editor fallback controls.

---

## Camera Setup

1. Main Camera → add `ChaseCameraController`.
2. Assign target = player transform.
3. Assign car reference = `CarController`.
4. Tune offset and damping for heavier/lighter feel.

---

## Audio Setup

1. Add mixer with snapshots: `Outside`, `Tunnel`.
2. Add 4 engine loop clips to separate AudioSources on car:
   - Idle
   - Low
   - Mid
   - High
3. Add optional skid and shift AudioSources.
4. Add `EngineAudioController` and assign sources + car + input.
5. Add trigger colliders inside tunnel and attach `TunnelAudioZone`.
6. Add environmental loop sources in zones with `ScenicZoneAudio`.

---

## HUD Setup

1. Add TextMeshPro labels for speed and RPM.
2. Add pause menu panel.
3. Add `HUDController` on UI root and assign refs.
4. Hook pause button to `HUDController.TogglePause()`.

---

## Performance Tuning Checklist (30–60 FPS)

If FPS drops:
1. Reduce shadow distance first.
2. Lower LOD transition distances.
3. Lower texture max size on terrain props.
4. Disable optional bloom.
5. Reduce far traffic/prop counts.
6. Reduce reflection probe count and resolution.

---



## Engine Scope Note

This repository is currently a Unity URP project. Replacing it with Unreal Engine would require a full engine migration (project files, assets, build pipeline, gameplay code rewrite).
To keep this codebase working now, visual quality upgrades are implemented in Unity with mobile-focused cinematic post and adaptive quality systems.

---

## GitHub Actions (Single Workflow Only)

This repository now uses **one** GitHub Action workflow: `.github/workflows/unity-android-build.yml`.

Behavior:
- Triggers on every push.
- Runs one job that compiles a **full Android APK**.
- Verifies at least one `.apk` file exists after build.
- Uploads one artifact: `android-apk`.
- Fails fast if Unity secrets are missing and only succeeds when a real APK is built.

Required repository secrets:
- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

---

## Script Wiring Quick Reference

- `TouchInputProvider`: central input state + smoothing + speed-sensitive steering.
- `CarController`: WheelCollider movement, drift behavior, traction recovery, RPM telemetry.
- `EngineAudioController`: RPM/throttle-based blending + skid + shift dip.
- `TunnelAudioZone`: audio mixer snapshot transitions in/out tunnel.
- `ChaseCameraController`: chase cam smoothing + FOV + drift shake.
- `HUDController`: speed/RPM/pause updates.
- `WorldBootstrap`: fog, skybox, high-end graphics defaults (shadows/AA/LOD/anisotropy) + target framerate policy.
- `UltraGraphicsConfigurator`: pushes high-end URP visuals for mid/high/premium devices with tiered frame targets.
- `AdaptiveQualityRuntime`: dynamic render-scale/shadow tuning to keep frame time stable without hard visual drops.
- `MobilePostProcessingConfigurator`: tiered cinematic post-processing (motion blur, DoF, tonemapping, bloom) for mid/high/premium devices.
- `ScenicZoneAudio`: fade-in/out ambience by trigger zone.

This setup is intentionally tuned for **mobile realism through illusion and feel**, not brute-force simulation.
