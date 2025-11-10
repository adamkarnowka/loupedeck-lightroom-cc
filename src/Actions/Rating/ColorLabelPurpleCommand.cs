namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class ColorLabelPurpleCommand : PluginDynamicCommand
    {
        public ColorLabelPurpleCommand()
            : base(displayName: "Color: Purple", description: "Set color label to purple", groupName: "Color labels")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("colorLabelPurple");
                    PluginLog.Info("Set color label to purple");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error setting color label to purple: {ex.Message}");
                }
            });
        }
    }
}

