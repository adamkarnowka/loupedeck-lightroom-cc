namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class ResetAllAdjustmentsCommand : PluginDynamicCommand
    {
        public ResetAllAdjustmentsCommand()
            : base(displayName: "Reset All Adjustments", description: "Reset all develop adjustments to default values", groupName: "General")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    PluginLog.Info("Resetting all develop adjustments...");
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("resetAllDevelopAdjustments");
                    PluginLog.Info("All develop adjustments reset successfully");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error resetting all adjustments: {ex.Message}");
                }
            });
        }
    }
}

