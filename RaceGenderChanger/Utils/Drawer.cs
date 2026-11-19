using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System;
using static RaceGenderChanger.Utils.Constant;

namespace RaceGenderChanger.Utils;

internal sealed class Drawer : IDisposable
{
    private bool disposed;
    internal static nint ChangedActor { get; private set; }

    public Drawer()
    {
        Service.configWindow.OnConfigChanged += RefreshLocalPlayer;
        Service.clientState.Login += OnLogin;
        Service.clientState.Logout += OnLogout;
        if (Service.configuration.enabled)
            RefreshLocalPlayer();
    }

    private void OnLogin()
    {
        if (Service.configuration.enabled)
            RefreshLocalPlayer();
    }

    private void OnLogout(int type, int code) => ChangedActor = 0;

    // Defer even on the framework thread so UI callbacks and IPC notifications can finish first.
    internal void RefreshLocalPlayer() => _ = Service.framework.RunOnTick(() =>
    {
        if (disposed)
            return;

        Service.penumbraApi.RedrawLocalPlayer(reportUnavailable: Service.configuration.enabled);
        Service.namePlateGui.RequestRedraw();
    });

    internal static unsafe void OnCreatingCharacterBase(nint actor, Guid _collection, nint model, nint data, nint _equipment)
    {
        if (!Service.configuration.enabled || !Service.framework.IsInFrameworkUpdateThread ||
            actor == 0 || model == 0 || data == 0 || *(ushort*)model != 0 ||
            Service.objectTable.LocalPlayer?.Address != actor)
            return;

        ChangedActor = 0;
        var customize = (CustomizeData*)data;
        // Penumbra normally supplies the model's creation buffer. Never overwrite live actor data.
        if (customize == &((Character*)actor)->DrawData.CustomizeData)
        {
            Service.log.Warning("Penumbra supplied live actor customization to the creation callback.");
            return;
        }

        var targetRace = (byte)Service.configuration.SelectedRace;
        var targetSex = Service.configuration.SelectedGender;
        if (targetRace is < (byte)Race.HYUR or > (byte)Race.VIERA || targetSex > 1 ||
            customize->Race is < (byte)Race.HYUR or > (byte)Race.VIERA ||
            (customize->Race == targetRace && customize->Sex == targetSex))
            return;

        var target = *customize;
        if (target.Race != targetRace)
            target.Tribe = (byte)(targetRace * 2 - (target.Tribe & 1));

        target.Race = targetRace;
        target.Sex = targetSex;
        var normalized = target;
        _ = normalized.Normalize(&target);
        *customize = normalized;
        ChangedActor = actor;
    }

    public void Dispose()
    {
        disposed = true;
        Service.configWindow.OnConfigChanged -= RefreshLocalPlayer;
        Service.clientState.Login -= OnLogin;
        Service.clientState.Logout -= OnLogout;
        ChangedActor = 0;
    }
}
