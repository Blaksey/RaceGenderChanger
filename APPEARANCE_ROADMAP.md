# Appearance customization roadmap

Extend RaceGenderChanger into a local appearance editor. Each control should work independently, so a player can change their hairstyle, height, or eye color while keeping their current race and gender. Turning the plugin off should restore the game's appearance through the existing Penumbra redraw.

## Design

- **Settings.** Add a **Keep current** choice for race, gender, and each new field. Store explicit overrides separately from inherited values, since zero can be a valid choice. Migrate existing settings so saved race and gender selections still apply, and default new fields to **Keep current**.
- **Available choices.** Resolve race and gender first, then clan and face, then any fields that depend on them. Use game data for choices and defaults. Preserve the player's incoming value when it is valid for the target appearance. If a saved selection becomes invalid after a race or clan change, show the fallback in the UI and keep the saved choice available when the player switches back.
- **Application.** Use the existing Penumbra character-creation callback in `Utils/Drawer.cs`. Resolve overrides on a copy of the incoming customization data, validate them, and write the result to the local human model's creation buffer. Replace the current race/gender equality check with a comparison that also accounts for appearance-only changes. Keep the framework-thread redraw and the guard against writing to live actor storage.
- **UI.** Group controls in the existing `/rgc` window. Show choices supported by the effective race, clan, gender, and face. Save and redraw once per committed change, such as when a player releases the height slider. Keep the existing enable, restore, and stay-on behavior.

## Implementation order

1. **Independent overrides, clan, and height.** Add the settings migration and **Keep current** mode. Preserve clan parity on race changes when the player has no explicit clan override and the incoming clan does not belong to the target race. Apply height within the game's supported range. Verify that height alone changes the model and that enabling or disabling the plugin restores the expected appearance.
2. **Hairstyle, hair color, and highlights.** Populate the controls from game data, using the resolved appearance to filter compatible styles. Verify a hair-only change, a race switch that invalidates the selected hairstyle, and a switch back to the original choice.
3. **Face and other character-creator options.** Add face selection, facial features, skin tone, eye colors, and face paint. Resolve face before face-dependent options. Add race-specific settings such as tail shape only where the target model supports them.
4. **Presets.** Save named sets of overrides, including which fields use **Keep current**. Apply each preset with one redraw and resolve its choices through the same validation used by the editor.
5. **Equipment and custom proportions.** Evaluate outfits, dyes, and body scaling separately. Equipment and dyes require their own compatibility and restoration checks, and proportions beyond the character creator's built-in options involve the model or skeletal system. Evaluate whether to integrate with existing tools before expanding the native code.

## Verification

- Confirm that existing configuration files load with their saved race, gender, and enable settings.
- Check race-only, gender-only, and appearance-only edits across multiple races and clans, including Hrothgar and Viera. Check that nearby players stay unchanged.
- Exercise missing or incompatible values and confirm the character remains visible. Check disable, plugin unload, zoning, and login with **Stay on**.
- Confirm the HQ nameplate indicator updates when the appearance is applied or restored. Decide whether appearance-only edits should display the indicator, and update its label if needed.

[FFXIVClientStructs customization fields](https://github.com/aers/FFXIVClientStructs/blob/main/FFXIVClientStructs/FFXIV/Client/Game/Character/DrawDataContainer.cs#L119-L167)
