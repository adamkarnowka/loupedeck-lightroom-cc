namespace Loupedeck.LightroomPlugin
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;


    public class BlacksAdjustment : ActionEditorAdjustment
    {
        private const String StepControlName = "StepValue";

        public BlacksAdjustment() : base(hasReset: false)
        {
            this.GroupName = "Light";
            this.Name = "BlacksAdjustment";
            this.DisplayName = "Light: Blacks";
            this.Description = "Adjust blacks with knob rotation";

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
                    e.ActionEditorState.SetDisplayName($"Light: Blacks (Step: {stepValue:F2})");
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
                    SendBlacksRequest("increment", stepValue);
                }
            }
            else if (diff < 0)
            {
                for (int i = 0; i < Math.Abs(diff); i++)
                {
                    SendBlacksRequest("decrement", stepValue);
                }
            }

            return true;
        }

        private void SendBlacksRequest(String action, Double stepValue)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (action == "increment")
                    {
                        await LightroomPlugin.WebSocketClient.IncrementParameterAsync("Blacks2012", stepValue);
                    }
                    else if (action == "decrement")
                    {
                        await LightroomPlugin.WebSocketClient.DecrementParameterAsync("Blacks2012", stepValue);
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error sending blacks {action} request: {ex.Message}");
                }
            });
        }
    }
}

