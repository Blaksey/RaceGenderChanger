using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using RaceGenderChanger.Utils;
using System;
using System.Numerics;
using static RaceGenderChanger.Utils.Constant;

namespace RaceGenderChanger.Windows;

internal class ConfigWindow : Window
{
    private readonly Configuration configuration;
    private readonly string[] race = ["Lalafell", "Hyur", "Elezen", "Miqo'te", "Roegadyn", "Au Ra", "Hrothgar", "Viera"];
    private int selectedRaceIndex;
    private static readonly Race[] RaceOrder = [Race.LALAFELL, Race.HYUR, Race.ELEZEN, Race.MIQOTE, Race.ROEGADYN, Race.AU_RA, Race.HROTHGAR, Race.VIERA];
    public event Action? OnConfigChanged;

    public ConfigWindow() : base(
        "RaceGenderChanger Configuration Window",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(285, 190);
        SizeCondition = ImGuiCond.Always;

        configuration = Service.configuration;
        selectedRaceIndex = Math.Max(0, Array.IndexOf(RaceOrder, configuration.SelectedRace));
    }

    public override void Draw()
    {
        // select race
        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted("Target Race");
        ImGui.SameLine();
        if (ImGui.Combo("###Race", ref selectedRaceIndex, race, race.Length))
        {
            configuration.SelectedRace = RaceOrder[selectedRaceIndex];
            SaveAppearance();
        }

        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted("Target Gender");
        ImGui.SameLine();
        if (ImGui.RadioButton("Male", configuration.SelectedGender == 0))
        {
            configuration.SelectedGender = 0;
            SaveAppearance();
        }
        ImGui.SameLine();
        if (ImGui.RadioButton("Female", configuration.SelectedGender == 1))
        {
            configuration.SelectedGender = 1;
            SaveAppearance();
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

    private void SaveAppearance()
    {
        configuration.Save();
        if (configuration.enabled)
            OnConfigChanged?.Invoke();
    }

    public void InvokeConfigChanged()
    {
        OnConfigChanged?.Invoke();
    }
}
