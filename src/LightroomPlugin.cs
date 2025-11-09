namespace Loupedeck.LightroomPlugin
{
    using System;


    public class LightroomPlugin : Plugin
    {
        public static LightroomWebSocketClient WebSocketClient { get; private set; }

        private const String SettingLightroomHost = "LightroomHost";
        private const String SettingLightroomPort = "LightroomPort";

        private const String DefaultLightroomHost = "127.0.0.1";
        private const String DefaultLightroomPort = "7682";

        public override Boolean UsesApplicationApiOnly => true;

        public override Boolean HasNoApplication => true;

        public LightroomPlugin()
        {
            PluginLog.Init(this.Log);

            PluginResources.Init(this.Assembly);
        }

        public override void Load()
        {
            var host = GetLightroomHost();
            var port = GetLightroomPort();
            var webSocketUrl = $"ws://{host}:{port}";

            WebSocketClient = new LightroomWebSocketClient(webSocketUrl);
            PluginLog.Info($"Lightroom WebSocket client initialized with URL: {webSocketUrl}");
        }

        public override void Unload()
        {
            if (WebSocketClient != null)
            {
                WebSocketClient.CloseAsync().Wait();
                PluginLog.Info("Lightroom WebSocket client closed");
            }
        }

        public String GetLightroomHost()
        {
            if (this.TryGetPluginSetting(SettingLightroomHost, out var host) && !String.IsNullOrWhiteSpace(host))
            {
                return host;
            }

            this.SetPluginSetting(SettingLightroomHost, DefaultLightroomHost, false);
            return DefaultLightroomHost;
        }

        public void SetLightroomHost(String host)
        {
            if (String.IsNullOrWhiteSpace(host))
            {
                host = DefaultLightroomHost;
            }

            this.SetPluginSetting(SettingLightroomHost, host, false);
            PluginLog.Info($"Lightroom host updated to: {host}");
        }

        public String GetLightroomPort()
        {
            if (this.TryGetPluginSetting(SettingLightroomPort, out var port) && !String.IsNullOrWhiteSpace(port))
            {
                return port;
            }

            this.SetPluginSetting(SettingLightroomPort, DefaultLightroomPort, false);
            return DefaultLightroomPort;
        }

        public void SetLightroomPort(String port)
        {
            if (String.IsNullOrWhiteSpace(port))
            {
                port = DefaultLightroomPort;
            }

            this.SetPluginSetting(SettingLightroomPort, port, false);
            PluginLog.Info($"Lightroom port updated to: {port}");
        }

        public String GetLightroomWebSocketUrl()
        {
            var host = GetLightroomHost();
            var port = GetLightroomPort();
            return $"ws://{host}:{port}";
        }

        public void ReconnectToLightroom()
        {
            try
            {
                if (WebSocketClient != null)
                {
                    WebSocketClient.CloseAsync().Wait();
                    PluginLog.Info("Closed existing Lightroom connection");
                }

                var host = GetLightroomHost();
                var port = GetLightroomPort();
                var webSocketUrl = $"ws://{host}:{port}";

                WebSocketClient = new LightroomWebSocketClient(webSocketUrl);
                PluginLog.Info($"Reconnected to Lightroom at: {webSocketUrl}");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error reconnecting to Lightroom: {ex.Message}");
            }
        }
    }
}
