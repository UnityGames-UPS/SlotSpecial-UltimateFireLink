# CLAUDE.md

## Scope restriction

Claude is only allowed to touch C# scripts (`Assets/Scripts/**`, etc.). Claude must not create, edit, or delete any Unity-related objects/assets — scenes (`.unity`), prefabs (`.prefab`), materials, meta files, or any other assets in the Unity Editor. All Unity-side changes (scenes, prefabs, inspector wiring, etc.) are handled by the user.

## Codebase overview

Unity slot game ("Ultimate Fire Link"). Gameplay logic lives in `Assets/Scripts/`.

### Key scripts
- `Functionality/SlotBehaviour.cs` — core gameplay: spin flow, reel tweening, bet changes (`ChangeBet`/`MaxBet`), turbo, auto-spin, free-spin, and balance/win text updates. Button click listeners are (re)wired centrally in `assignbuttons()`, which runs on `Start()` and again on every orientation switch.
- `Functionality/BonusController.cs` — bonus/hold-and-spin game; reads `slotBehaviour.IsTurboOn` to speed up its own animations.
- `UIManager.cs` — paytable/sound/music/exit buttons and their sprite state.
- `OrientationChange.cs` — receives device + orientation messages (from JS/WebGL host; editor hotkeys `K`=PC, `L`=mobile, `O`=iPhone in `Update`), rotates/scales the `UIWrapper`, and delegates UI swaps to `CanvasScalerSwitcher`.
- `APIs/CanvasScalerSwitcher.cs` — swaps the active UI set for **PC/landscape** vs **mobile/portrait** (and Apple offset tweaks). On each switch it rebinds the shared text/button references on `SlotBehaviour`/`UIManager` to the correct orientation's widgets, then calls `assignbuttons()`.

### Portrait vs landscape UI convention
Both orientations' widgets exist in the scene at once; code toggles/updates the matching pair together. Naming conventions for paired references:
- `SlotBehaviour.cs`: `Mobile_`-prefixed = portrait, unprefixed = landscape/PC (e.g. `Mobile_StopSpin_Button` / `StopSpin_Button`). Auto-spin uses `M_`/`P_` prefixes.
- `CanvasScalerSwitcher.cs`: `m_` = mobile/portrait, `p_` = PC/landscape (e.g. `m_creditText` / `p_creditText`).

### Turbo button pattern
Turbo uses a **two-GameObject switch** (not a sprite swap). Per orientation there is a *Turbo-Off* button (shown by default) and a *Turbo-On* button (hidden by default): `Turbo_Off_Button`/`Turbo_On_Button` (landscape) and `Mobile_Turbo_Off_Button`/`Mobile_Turbo_On_Button` (portrait). `ApplyTurboState(bool)` sets `IsTurboOn` and `SetActive`s all four so both orientations stay in sync; the click handler `SetTurbo(bool)` wraps it with button audio. The Turbo-On GameObject plays its own animation on enable (Unity-side).
