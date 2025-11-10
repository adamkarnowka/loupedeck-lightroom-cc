namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class ColorLabelBlueCommand : PluginDynamicCommand
    {
        public ColorLabelBlueCommand()
            : base(displayName: "Color: Blue", description: "Set color label to blue", groupName: "Color labels")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("colorLabelBlue");
                    PluginLog.Info("Set color label to blue");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error setting color label to blue: {ex.Message}");
                }
            });
        }
    }
}

