using System;
using Xamarin.Forms;

namespace SeaFight.Triggers
{
    public class HeightEqualWidthTriggerAction : TriggerAction<VisualElement>
    {
        public double Accuracy { get; set; } = 0.05;

        public HeightEqualWidthTriggerAction() { }
        public HeightEqualWidthTriggerAction(double accuracy) { Accuracy = accuracy; }

        protected override void Invoke(VisualElement sender)
        {
            if (Math.Abs(sender.Width - sender.Height) < Accuracy) return;

            sender.HeightRequest = sender.Width;
        }
    }
}
