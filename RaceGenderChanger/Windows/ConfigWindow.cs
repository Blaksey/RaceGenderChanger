using Dalamud.Interface.Windowing;
using ImGuiNET;
using RaceGenderChanger.Utils;
using System;
using System.Numerics;
using static RaceGenderChanger.Utils.Constant;

namespace RaceGenderChanger.Windows;

internal class ConfigWindow : Window
{
    private readonly Configuration configuration;
    private readonly string[] race = ["Lalafell", "Hyur", "Elezen", "Miqo'te", "Roegadyn", "Au Ra", "Hrothgar", "Viera"];
    private int selectedRaceIndex = 0;
    public event Action? OnConfigChanged;

    public ConfigWindow(Plugin plugin) : base(
        "RaceGenderChanger Configuration Window",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(285, 190);
        SizeCondition = ImGuiCond.Always;

        configuration = Service.configuration;
    }

    public override void Draw()
    {
        // select race
        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted("Target Race");
        ImGui.SameLine();
        if (ImGui.Combo("###Race", ref selectedRaceIndex, race, race.Length))
        {
            configuration.SelectedRace = MapIndexToRace(selectedRaceIndex);

            // Auto-correct gender for race-locked genders
            if (configuration.SelectedRace == Race.HROTHGAR)
            {
                configuration.SelectedGender = 0; // Force male
            }
            else if (configuration.SelectedRace == Race.VIERA)
            {
                configuration.SelectedGender = 1; // Force female
            }

            configuration.Save();
            // Only trigger refresh if plugin is enabled (race/gender changes only matter when active)
            if (configuration.enabled)
            {
                OnConfigChanged?.Invoke();
            }
        }

        // select gender
        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted("Target Gender");
        ImGui.SameLine();

        // Disable gender selection for gender-locked races
        bool isGenderLocked = configuration.SelectedRace == Race.HROTHGAR || configuration.SelectedRace == Race.VIERA;
        if (isGenderLocked)
        {
            ImGui.BeginDisabled();
        }

        // Radio buttons for gender selection
        if (ImGui.RadioButton("Male", configuration.SelectedGender == 0))
        {
            if (!isGenderLocked)
            {
                configuration.SelectedGender = 0;
                configuration.Save();
                // Only trigger refresh if plugin is enabled
                if (configuration.enabled)
                {
                    OnConfigChanged?.Invoke();
                }
            }
        }
        ImGui.SameLine();
        if (ImGui.RadioButton("Female", configuration.SelectedGender == 1))
        {
            if (!isGenderLocked)
            {
                configuration.SelectedGender = 1;
                configuration.Save();
                // Only trigger refresh if plugin is enabled
                if (configuration.enabled)
                {
                    OnConfigChanged?.Invoke();
                }
            }
        }

        if (isGenderLocked)
        {
            ImGui.EndDisabled();
            ImGui.SameLine();
            string lockText = configuration.SelectedRace == Race.HROTHGAR ? "(Male only)" : "(Female only)";
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), lockText);
        }

        // Enabled
        bool _Enabled = configuration.enabled;
        if (ImGui.Checkbox("Enable", ref _Enabled))
        {
            configuration.enabled = _Enabled;
            configuration.Save();
            OnConfigChanged?.Invoke();
        }

        bool _StayOn = configuration.stayOn;
        if (ImGui.Checkbox("Stay on", ref _StayOn))
        {
            configuration.stayOn = _StayOn;
            configuration.Save();
        }

        ImGui.Separator();
        bool _NameHQ = configuration.nameHQ;
        if (ImGui.Checkbox("Add HQ symbol to your changed race", ref _NameHQ))
        {
            configuration.nameHQ = _NameHQ;
            configuration.Save();
            OnConfigChanged?.Invoke();
        }
    }

    private static Race MapIndexToRace(int index)
    {
        return index switch
        {
            0 => Race.LALAFELL,
            1 => Race.HYUR,
            2 => Race.ELEZEN,
            3 => Race.MIQOTE,
            4 => Race.ROEGADYN,
            5 => Race.AU_RA,
            6 => Race.HROTHGAR,
            7 => Race.VIERA,
            _ => Race.LALAFELL,
        };
    }

    public void InvokeConfigChanged()
    {
        OnConfigChanged?.Invoke();
    }
}
