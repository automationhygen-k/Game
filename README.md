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


## GitHub Actions (Auto-Compile on Push)

This repository includes `.github/workflows/unity-android-build.yml` to automatically build Android when you push to GitHub.

Required repository secrets:
- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

Workflow behavior:
- Triggers on every push and pull request.
- Builds Android App Bundle (`.aab`) using `game-ci/unity-builder`.
- Uploads build output as `android-build` artifact.

---

## Script Wiring Quick Reference

- `TouchInputProvider`: central input state + smoothing + speed-sensitive steering.
- `CarController`: WheelCollider movement, drift behavior, traction recovery, RPM telemetry.
- `EngineAudioController`: RPM/throttle-based blending + skid + shift dip.
- `TunnelAudioZone`: audio mixer snapshot transitions in/out tunnel.
- `ChaseCameraController`: chase cam smoothing + FOV + drift shake.
- `HUDController`: speed/RPM/pause updates.
- `WorldBootstrap`: fog, skybox, high-end graphics defaults (shadows/AA/LOD/anisotropy) + target framerate policy.
- `UltraGraphicsConfigurator`: pushes Ultra visuals on high-end phones and auto-falls back to High/Balanced on weaker devices.
- `ScenicZoneAudio`: fade-in/out ambience by trigger zone.

This setup is intentionally tuned for **mobile realism through illusion and feel**, not brute-force simulation.
