namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class ColorLabelRedCommand : PluginDynamicCommand
    {
        public ColorLabelRedCommand()
            : base(displayName: "Color: Red", description: "Set color label to red", groupName: "Color labels")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("colorLabelRed");
                    PluginLog.Info("Set color label to red");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error setting color label to red: {ex.Message}");
                }
            });
        }
    }
}

