namespace Loupedeck.LightroomPlugin
{
    using System;


    public class ConfigureLightroomConnectionCommand : ActionEditorCommand
    {
        private const String HostControlName = "Host";
        private const String PortControlName = "Port";

        public ConfigureLightroomConnectionCommand()
        {
            this.Name = "ConfigureLightroomConnection";
            this.DisplayName = "Configure Lightroom Connection";
            this.GroupName = "Settings";
            this.Description = "Configure Lightroom WebSocket host and port";

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: HostControlName, labelText: "Host:")
                    .SetPlaceholder("127.0.0.1")
                    .SetRequired());

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: PortControlName, labelText: "Port:")
                    .SetPlaceholder("7682")
                    .SetRequired());

            this.ActionEditor.ControlValueChanged += this.OnControlValueChanged;
        }

        private void OnControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
        {
            var host = e.ActionEditorState.GetControlValue(HostControlName);
            var port = e.ActionEditorState.GetControlValue(PortControlName);

            if (!String.IsNullOrWhiteSpace(host) && !String.IsNullOrWhiteSpace(port))
            {
                e.ActionEditorState.SetDisplayName($"Configure LR ({host}:{port})");
            }
            else if (!String.IsNullOrWhiteSpace(host))
            {
                e.ActionEditorState.SetDisplayName($"Configure LR ({host}:?)");
            }
            else
            {
                e.ActionEditorState.SetDisplayName("Configure LR Connection");
            }
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            try
            {
                if (!actionParameters.TryGetString(HostControlName, out var host) || String.IsNullOrWhiteSpace(host))
                {
                    PluginLog.Warning("Host is required for Lightroom connection configuration");
                    return false;
                }

                if (!actionParameters.TryGetString(PortControlName, out var port) || String.IsNullOrWhiteSpace(port))
                {
                    PluginLog.Warning("Port is required for Lightroom connection configuration");
                    return false;
                }

                if (!Int32.TryParse(port, out var portNumber) || portNumber < 1 || portNumber > 65535)
                {
                    PluginLog.Warning($"Invalid port number: {port}. Port must be between 1 and 65535.");
                    return false;
                }

                var plugin = this.Plugin as LightroomPlugin;
                if (plugin == null)
                {
                    PluginLog.Error("Failed to get plugin instance");
                    return false;
                }

                plugin.SetLightroomHost(host);
                plugin.SetLightroomPort(port);

                PluginLog.Info($"Lightroom connection configured: {host}:{port}");
                
                plugin.ReconnectToLightroom();
                PluginLog.Info("Reconnected to Lightroom with new settings.");

                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error configuring Lightroom connection: {ex.Message}");
                return false;
            }
        }
    }
}

