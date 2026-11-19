using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using RaceGenderChanger.Utils;
using RaceGenderChanger.Windows;

namespace RaceGenderChanger
{
    internal sealed class Plugin : IDalamudPlugin
    {
        public static string Name => "RaceGenderChanger";
        private const string CommandName = "/rgc";

        public WindowSystem WindowSystem { get; } = new("RaceGenderChanger");

        public Plugin(IDalamudPluginInterface pluginInterface)
        {
            Service.pluginInterface = pluginInterface;

            Service.configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            if (!Service.configuration.stayOn)
            {
                Service.configuration.enabled = false;
            }

            Service.configuration.Initialize(pluginInterface);

            _ = pluginInterface.Create<Service>();
            Service.penumbraApi = new PenumbraIpc(pluginInterface);
            Service.configWindow = new ConfigWindow();
            WindowSystem.AddWindow(Service.configWindow);

            Service.drawer = pluginInterface.Create<Drawer>()!;
            Service.nameplate = pluginInterface.Create<Nameplate>()!;

            pluginInterface.UiBuilder.Draw += DrawUI;
            pluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            pluginInterface.UiBuilder.OpenMainUi += DrawConfigUI;

            Service.commandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens race change config menu."
            });
        }

        public static void OutputChatLine(SeString message)
        {
            var sb = new SeStringBuilder().AddUiForeground("[RGC] ", 58).Append(message);
            Service.chatGui.Print(new XivChatEntry { Message = sb.BuiltString });
        }

        public void Dispose()
        {
            Service.pluginInterface.UiBuilder.Draw -= DrawUI;
            Service.pluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
            Service.pluginInterface.UiBuilder.OpenMainUi -= DrawConfigUI;

            var penumbra = Service.penumbraApi;
            Service.drawer?.Dispose();
            penumbra?.Dispose();
            if (Service.configuration.enabled && penumbra != null)
                _ = Service.framework.RunOnFrameworkThread(() => penumbra.RedrawLocalPlayer(reportUnavailable: false));

            WindowSystem.RemoveAllWindows();

            Service.nameplate?.Dispose();
            Service.commandManager?.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            if (args == "on")
            {
                Service.configuration.enabled = true;
                Service.configuration.Save();
                Service.configWindow.InvokeConfigChanged();
                return;
            }
            if (args == "off")
            {
                Service.configuration.enabled = false;
                Service.configuration.Save();
                Service.configWindow.InvokeConfigChanged();
                return;
            }
            Service.configWindow.IsOpen = true;
        }

        private void DrawUI() => WindowSystem.Draw();

        public static void DrawConfigUI() => Service.configWindow.IsOpen = true;
    }
}
