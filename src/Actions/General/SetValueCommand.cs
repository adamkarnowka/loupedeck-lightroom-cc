namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    public class SetValueCommand : ActionEditorCommand
    {
        private const String ParameterControlName = "Parameter";
        private const String ValueControlName = "Value";

        public SetValueCommand()
        {
            this.Name = "SetValueCommand";
            this.DisplayName = "Set Value";
            this.GroupName = "General";
            this.Description = "Set a specific parameter to a specific value";

            this.ActionEditor.AddControlEx(
                new ActionEditorListbox(ParameterControlName, "Parameter:")
                    .SetRequired());

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(ValueControlName, "Value:")
                    .SetRequired()
                    .SetPlaceholder("0.0"));

            this.ActionEditor.ListboxItemsRequested += this.OnListboxItemsRequested;
            this.ActionEditor.ControlValueChanged += this.OnControlValueChanged;
        }

        private void OnListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
        {
            if (e.ControlName.EqualsNoCase(ParameterControlName))
            {
                e.AddItem("Exposure2012", "Light: Exposure", "Adjust overall brightness");
                e.AddItem("Whites2012", "Light: Whites", "Adjust white tones");
                e.AddItem("Highlights2012", "Light: Highlights", "Adjust bright areas");
                e.AddItem("Shadows2012", "Light: Shadows", "Adjust dark areas");
                e.AddItem("Blacks2012", "Light: Blacks", "Adjust black tones");
                e.AddItem("Contrast2012", "Light: Contrast", "Adjust tonal contrast");
                
                e.AddItem("Temperature", "Tint: Temperature", "Adjust color temperature");
                e.AddItem("Tint", "Tint: Tint", "Adjust green-magenta balance");
                e.AddItem("Vibrance", "Tint: Vibrance", "Adjust subtle saturation");
                e.AddItem("Saturation", "Tint: Saturation", "Adjust overall color intensity");
                
                e.AddItem("Texture", "Effects: Texture", "Adjust fine detail");
                e.AddItem("Clarity2012", "Effects: Clarity", "Adjust midtone contrast");
                e.AddItem("Dehaze", "Effects: Dehaze", "Remove atmospheric haze");
                
                e.AddItem("LuminanceSmoothing", "Detail: Luminance Smoothing", "Reduce luminance noise");
                e.AddItem("ColorNoiseReduction", "Detail: Color Noise Reduction", "Remove color noise");
                e.AddItem("Sharpness", "Detail: Sharpness", "Adjust image sharpness");
                
                e.AddItem("PerspectiveRotate", "Perspective: Rotate", "Rotate perspective");
                e.AddItem("PerspectiveAspect", "Perspective: Aspect", "Adjust aspect ratio");
                e.AddItem("PerspectiveScale", "Perspective: Scale", "Scale perspective");
                e.AddItem("PerspectiveVertical", "Perspective: Vertical", "Vertical perspective");
                e.AddItem("PerspectiveHorizontal", "Perspective: Horizontal", "Horizontal perspective");
                e.AddItem("PerspectiveX", "Perspective: X Offset", "X position offset");
                e.AddItem("PerspectiveY", "Perspective: Y Offset", "Y position offset");
                e.AddItem("PerspectiveUpright", "Perspective: Upright", "Auto upright correction");
                
                e.AddItem("AutoLateralCA", "Optics: Chromatic Aberration", "Remove chromatic aberration (0 or 1)");
                e.AddItem("LensProfileEnable", "Optics: Lens Corrections", "Enable lens corrections (0 or 1)");
                
                e.SetSelectedItemName("Exposure2012");
            }
        }

        private void OnControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
        {
            var parameter = e.ActionEditorState.GetControlValue(ParameterControlName);
            var value = e.ActionEditorState.GetControlValue(ValueControlName);
            
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(value))
            {
                e.ActionEditorState.SetDisplayName($"Set {parameter}: {value}");
            }
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            try
            {
                if (!actionParameters.TryGetString(ParameterControlName, out var parameter) || String.IsNullOrWhiteSpace(parameter))
                {
                    PluginLog.Error("Parameter not provided.");
                    return false;
                }

                if (!actionParameters.TryGetString(ValueControlName, out var valueString) || String.IsNullOrWhiteSpace(valueString))
                {
                    PluginLog.Error("Value not provided.");
                    return false;
                }

                if (!Double.TryParse(valueString, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                {
                    PluginLog.Error($"Invalid value: {valueString}");
                    return false;
                }

                Task.Run(async () =>
                {
                    try
                    {
                        await LightroomPlugin.WebSocketClient.SetValueAsync(parameter, value);
                        PluginLog.Info($"Successfully set {parameter} to {value}");
                    }
                    catch (Exception ex)
                    {
                        PluginLog.Error($"Error setting {parameter} to {value}: {ex.Message}");
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error executing Set Value command: {ex.Message}");
                return false;
            }
        }
    }
}

