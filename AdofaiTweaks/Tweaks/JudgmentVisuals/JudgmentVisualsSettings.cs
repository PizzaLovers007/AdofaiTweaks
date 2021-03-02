using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.JudgmentVisuals
{
    public class JudgmentVisualsSettings : TweakSettings
    {
        public bool ShowHitErrorMeter { get; set; }
        public float ErrorMeterScale { get; set; } = 1f;
        public float ErrorMeterTickLife { get; set; } = 4f;
        public float ErrorMeterSensitivity { get; set; } = 0.2f;
    }
}
