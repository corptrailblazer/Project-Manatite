# Project Manatite Feature Checklist

This document tracks upcoming features, improvements, and refactors to be implemented in the project.

## Pending Tasks

## Completed Tasks
- [x] Initial Architecture Documentation created.
- [x] AI Context Lookup Table created.
- [x] **Settings Panel: Master Volume Control**: Master slider added; effective volume = `master * channel` applied to AudioMixer. Both MainMenu and EntreMapas wired.
- [x] **Settings Panel: Event-Driven Architecture**: `FloatEventChannelSO` assets (Master, Music, SFX) and `VoidEventChannelSO` (SettingsApplied) created and wired to `SettingsMenu` in both scenes.
- [x] **Settings Panel: VSync Toggle**: `Toggle_VSync` added to both scenes; drives `QualitySettings.vSyncCount`, saved via PlayerPrefs.
- [x] **Settings Panel: Safe Resolution Apply**: 15-second `RevertBanner` with live countdown and [Keep] / [Revert] buttons; duration configurable via `GameConfigSO.ResolutionRevertDuration`. Wired in both scenes.
- [x] **Settings Panel: Section Headers**: AUDIO and VIDEO section labels added above their respective controls in both scenes.
- [x] **Settings Panel: Inline Volume Readouts**: Percentage `TMP_Text` labels (Master, Music, SFX) positioned to the right of each slider, updated on every value change.
