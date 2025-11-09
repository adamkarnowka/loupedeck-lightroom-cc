namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class Rate1Command : PluginDynamicCommand
    {
        public Rate1Command()
            : base(displayName: "Rate 1", description: "Set photo rating to 1 star", groupName: "Flagging & Rating")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("rating1");
                    PluginLog.Info("Photo rated 1 star");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error rating photo: {ex.Message}");
                }
            });
        }
    }
}

