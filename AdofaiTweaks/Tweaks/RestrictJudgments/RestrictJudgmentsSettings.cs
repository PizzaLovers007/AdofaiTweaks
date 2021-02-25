using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.RestrictJudgments
{
    public class RestrictJudgmentsSettings : TweakSettings
    {
        public bool[] RestrictJudgments { get; set; }
        public string CustomDeathString { get; set; } = "No {judgment}s allowed!";
    }
}
