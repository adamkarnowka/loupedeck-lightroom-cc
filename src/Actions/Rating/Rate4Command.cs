namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class Rate4Command : PluginDynamicCommand
    {
        public Rate4Command()
            : base(displayName: "Rate 4", description: "Set photo rating to 4 stars", groupName: "Flagging & Rating")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("rating4");
                    PluginLog.Info("Photo rated 4 stars");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error rating photo: {ex.Message}");
                }
            });
        }
    }
}

