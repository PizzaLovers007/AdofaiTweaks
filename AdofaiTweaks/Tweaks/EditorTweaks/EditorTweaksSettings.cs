using System.Xml.Serialization;
using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.EditorTweaks;

/// <summary>
/// Settings for the Editor Tweaks tweak.
/// </summary>
public class EditorTweaksSettings : TweakSettings
{
    /// <summary>
    /// Displays angle for selected floor if this option is on.
    /// </summary>
    public bool ShowFloorAngle { get; set; }

    /// <summary>
    /// Displays beats for selected floor if this option is on.
    /// </summary>
    public bool ShowFloorBeats { get; set; }

    /// <summary>
    /// Enables fine-tuned rotation for floors.
    /// </summary>
    public bool FineTuneFloorRotations { get; set; }

    /// <summary>
    /// Floor rotation step value.
    /// </summary>
    public float FloorRotationStep { get; set; } = 22.5f;

    /// <summary>
    /// Syncs level event values with floor flip & rotation operations.
    /// </summary>
    public bool SyncLevelEventValuesWithFloorFlipsAndRotations { get; set; }

    /// <summary>
    /// Whether the patch should display anything.
    /// </summary>
    [XmlIgnore]
    public bool ShouldShowAny => ShowFloorAngle || ShowFloorBeats;
}