namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class PasteEditSettingsCommand : PluginDynamicCommand
    {
        private Boolean _canPaste = false;
        private Timer _statusCheckTimer;
        private static PasteEditSettingsCommand _instance;

        public PasteEditSettingsCommand()
            : base(displayName: "Paste Edit Settings", description: "Paste edit settings to current photo", groupName: "General")
        {
            _instance = this;
            this._statusCheckTimer = new Timer(CheckPasteStatus, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
        }

        public static void ForceStatusCheck()
        {
            _instance?.CheckPasteStatusNow();
        }

        private void CheckPasteStatusNow()
        {
            CheckPasteStatus(null);
        }

        private void CheckPasteStatus(Object state)
        {
            Task.Run(async () =>
            {
                try
                {
                    var response = await LightroomPlugin.WebSocketClient.SendCommandWithResponseAsync("canPasteEditSettings");
                    
                    if (!String.IsNullOrEmpty(response))
                    {
                        using (JsonDocument doc = JsonDocument.Parse(response))
                        {
                            if (doc.RootElement.TryGetProperty("response", out var responseElement))
                            {
                                Boolean newCanPaste = false;
                                
                                if (responseElement.ValueKind == JsonValueKind.Array && responseElement.GetArrayLength() > 0)
                                {
                                    newCanPaste = responseElement[0].GetBoolean();
                                }
                                else if (responseElement.ValueKind == JsonValueKind.True || responseElement.ValueKind == JsonValueKind.False)
                                {
                                    newCanPaste = responseElement.GetBoolean();
                                }

                                if (newCanPaste != this._canPaste)
                                {
                                    this._canPaste = newCanPaste;
                                    this.ActionImageChanged();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Warning($"Error checking paste status: {ex.Message}");
                }
            });
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (!this._canPaste)
            {
                using (var bitmapBuilder = new BitmapBuilder(imageSize))
                {
                    bitmapBuilder.Clear(BitmapColor.Black);
                    bitmapBuilder.FillRectangle(0, 0, bitmapBuilder.Width, bitmapBuilder.Height, new BitmapColor(40, 40, 40));
                    bitmapBuilder.DrawText("Paste\nDisabled", new BitmapColor(102, 102, 102));
                    return bitmapBuilder.ToImage();
                }
            }

            return base.GetCommandImage(actionParameter, imageSize);
        }

        protected override void RunCommand(String actionParameter)
        {
            if (!this._canPaste)
            {
                PluginLog.Warning("Cannot paste edit settings - no settings copied");
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    PluginLog.Info("Pasting edit settings...");
                    await LightroomPlugin.WebSocketClient.SendCommandAsync("pasteEditSettings");
                    PluginLog.Info("Edit settings pasted successfully");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error pasting edit settings: {ex.Message}");
                }
            });
        }
    }
}

