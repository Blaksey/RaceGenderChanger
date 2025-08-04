using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Penumbra.Api.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static OopsAllLalafellsSRE.Utils.Constant;

namespace OopsAllLalafellsSRE.Utils
{
    internal class Drawer : IDisposable
    {
        public static HashSet<string> NonNativeID = [];

        public Drawer()
        {
            Service.configWindow.OnConfigChanged += RefreshLocalPlayer;
            if (Service.configuration.enabled)
            {
                Plugin.OutputChatLine("OopsAllLalafellsSRE starting...");
                RefreshLocalPlayer();
            }
        }

        private static void RefreshLocalPlayer()
        {
            // Always allow refresh - this is needed when:
            // 1. Plugin gets enabled (to apply changes)
            // 2. Plugin gets disabled (to revert changes)
            // 3. Settings change while enabled (to update appearance)
            Plugin.OutputChatLine("Refreshing local player appearance");
            Service.penumbraApi.RedrawLocalPlayer(RedrawType.Redraw);
            Service.namePlateGui.RequestRedraw();
        }

        public static unsafe void OnCreatingCharacterBase(nint gameObjectAddress, Guid _1, nint _2, nint customizePtr, nint _3)
        {
            if (!Service.configuration.enabled) return;

            // return if not player character
            var gameObj = (GameObject*)gameObjectAddress;
            if (gameObj->ObjectKind != ObjectKind.Pc) return;

            // Only change your own character - check if this is the local player
            if (Service.clientState.LocalPlayer == null) return;
            if (gameObj->NameString != Service.clientState.LocalPlayer.Name.TextValue) return;

            var customData = Marshal.PtrToStructure<CharaCustomizeData>(customizePtr);

            // Determine target gender with safety checks
            byte targetGender = Service.configuration.SelectedGender;
            if (Service.configuration.SelectedRace == Race.HROTHGAR)
            {
                targetGender = 0; // Force male for Hrothgar
            }
            else if (Service.configuration.SelectedRace == Race.VIERA)
            {
                targetGender = 1; // Force female for Viera
            }

            // Check if we need to change race OR gender
            bool needsRaceChange = customData.Race != Service.configuration.SelectedRace && customData.Race != Race.UNKNOWN;
            bool needsGenderChange = customData.Gender != targetGender;

            if (!needsRaceChange && !needsGenderChange)
                return;

            NonNativeID.Add(gameObj->NameString);
            ChangeRace(customData, customizePtr, Service.configuration.SelectedRace);
        }

        private static unsafe void ChangeRace(CharaCustomizeData customData, nint customizePtr, Race selectedRace)
        {
            var originalRace = customData.Race;
            var originalGender = customData.Gender;

            // Apply gender with safety checks for race-locked genders
            byte targetGender = Service.configuration.SelectedGender;
            if (selectedRace == Race.HROTHGAR)
            {
                targetGender = 0; // Force male for Hrothgar
            }
            else if (selectedRace == Race.VIERA)
            {
                targetGender = 1; // Force female for Viera
            }

            customData.Race = selectedRace;
            customData.Gender = targetGender;

            // Recalculate tribe based on race and gender
            // Each race has 2 tribes: one for each gender typically
            byte baseTribe = (byte)((byte)selectedRace * 2);
            customData.Tribe = (byte)(baseTribe - 1 + targetGender);

            // Reset face and hair to valid values for the new race/gender combination
            customData.FaceType %= 4;
            customData.HairStyle = (byte)((customData.HairStyle % RaceMappings.RaceHairs[selectedRace]) + 1);

            // Debug output
            Plugin.OutputChatLine($"Race: {originalRace} → {selectedRace}, Gender: {originalGender} → {targetGender}, Tribe: {customData.Tribe}");

            Marshal.StructureToPtr(customData, customizePtr, true);
        }

        public void Dispose()
        {
            Service.configWindow.OnConfigChanged -= RefreshLocalPlayer;
        }
    }
}
