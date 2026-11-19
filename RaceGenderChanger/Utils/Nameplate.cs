using Dalamud.Game.Gui.NamePlate;
using System;
using System.Collections.Generic;

namespace RaceGenderChanger.Utils;

internal sealed class Nameplate : IDisposable
{
    public Nameplate() => Service.namePlateGui.OnNamePlateUpdate += UpdateNameplates;

    private static void UpdateNameplates(INamePlateUpdateContext context, IReadOnlyList<INamePlateUpdateHandler> handlers)
    {
        if (!Service.configuration.enabled || !Service.configuration.nameHQ || Drawer.ChangedActor == 0)
            return;

        foreach (var handler in handlers)
        {
            if (handler.PlayerCharacter?.Address == Drawer.ChangedActor)
                handler.NameParts.Text = $"{handler.Name} \uE03C";
        }
    }

    public void Dispose() => Service.namePlateGui.OnNamePlateUpdate -= UpdateNameplates;
}
