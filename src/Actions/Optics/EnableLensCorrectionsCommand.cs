namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class EnableLensCorrectionsCommand : PluginDynamicCommand
    {
        private Boolean _isEnabled = false;

        public EnableLensCorrectionsCommand()
            : base(displayName: "Optics: Enable Lens Corrections", description: "Toggle lens profile corrections", groupName: "Optics")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    _isEnabled = !_isEnabled;
                    var value = _isEnabled ? 1 : 0;
                    
                    await LightroomPlugin.WebSocketClient.SetValueAsync("LensProfileEnable", value);
                    PluginLog.Info($"Lens Corrections {(_isEnabled ? "enabled" : "disabled")}");
                    
                    this.ActionImageChanged();
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error toggling lens corrections: {ex.Message}");
                }
            });
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            return _isEnabled ? "Lens: ON" : "Lens: OFF";
        }
    }
}

