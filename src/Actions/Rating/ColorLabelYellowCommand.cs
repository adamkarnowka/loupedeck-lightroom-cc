namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class ColorLabelYellowCommand : PluginDynamicCommand
    {
        public ColorLabelYellowCommand()
            : base(displayName: "Color: Yellow", description: "Set color label to yellow", groupName: "Color labels")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("colorLabelYellow");
                    PluginLog.Info("Set color label to yellow");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error setting color label to yellow: {ex.Message}");
                }
            });
        }
    }
}

