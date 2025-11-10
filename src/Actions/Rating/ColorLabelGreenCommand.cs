namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class ColorLabelGreenCommand : PluginDynamicCommand
    {
        public ColorLabelGreenCommand()
            : base(displayName: "Color: Green", description: "Set color label to green", groupName: "Color labels")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("colorLabelGreen");
                    PluginLog.Info("Set color label to green");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error setting color label to green: {ex.Message}");
                }
            });
        }
    }
}

