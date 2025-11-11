namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    public class PerspectiveRotateAdjustment : ActionEditorAdjustment
    {
        private const String StepControlName = "StepValue";

        public PerspectiveRotateAdjustment() : base(hasReset: false)
        {
            this.GroupName = "Perspective";
            this.Name = "PerspectiveRotateAdjustment";
            this.DisplayName = "Perspective: Rotate";
            this.Description = "Adjust perspective rotate with knob rotation";

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
                    e.ActionEditorState.SetDisplayName($"Perspective: Rotate (Step: {stepValue:F2})");
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
                    SendRequest("increment", stepValue);
                }
            }
            else if (diff < 0)
            {
                for (int i = 0; i < Math.Abs(diff); i++)
                {
                    SendRequest("decrement", stepValue);
                }
            }
            return true;
        }

        private void SendRequest(String action, Double stepValue)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (action == "increment")
                    {
                        await LightroomPlugin.WebSocketClient.IncrementParameterAsync("PerspectiveRotate", stepValue);
                    }
                    else if (action == "decrement")
                    {
                        await LightroomPlugin.WebSocketClient.DecrementParameterAsync("PerspectiveRotate", stepValue);
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error sending perspective rotate {action} request: {ex.Message}");
                }
            });
        }
    }
}

