namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class Rate5Command : PluginDynamicCommand
    {
        public Rate5Command()
            : base(displayName: "Rate 5", description: "Set photo rating to 5 stars", groupName: "Flagging & Rating")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("rating5");
                    PluginLog.Info("Photo rated 5 stars");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error rating photo: {ex.Message}");
                }
            });
        }
    }
}

