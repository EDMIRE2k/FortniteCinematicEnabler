# Fortnite Cinematic Settings Tool

A simple Windows utility by **EDMIRE from BiomeForge** that modifies Fortnite’s `GameUserSettings.ini` to enable hidden cinematic graphics settings.

The tool automatically finds the Fortnite settings file, applies your chosen preset, creates a backup, and can optionally set the file to read-only so Fortnite does not reset the values.

## Features

- Automatically locates `GameUserSettings.ini` using `%LOCALAPPDATA%`
- Clean fixed-size Windows UI
- Preset selector
- Automatic backup before applying changes
- One-click undo from the latest backup
- Optional read-only protection
- Manual file picker for unusual config locations
- Warning if Fortnite appears to be running

## Presets

### Default

Recommended for normal matches.

This preset uses the safer cinematic values that improve lighting, reflections, shadows, foliage, shading, and landscape quality while avoiding known visual bugs during normal gameplay.

### Photography Mode

Intended for screenshots only.

Photography Mode maxes every numbered quality value to `5`.

This can cause:

- Severe performance drops
- Severe graphical glitches
- Visual bugs when other players wear specific outfits

Use this preset only when taking screenshots or cinematic captures.

## Important GPU Warning

These settings have **immense GPU requirements**.

DLSS, TSR, XeSS, or another upscaling method is strongly recommended unless your system has at least an **RTX 4090** or **RTX 5090**.

Lower-end GPUs may experience major frame rate drops, stutters, or instability.

## How To Use

1. Close Fortnite.
2. Run `FortniteCinematicSettings.exe`.
3. Confirm that the app found your `GameUserSettings.ini`.
4. Choose a preset:
   - `Default` for normal gameplay
   - `Photography mode` for screenshots only
5. Leave **Set GameUserSettings.ini to read-only after applying** enabled if you want to prevent Fortnite from resetting the settings.
6. Click **Apply preset**.
7. Launch Fortnite.

## Undoing Changes

Click **Undo from backup** inside the app.

The tool creates a timestamped backup before applying changes. Backups are stored next to the original `GameUserSettings.ini`.

## Default Preset Values

```ini
bUseNanite=True
DesiredGlobalIlluminationQuality=5
DesiredReflectionQuality=5
PreNaniteGlobalIlluminationQuality=5
PreNaniteReflectionQuality=5
bRayTracing=True

sg.ViewDistanceQuality=3
sg.AntiAliasingQuality=3
sg.ShadowQuality=5
sg.GlobalIlluminationQuality=5
sg.ReflectionQuality=5
sg.PostProcessQuality=5
sg.TextureQuality=3
sg.EffectsQuality=3
sg.FoliageQuality=5
sg.ShadingQuality=5
sg.LandscapeQuality=5
