# RaceGenderChanger

RaceGenderChanger is a Dalamud plugin for Final Fantasy XIV that changes how your own character appears in your game client. Choose any of the eight playable races and either gender, including gender-only changes. The effect is local to your client.

## Install Penumbra

RaceGenderChanger requires the Penumbra plugin to be loaded with mods enabled.

1. In FFXIV, run `/xlsettings` and open **Experimental**.
2. Under **Custom Plugin Repositories**, enter `https://raw.githubusercontent.com/xivdev/Penumbra/master/repo.json`. Click **+**, then **Save and Close**.
3. Run `/xlplugins`, find **Penumbra** under **Available Plugins**, and install and enable it.
4. Run `/penumbra` and make sure **Enable Mods** is on.

[Official Penumbra installation guide](https://github.com/xivdev/Penumbra#installing)

## Build and load RaceGenderChanger

You’ll need the .NET 10 SDK and an XIVLauncher installation with Dalamud API 15. From the repository root, run:

```sh
dotnet build RaceGenderChanger.sln -c Release -p:Platform=x64
```

In `/xlsettings` → **Experimental**, add the full path to `RaceGenderChanger/bin/x64/Release/RaceGenderChanger.dll` under **Dev Plugin Locations** and save. Keep the DLL in its build output directory alongside its manifest and dependencies. In `/xlplugins`, find **RaceGenderChanger** and enable it.

## Usage

- Run `/rgc` to open the settings. Pick a **Target Race** and **Target Gender**, then check **Enable**.
- Run `/rgc on` to apply your saved selection, or `/rgc off` to restore your character’s usual appearance.
- Check **Stay on** to keep the effect enabled when the plugin reloads or the game restarts.
- The **Add HQ symbol to your changed race** option adds an HQ symbol to your own nameplate while the appearance change is active.

If RaceGenderChanger reports that Penumbra’s API is unavailable, check that Penumbra is installed and enabled in `/xlplugins` and that `/penumbra` opens. If RaceGenderChanger asks you to enable mods, check **Enable Mods** in Penumbra and retry `/rgc on`.

## Credits

Based on the original plugin by Avaflow, with compatibility updates by Ars Magna and Kelvin. Blaksey forked it to focus on the local character and add gender selection.
