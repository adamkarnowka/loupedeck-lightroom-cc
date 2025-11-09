namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class Rate0Command : PluginDynamicCommand
    {
        public Rate0Command()
            : base(displayName: "Rate 0", description: "Set photo rating to 0 stars", groupName: "Flagging & Rating")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("rating0");
                    PluginLog.Info("Photo rated 0 stars");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error rating photo: {ex.Message}");
                }
            });
        }
    }
}

