using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using static RaceGenderChanger.Utils.Constant;

namespace RaceGenderChanger
{
    [Serializable]
    internal class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public Race SelectedRace { get; set; } = Race.LALAFELL;
        public byte SelectedGender { get; set; } = 0; // 0 = male, 1 = female
        public bool enabled { get; set; } = false;
        public bool stayOn { get; set; } = false;
        public bool nameHQ { get; set; } = true;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private IDalamudPluginInterface? pluginInterface;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}
