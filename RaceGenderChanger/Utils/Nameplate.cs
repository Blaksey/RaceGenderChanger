using Dalamud.Game.Gui.NamePlate;

namespace RaceGenderChanger.Utils
{
    internal class Nameplate
    {
        public Nameplate()
        {
            Service.namePlateGui.OnNamePlateUpdate += (context, handlers) =>
            {
                if (!Service.configuration.enabled || !Service.configuration.nameHQ)
                    return;

                foreach (var handler in handlers)
                {
                    if (handler.NamePlateKind == NamePlateKind.PlayerCharacter)
                    {
                        unsafe
                        {
                            if (handler.PlayerCharacter == null) return;

                            // Only add HQ symbol to your own character if it's been race-changed
                            if (Service.clientState.LocalPlayer != null &&
                                handler.PlayerCharacter.Name.TextValue == Service.clientState.LocalPlayer.Name.TextValue &&
                                Drawer.NonNativeID.Contains(handler.PlayerCharacter.Name.TextValue))
                            {
                                // Plugin.OutputChatLine($"Adding HQ to {handler.PlayerCharacter.Name.TextValue}");
                                handler.NameParts.Text = $"{handler.Name} \uE03C";
                            }
                        }
                    }
                }
            };
        }

        public void Dispose() { }
    }
}
