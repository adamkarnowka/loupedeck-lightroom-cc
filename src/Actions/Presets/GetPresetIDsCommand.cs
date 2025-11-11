namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class GetPresetIDsCommand : PluginDynamicCommand
    {
        public GetPresetIDsCommand()
            : base(displayName: "Get Preset IDs", description: "Retrieve all available preset IDs and names from Lightroom", groupName: "Presets")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    PluginLog.Info("=== Requesting Preset IDs from Lightroom ===");
                    
                    var response = await LightroomPlugin.WebSocketClient.SendCommandWithResponseAsync("getPresetIDs");
                    
                    if (!String.IsNullOrEmpty(response))
                    {
                        PluginLog.Info("=== Preset IDs Response ===");
                        PluginLog.Info(response);
                        PluginLog.Info("===========================");

                        using (JsonDocument doc = JsonDocument.Parse(response))
                        {
                            if (doc.RootElement.TryGetProperty("response", out var responseElement))
                            {
                                PluginLog.Info($"=== Response Type: {responseElement.ValueKind} ===");
                                PluginLog.Info("=== Retrieving Preset Names ===");
                                var presetCount = 0;
                                var presetDictionary = new Dictionary<String, String>();

                                JsonDocument innerDoc = null;
                                JsonElement? presetArray = null;
                                
                                try
                                {
                                    if (responseElement.ValueKind == JsonValueKind.Array)
                                    {
                                        presetArray = responseElement;
                                        PluginLog.Info($"Direct array with {presetArray.Value.GetArrayLength()} items");
                                    }
                                    else if (responseElement.ValueKind == JsonValueKind.String)
                                    {
                                        var arrayString = responseElement.GetString();
                                        PluginLog.Info($"String response: {arrayString?.Substring(0, Math.Min(100, arrayString?.Length ?? 0))}...");
                                        
                                        if (!String.IsNullOrEmpty(arrayString))
                                        {
                                            innerDoc = JsonDocument.Parse(arrayString);
                                            presetArray = innerDoc.RootElement;
                                            PluginLog.Info($"Parsed string to array with {presetArray.Value.GetArrayLength()} items");
                                        }
                                        else
                                        {
                                            PluginLog.Warning("Response is an empty string");
                                            return;
                                        }
                                    }
                                    else if (responseElement.ValueKind == JsonValueKind.Object)
                                    {
                                        PluginLog.Info("Response is an object, inspecting properties...");
                                        
                                        var foundArray = false;
                                        foreach (var property in responseElement.EnumerateObject())
                                        {
                                            PluginLog.Info($"Property: {property.Name}, Type: {property.Value.ValueKind}");
                                            
                                            if (property.Value.ValueKind == JsonValueKind.Array)
                                            {
                                                presetArray = property.Value;
                                                foundArray = true;
                                                PluginLog.Info($"Found array in property '{property.Name}' with {presetArray.Value.GetArrayLength()} items");
                                                break;
                                            }
                                        }
                                        
                                        if (!foundArray)
                                        {
                                            PluginLog.Warning("No array found in response object");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        PluginLog.Warning($"Unexpected response format: {responseElement.ValueKind}");
                                        return;
                                    }

                                    if (presetArray.HasValue && presetArray.Value.ValueKind == JsonValueKind.Array)
                                    {
                                        foreach (var element in presetArray.Value.EnumerateArray())
                                        {
                                            if (element.ValueKind == JsonValueKind.Array)
                                            {
                                                PluginLog.Info($"Found nested array with {element.GetArrayLength()} preset IDs");
                                                
                                                foreach (var presetId in element.EnumerateArray())
                                                {
                                                    var id = presetId.ValueKind == JsonValueKind.String 
                                                        ? presetId.GetString() 
                                                        : presetId.ToString();
                                                        
                                                    if (!String.IsNullOrEmpty(id))
                                                    {
                                                        PluginLog.Info($"Processing preset ID: {id}");
                                                        
                                                        var nameResponse = await LightroomPlugin.WebSocketClient.SendCommandWithResponseAsync(
                                                            "getPresetName", 
                                                            new Object[] { id }
                                                        );

                                                        if (!String.IsNullOrEmpty(nameResponse))
                                                        {
                                                            using (JsonDocument nameDoc = JsonDocument.Parse(nameResponse))
                                                            {
                                                                if (nameDoc.RootElement.TryGetProperty("response", out var nameElement))
                                                                {
                                                                    var presetName = nameElement.ValueKind == JsonValueKind.Array && nameElement.GetArrayLength() > 0
                                                                        ? nameElement[0].GetString()
                                                                        : (nameElement.ValueKind == JsonValueKind.String ? nameElement.GetString() : nameElement.ToString());

                                                                    PluginLog.Info($"Preset ID: {id} -> Name: {presetName}");
                                                                    presetDictionary[id] = presetName;
                                                                    presetCount++;
                                                                }
                                                                else
                                                                {
                                                                    PluginLog.Warning($"No 'response' property in name response for ID: {id}");
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            PluginLog.Warning($"Empty name response for ID: {id}");
                                                        }

                                                        await Task.Delay(50);
                                                    }
                                                }
                                            }
                                            else if (element.ValueKind == JsonValueKind.String)
                                            {
                                                var id = element.GetString();
                                                    
                                                if (!String.IsNullOrEmpty(id))
                                                {
                                                    PluginLog.Info($"Processing preset ID: {id}");
                                                    
                                                    var nameResponse = await LightroomPlugin.WebSocketClient.SendCommandWithResponseAsync(
                                                        "getPresetName", 
                                                        new Object[] { id }
                                                    );

                                                    if (!String.IsNullOrEmpty(nameResponse))
                                                    {
                                                        using (JsonDocument nameDoc = JsonDocument.Parse(nameResponse))
                                                        {
                                                            if (nameDoc.RootElement.TryGetProperty("response", out var nameElement))
                                                            {
                                                                var presetName = nameElement.ValueKind == JsonValueKind.Array && nameElement.GetArrayLength() > 0
                                                                    ? nameElement[0].GetString()
                                                                    : (nameElement.ValueKind == JsonValueKind.String ? nameElement.GetString() : nameElement.ToString());

                                                                PluginLog.Info($"Preset ID: {id} -> Name: {presetName}");
                                                                presetDictionary[id] = presetName;
                                                                presetCount++;
                                                            }
                                                            else
                                                            {
                                                                PluginLog.Warning($"No 'response' property in name response for ID: {id}");
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        PluginLog.Warning($"Empty name response for ID: {id}");
                                                    }

                                                    await Task.Delay(50);
                                                }
                                            }
                                        }

                                        PluginLog.Info($"=== Total Presets Found: {presetCount} ===");
                                        
                                        if (presetDictionary.Count > 0)
                                        {
                                            var plugin = this.Plugin as LightroomPlugin;
                                            if (plugin != null)
                                            {
                                                var pluginDataDirectory = plugin.GetPluginDataDirectory();
                                                if (IoHelpers.EnsureDirectoryExists(pluginDataDirectory))
                                                {
                                                    var filePath = Path.Combine(pluginDataDirectory, "presets.json");
                                                    
                                                    var jsonOptions = new JsonSerializerOptions
                                                    {
                                                        WriteIndented = true
                                                    };
                                                    
                                                    var jsonString = JsonSerializer.Serialize(presetDictionary, jsonOptions);
                                                    File.WriteAllText(filePath, jsonString);
                                                    
                                                    PluginLog.Info($"=== Presets saved to: {filePath} ===");
                                                }
                                                else
                                                {
                                                    PluginLog.Error("Failed to create plugin data directory");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        PluginLog.Warning($"Parsed element is not an array: {presetArray?.ValueKind.ToString() ?? "null"}");
                                    }
                                }
                                finally
                                {
                                    innerDoc?.Dispose();
                                }
                            }
                            else
                            {
                                PluginLog.Warning("No 'response' property found in root element");
                            }
                        }
                    }
                    else
                    {
                        PluginLog.Warning("No response received from Lightroom for getPresetIDs");
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error getting preset IDs: {ex.Message}");
                }
            });
        }
    }
}

