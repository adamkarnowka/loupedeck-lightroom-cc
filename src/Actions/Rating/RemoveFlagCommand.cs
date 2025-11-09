namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class RemoveFlagCommand : PluginDynamicCommand
    {
        public RemoveFlagCommand()
            : base(displayName: "Remove Flag", description: "Remove pick/reject flag from photo", groupName: "Flagging & Rating")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("flagUnflag");
                    PluginLog.Info("Photo flag removed");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error removing flag: {ex.Message}");
                }
            });
        }
    }
}

