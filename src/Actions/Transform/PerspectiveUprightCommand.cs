namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;


    public class PerspectiveUprightCommand : PluginDynamicCommand
    {
        public PerspectiveUprightCommand()
            : base(displayName: "Perspective Upright", description: "Executes Perspective Upright Auto mode", groupName: "Transform")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LightroomPlugin.WebSocketClient.SetValueAsync("PerspectiveUpright", 1);
                    PluginLog.Info("Successfully executed Perspective Upright Auto");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error executing Perspective Upright: {ex.Message}");
                }
            });
        }
    }
}

