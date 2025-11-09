namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class FlagAsRejectedCommand : PluginDynamicCommand
    {
        public FlagAsRejectedCommand()
            : base(displayName: "Flag as Rejected", description: "Mark photo as rejected", groupName: "Flagging & Rating")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("flagReject");
                    PluginLog.Info("Photo flagged as rejected");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error flagging photo as rejected: {ex.Message}");
                }
            });
        }
    }
}

