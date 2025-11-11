namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class CopyEditSettingsCommand : PluginDynamicCommand
    {
        public CopyEditSettingsCommand()
            : base(displayName: "Copy Edit Settings", description: "Copy edit settings from current photo", groupName: "General")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    PluginLog.Info("Copying edit settings...");
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("copyEditSettings");
                    PluginLog.Info("Edit settings copied successfully");
                    
                    await Task.Delay(100);
                    PasteEditSettingsCommand.ForceStatusCheck();
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error copying edit settings: {ex.Message}");
                }
            });
        }
    }
}

