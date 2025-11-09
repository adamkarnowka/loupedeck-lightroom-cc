namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Threading.Tasks;

    public class RemoveChromaticAberrationCommand : PluginDynamicCommand
    {
        private Boolean _isEnabled = false;

        public RemoveChromaticAberrationCommand()
            : base(displayName: "Optics: Remove Chromatic Aberration", description: "Toggle chromatic aberration removal", groupName: "Optics")
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
                    
                    await LightroomPlugin.WebSocketClient.SetValueAsync("AutoLateralCA", value);
                    PluginLog.Info($"Chromatic Aberration removal {(_isEnabled ? "enabled" : "disabled")}");
                    
                    this.ActionImageChanged();
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error toggling chromatic aberration: {ex.Message}");
                }
            });
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            return _isEnabled ? "CA: ON" : "CA: OFF";
        }
    }
}


