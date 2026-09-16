Include ..\AGENTS.md

# Highlight Tweaks — Mod-Specific Agent Instructions

## Identity
- **Assembly:** `highlighttweaks`
- **Namespace:** `Calloatti.HighlightTweaks`
- **Framework:** Harmony, Bindito DI
- **ModId:** `Calloatti.HighlightTweaks`
- **Min Game Version:** 1.1.2.4 — uses `timberborn-decompiled-1.1.*`

## What This Mod Does
Replaces the default beaver selection highlight with a golden vertical beam and ground circle, similar to Civilization V's selected unit indicator. Follows the beaver smoothly using `CharacterModel.Position`.

## Source Architecture (`Version-1.1/Source/`)

| File | Role |
|---|---|
| `ModStarter.cs` | Entry point — `IModStarter`, applies Harmony patches |
| `HighlightConfigurator.cs` | Bindito DI — registers `GoldenHighlightSpawner` as singleton in Game context |
| `GoldenHighlightSpawner.cs` | Core — EventBus listener, creates/destroys golden beam + ring LineRenderers |
| `GoldenHighlightEffect.cs` | MonoBehaviour — follows beaver via `CharacterModel.Position` in `LateUpdate` |
| `HighlightPatch.cs` | Harmony prefix — suppresses vanilla `HighlightableObject.HighlightPrimary` for Characters |

## Key Implementation Details

- **Smooth tracking:** `CharacterModel.Position` (interpolated via `_animatedPathFollower`) — NOT `transform.position` (snaps between nav-mesh waypoints)
- **EventBus registration:** Must happen in `PostLoad()`, not `Load()` — Bindito does not dispatch events to objects registered before `PostLoad`
- **Rendering:** `LineRenderer` with `Sprites/Default` shader, `_ZTest=Always`, `renderQueue=4000`, `sortingOrder=32767`
- **Gradients:** Cached as `static readonly` — no per-spawn allocations
- **Y-offset:** Beam starts at local Y=0.8 (above beaver head), ring at Y=0.01 (ground level)
- **Cleanup:** `IUnloadableSingleton.Unload()` destroys material, texture, and effect GameObject

## Assemblies Referenced

- `Timberborn.SelectionSystem` — publicized (for `SelectableObjectSelectedEvent`, `HighlightableObject`)
- `Timberborn.Characters` — publicized (for `Character` component)
- `Timberborn.CharacterModelSystem` — used via public API (`CharacterModel.Position`)

## Build & Deploy

```powershell
dotnet build    # Builds + auto-deploys via pre/post-build scripts
```

**Deploy target:** `%USERPROFILE%\Documents\Timberborn\Mods\Highlight Tweaks\Version-1.1\`

## Debugging
- Check `C:\Users\calloatti\AppData\LocalLow\Mechanistry\Timberborn\Player.log` for `[Highlight Tweaks]` prefix logs

## Lessons Learned
- `useWorldSpace = false` with local-space positions and parent transform following is the correct approach for moving highlights — `useWorldSpace = true` leaves positions at spawn location and they don't follow the parent
- `LineRenderer` works fine in Timberborn's URP with `Sprites/Default` shader — the beam and ring both render
- Horizontal rings (XZ plane) at Y=0 may be invisible if parent Y=0 while terrain is elevated — always use `CharacterModel.Position.y` as the base Y
- EventBus registration in `Load()` silently fails to receive events — must register in `PostLoad()`

## Hard Rule
DO NOT EVER TOUCH THE DEPLOY FOLDER.

BUILD DOES EVERYTHING, NEVER EVER MESS WITH THE DEPLOY PROCESS.
