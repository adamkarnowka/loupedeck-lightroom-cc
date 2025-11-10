namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class ColorLabelNoneCommand : PluginDynamicCommand
    {
        public ColorLabelNoneCommand()
            : base(displayName: "Color: None", description: "Remove color label", groupName: "Color labels")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("colorLabelNone");
                    PluginLog.Info("Removed color label");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error removing color label: {ex.Message}");
                }
            });
        }
    }
}

