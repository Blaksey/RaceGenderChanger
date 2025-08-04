using Dalamud.Plugin;
using Penumbra.Api.Enums;
using Penumbra.Api.Helpers;
using Penumbra.Api.IpcSubscribers;
using System;

namespace RaceGenderChanger.Utils
{
    internal class PenumbraIpc(IDalamudPluginInterface pluginInterface) : IDisposable
    {
        private readonly RedrawAll redrawAll = new(pluginInterface);
        private readonly RedrawObject redrawObject = new(pluginInterface);
        private readonly EventSubscriber<nint, Guid, nint, nint, nint> creatingCharacterBaseEvent =
            CreatingCharacterBase.Subscriber(pluginInterface, Drawer.OnCreatingCharacterBase);

        public void Dispose()
        {
            creatingCharacterBaseEvent.Dispose();
        }

        internal void RedrawAll(RedrawType setting)
        {
            try
            {
                redrawAll.Invoke(setting);
            }
            catch (Exception ex)
            {
                Plugin.OutputChatLine($"Warning: Penumbra not found. Error: {ex.Message}\n" +
                                      "Note: if you disable Penumbra before this plugin, lalafells will stay there until updated.");
            }
        }

        internal void RedrawLocalPlayer(RedrawType setting)
        {
            try
            {
                if (Service.clientState.LocalPlayer != null)
                {
                    redrawObject.Invoke(Service.clientState.LocalPlayer.ObjectIndex, setting);
                }
            }
            catch (Exception ex)
            {
                Plugin.OutputChatLine($"Warning: Could not redraw local player. Error: {ex.Message}");
                // Fallback to redrawing all if targeted redraw fails
                RedrawAll(setting);
            }
        }
    }
}
