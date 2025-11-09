namespace Loupedeck.LightroomPlugin
{
    using System;


    public class ShowLightroomConnectionCommand : PluginDynamicCommand
    {
        public ShowLightroomConnectionCommand()
            : base(displayName: "Connection Info", description: "Shows current Lightroom WebSocket connection settings", groupName: "Settings")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                var plugin = this.Plugin as LightroomPlugin;
                if (plugin == null)
                {
                    PluginLog.Error("Failed to get plugin instance");
                    return;
                }

                var host = plugin.GetLightroomHost();
                var port = plugin.GetLightroomPort();
                var url = plugin.GetLightroomWebSocketUrl();

                PluginLog.Info("=== Lightroom Connection Settings ===");
                PluginLog.Info($"Host: {host}");
                PluginLog.Info($"Port: {port}");
                PluginLog.Info($"WebSocket URL: {url}");
                PluginLog.Info("=====================================");

                this.ActionImageChanged();
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error showing connection settings: {ex.Message}");
            }
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            try
            {
                var plugin = this.Plugin as LightroomPlugin;
                if (plugin == null)
                {
                    return "Connection Info";
                }

                var host = plugin.GetLightroomHost();
                var port = plugin.GetLightroomPort();
                
                var isConnected = LightroomPlugin.WebSocketClient?.IsConnected ?? false;
                var status = isConnected ? "Connected" : "Disconnected";

                return $"{host}{Environment.NewLine}{port}{Environment.NewLine}{status}";
            }
            catch
            {
                return "Connection Info";
            }
        }
    }
}

