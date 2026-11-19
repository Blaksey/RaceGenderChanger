using Dalamud.Plugin;
using Dalamud.Plugin.Ipc.Exceptions;
using Penumbra.Api.Enums;
using Penumbra.Api.IpcSubscribers;
using System;

namespace RaceGenderChanger.Utils;

internal sealed class PenumbraIpc : IDisposable
{
    private readonly GetEnabledState getEnabledState;
    private readonly RedrawObject redrawObject;
    private readonly IDisposable creation;
    private readonly IDisposable initialized;
    private readonly IDisposable enabledChange;

    public PenumbraIpc(IDalamudPluginInterface pluginInterface)
    {
        getEnabledState = new GetEnabledState(pluginInterface);
        redrawObject = new RedrawObject(pluginInterface);
        creation = CreatingCharacterBase.Subscriber(pluginInterface, Drawer.OnCreatingCharacterBase);
        initialized = Initialized.Subscriber(pluginInterface, OnPenumbraReady);
        enabledChange = EnabledChange.Subscriber(pluginInterface, enabled =>
        {
            if (enabled)
                OnPenumbraReady();
        });
    }

    private static void OnPenumbraReady()
    {
        if (Service.configuration.enabled)
            Service.drawer?.RefreshLocalPlayer();
    }

    internal void RedrawLocalPlayer(bool reportUnavailable)
    {
        try
        {
            var player = Service.objectTable.LocalPlayer;
            if (player == null)
                return;

            if (!getEnabledState.Invoke())
            {
                if (reportUnavailable)
                    Plugin.OutputChatLine("Enable mods in Penumbra to change your appearance.");
                return;
            }

            redrawObject.Invoke(player.ObjectIndex, RedrawType.Redraw);
        }
        catch (IpcNotReadyError ex)
        {
            Service.log.Warning($"Penumbra IPC is unavailable: {ex.Message}");
            if (reportUnavailable)
                Plugin.OutputChatLine("Penumbra has not loaded its API. Install or enable Penumbra in the Dalamud Plugin Installer.");
        }
        catch (Exception ex)
        {
            Service.log.Warning(ex, "Could not request a local-player redraw from Penumbra.");
            if (reportUnavailable)
                Plugin.OutputChatLine("Could not redraw your character. Check that Penumbra is installed, up to date, and active.");
        }
    }

    public void Dispose()
    {
        initialized.Dispose();
        enabledChange.Dispose();
        creation.Dispose();
    }
}
