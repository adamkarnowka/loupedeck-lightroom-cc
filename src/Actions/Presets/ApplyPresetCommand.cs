namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class ApplyPresetCommand : ActionEditorCommand
    {
        private const String PresetControlName = "PresetSelection";

        public ApplyPresetCommand()
        {
            this.Name = "ApplyPreset";
            this.DisplayName = "Apply Preset";
            this.GroupName = "Presets";
            this.Description = "Apply a Lightroom preset to the current photo";

            this.ActionEditor.AddControlEx(
                new ActionEditorListbox(name: PresetControlName, labelText: "Select Preset:"));

            this.ActionEditor.ListboxItemsRequested += this.OnListboxItemsRequested;
            this.ActionEditor.ControlValueChanged += this.OnControlValueChanged;
        }

        private void OnListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
        {
            if (e.ControlName.EqualsNoCase(PresetControlName))
            {
                var presets = this.LoadPresets();

                if (presets == null || presets.Count == 0)
                {
                    e.AddItem(name: "none", displayName: "Please update presets list first", description: "Run 'Get Preset IDs' command first");
                    e.SetSelectedItemName("none");
                }
                else
                {
                    var sortedPresets = presets.OrderBy(p => p.Value).ToList();

                    foreach (var preset in sortedPresets)
                    {
                        e.AddItem(name: preset.Key, displayName: preset.Value, description: $"ID: {preset.Key}");
                    }

                    if (sortedPresets.Any())
                    {
                        e.SetSelectedItemName(sortedPresets.First().Key);
                    }
                }
            }
        }

        private void OnControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
        {
            if (e.ControlName.EqualsNoCase(PresetControlName))
            {
                var presetId = e.ActionEditorState.GetControlValue(PresetControlName);

                if (!String.IsNullOrEmpty(presetId) && presetId != "none")
                {
                    var presets = this.LoadPresets();
                    if (presets != null && presets.TryGetValue(presetId, out var presetName))
                    {
                        e.ActionEditorState.SetDisplayName($"Preset: {presetName}");
                    }
                }
                else
                {
                    e.ActionEditorState.SetDisplayName("Apply Preset");
                }
            }
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            if (actionParameters.TryGetString(PresetControlName, out var presetId))
            {
                if (presetId == "none")
                {
                    PluginLog.Warning("No presets available. Please run 'Get Preset IDs' command first.");
                    return false;
                }

                Task.Run(async () =>
                {
                    try
                    {
                        PluginLog.Info($"Applying preset with ID: {presetId}");
                        await LightroomPlugin.WebSocketClient.SendCommandWithResponseAsync("applyPreset", new Object[] { presetId });
                        PluginLog.Info($"Preset {presetId} applied successfully");
                    }
                    catch (Exception ex)
                    {
                        PluginLog.Error($"Error applying preset: {ex.Message}");
                    }
                });

                return true;
            }

            return false;
        }

        private Dictionary<String, String> LoadPresets()
        {
            try
            {
                var plugin = this.Plugin as LightroomPlugin;
                if (plugin != null)
                {
                    var pluginDataDirectory = plugin.GetPluginDataDirectory();
                    var filePath = Path.Combine(pluginDataDirectory, "presets.json");

                    if (File.Exists(filePath))
                    {
                        var jsonString = File.ReadAllText(filePath);
                        var presets = JsonSerializer.Deserialize<Dictionary<String, String>>(jsonString);
                        PluginLog.Info($"Loaded {presets?.Count ?? 0} presets from file");
                        return presets;
                    }
                    else
                    {
                        PluginLog.Warning($"Presets file not found at: {filePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error loading presets: {ex.Message}");
            }

            return null;
        }
    }
}

