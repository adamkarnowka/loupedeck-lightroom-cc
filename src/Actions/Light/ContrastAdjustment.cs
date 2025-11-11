namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;


    public class ContrastAdjustment : ActionEditorAdjustment
    {
        private const String StepControlName = "StepValue";

        public ContrastAdjustment() : base(hasReset: false)
        {
            this.GroupName = "Light";
            this.Name = "ContrastAdjustment";
            this.DisplayName = "Light: Contrast";
            this.Description = "Adjust contrast with knob rotation";

            this.ActionEditor.AddControlEx(
                new ActionEditorSlider(name: StepControlName, labelText: "Step Value:", description: "Adjustment step size")
                    .SetValues(minimumValue: 0.01, maximumValue: 5.0, defaultValue: 0.1, step: 0.01));

            this.ActionEditor.ControlValueChanged += this.OnControlValueChanged;
        }

        private void OnControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
        {
            if (e.ControlName.EqualsNoCase(StepControlName))
            {
                var controlValue = e.ActionEditorState.GetControlValue(StepControlName);
                if (Double.TryParse(controlValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var stepValue))
                {
                    e.ActionEditorState.SetDisplayName($"Light: Contrast (Step: {stepValue:F2})");
                }
            }
        }

        protected override Boolean ApplyAdjustment(ActionEditorActionParameters actionParameters, Int32 diff)
        {
            var stepValue = 0.1;
            if (actionParameters.TryGetString(StepControlName, out var stepString))
            {
                if (!Double.TryParse(stepString, NumberStyles.Float, CultureInfo.InvariantCulture, out stepValue))
                {
                    stepValue = 0.1;
                }
            }

            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    SendContrastRequest("increment", stepValue);
                }
            }
            else if (diff < 0)
            {
                for (int i = 0; i < Math.Abs(diff); i++)
                {
                    SendContrastRequest("decrement", stepValue);
                }
            }

            return true;
        }

        private void SendContrastRequest(String action, Double stepValue)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (action == "increment")
                    {
                        await LightroomPlugin.WebSocketClient.IncrementParameterAsync("Contrast2012", stepValue);
                    }
                    else if (action == "decrement")
                    {
                        await LightroomPlugin.WebSocketClient.DecrementParameterAsync("Contrast2012", stepValue);
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error sending contrast {action} request: {ex.Message}");
                }
            });
        }
    }
}

