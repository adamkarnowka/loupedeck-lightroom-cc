namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class Rate3Command : PluginDynamicCommand
    {
        public Rate3Command()
            : base(displayName: "Rate 3", description: "Set photo rating to 3 stars", groupName: "Flagging & Rating")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("rating3");
                    PluginLog.Info("Photo rated 3 stars");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error rating photo: {ex.Message}");
                }
            });
        }
    }
}

