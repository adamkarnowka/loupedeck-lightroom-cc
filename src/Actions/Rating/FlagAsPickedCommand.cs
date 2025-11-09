namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class FlagAsPickedCommand : PluginDynamicCommand
    {
        public FlagAsPickedCommand()
            : base(displayName: "Flag as Picked", description: "Mark photo as picked", groupName: "Flagging & Rating")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("flagPick");
                    PluginLog.Info("Photo flagged as picked");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error flagging photo as picked: {ex.Message}");
                }
            });
        }
    }
}

