# Fortnite Cinematic Settings Tool

A simple Windows utility that enables Fortnite’s hidden cinematic graphics settings by modifying `GameUserSettings.ini`.

Built by **EDMIRE from BiomeForge**.

## What It Does

This tool automatically finds Fortnite’s `GameUserSettings.ini` file and applies a set of high-end cinematic rendering values, including:

- Nanite enabled
- Ray tracing enabled
- Global illumination quality set to cinematic levels
- Reflection quality set to cinematic levels
- Shadows, post-processing, foliage, shading, and landscape quality boosted beyond normal in-game presets

It also includes:

- Automatic backup creation before changes are applied
- One-click undo from the latest backup
- Optional read-only protection to prevent Fortnite from resetting the settings
- A clean Windows desktop UI
- Manual file selection for unusual installs or troubleshooting

## Important GPU Warning

These settings have **immense GPU requirements**.

DLSS, TSR, XeSS, or another upscaling method is strongly recommended unless your system has at least an **RTX 4090** or **RTX 5090**.

Expect significantly reduced performance on lower-end hardware.

## How To Use

1. Close Fortnite.
2. Run `FortniteCinematicSettings.exe`. If it claims the game is open when its not, ignore it, it will do no harm
3. Confirm that the tool found your `GameUserSettings.ini`.
4. Leave **Set GameUserSettings.ini to read-only after applying** enabled if you want Fortnite to stop resetting the values.
5. Click **Apply cinematic settings**.
6. Launch Fortnite.

## Undoing Changes

Click **Undo from backup** inside the app.

The tool creates a timestamped backup before applying changes, stored next to the original `GameUserSettings.ini`.

## Settings Applied

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
