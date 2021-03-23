# Creating a basic tweak

This page will be a quick start guide to creating a simple tweak called `LogPlanet` that will log every time the planet is switched. Before looking through this guide, be sure to have UnityModManager (UMM) and AdofaiTweaks installed.

## Helpful docs

These docs are essential to ADOFAI mod development:

* [Harmony](https://harmony.pardeike.net/articles/intro.html) - The patching framework used by UnityModManager to run code before/after methods.
  * [Prefixing](https://harmony.pardeike.net/articles/patching-prefix.html), [Postfixing](https://harmony.pardeike.net/articles/patching-postfix.html), and [Injections](https://harmony.pardeike.net/articles/patching-injections.html) are docs that I constantly reference.
* [UnityModManager how-to guides](https://wiki.nexusmods.com/index.php/Category:Unity_Mod_Manager) - How-to guides provided by UnityModManager.
* [Unity docs](https://docs.unity3d.com/2019.3/Documentation/Manual/index.html) - Unity is the game engine that ADOFAI is built on, so it's very helpful to have a good understanding of how the engine works.

## Installing developer tools

First, you should install these tools:

* [dnSpy](https://github.com/dnSpy/dnSpy) - A C# code decompiler used to look at the ADOFAI game code. You won't need this for creating this basic tweak because all the code is here, but you will need to use this or something similar when creating your own tweak.
* [Visual Studio](https://visualstudio.microsoft.com/) - A C# IDE to develop the tweaks. This repo is setup with Visual Studio in mind, but feel free to use any other IDE.
* [Unity 2019.3](https://unity.com/releases/2019-3) - While not strictly required, having a test project can help you figure out how to create objects in the game world using code only. Debugging ADOFAI mods can be quite difficult, so I highly recommend installing this.

To view the code in Visual Studio, open up the `AdofaiTweaks.sln` solution file.

**There may be several errors when first opening up the solution.** This is because the projects are referencing the *local copies* of the game code and other assemblies from the default Steam installation directory. If you installed ADOFAI in a different location, you'll need to modify the `.csproj` files to point to your local copies. You can change it by right clicking on References then selecting "Add Reference... > Browse..." and navigating to your installation.

Before changing any code, try building the solution with `Ctrl + Shift + B`. Since there could be many things wrong here, please ask for help in the [ADOFAI Modding Discord](https://discord.gg/YfVKH4WtvP) if you cannot build the solution.

## Code overview

Let's get to coding!

The code for each tweak should be located in the `AdofaiTweaks/Tweaks` folder under its own separate directory. Generally, every tweak is separated into 3 different classes:

1. The `Patches` class. This class will hold several inner classes that will be used to patch methods using Harmony.
2. The `Settings` class. This class will hold all settings for the tweak and will be saved/loaded automatically.
3. The `Tweak` class. This class will run code in different parts of the tweak lifecycle (a separate page explaining the lifecycle will be uploaded soon).

Create a folder under `AdofaiTweaks/Tweaks` named `LogPlanet` and add these 3 files in it. I've included some comments explaining what each part of the code does.

### `LogPlanetSettings.cs`

```csharp
using AdofaiTweaks.Core;

namespace AdofaiTweaks.Tweaks.LogPlanet
{
    // The settings for the LogPlanet tweak. Any public properties will
    // automatically be saved/loaded. If you don't want a property to be saved,
    // you can add [XmlIgnore] to it.
    public class LogPlanetSettings : TweakSettings
    {
        // Properties can have default values. These are set when the user
        // starts the game with the mod installed for the first time, or in
        // future updates when new settings are added.
        public bool LogSwitching { get; set; } = true;
    }
}
```

### `LogPlanetPatches.cs`

```csharp
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;

namespace AdofaiTweaks.Tweaks.LogPlanet
{
    // Patches for the LogPlanet tweak. Each patch should be an inner static
    // class.
    internal static class LogPlanetPatches
    {
        // This property automatically holds a reference to the settings. Any
        // fields changed in the patches will be visible in the Tweak class.
        [SyncTweakSettings]
        private static LogPlanetSettings Settings { get; set; }

        // This is a patch for scrPlanet.SwitchChosen.
        [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
        private static class LogOnSwitchPatch
        {
            // Prefix methods run before the actual method is called.
            public static void Prefix(scrPlanet __instance) {
                if (Settings.LogSwitching) {
                    // __instance refers to the current instance, and
                    // SwitchChosen is called with the planet that is currently
                    // not rotating. The new current planet would be the
                    // opposite one.
                    string currPlanet = __instance.isRed ? "Blue" : "Red";
                    AdofaiTweaks.Logger.Log($"New current planet: {currPlanet}");
                }
            }
        }
    }
}
```

### `LogPlanetTweak.cs`

```csharp
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.LogPlanet
{
    // The main tweak class for the LogPlanet tweak. You can override several
    // lifecycle events here. This RegisterTweak attribute here registers the
    // tweak to AdofaiTweaks, making it show up in the settings GUI. It also
    // links the Settings and Patches class to the tweak.
    [RegisterTweak(
        id: "log_planet",
        settingsType: typeof(LogPlanetSettings),
        patchesType: typeof(LogPlanetPatches))]
    public class LogPlanetTweak : Tweak
    {
        // The name that will show up in the UMM settings window.
        public override string Name => "Log Planet";

        // The description that will show up in the UMM settings window.
        public override string Description => "Basic tweak that logs when the planets switch.";

        // This property automatically holds a reference to the settings. Any
        // fields changed in the patches will be visible in the Patches class.
        [SyncTweakSettings]
        private LogPlanetSettings Settings { get; set; }

        // Create the settings GUI here using both GUILayout and MoreGUILayout.
        public override void OnSettingsGUI() {
            Settings.LogSwitching =
                GUILayout.Toggle(Settings.LogSwitching, "Write log when planet switches");
        }
    }
}
```

## Testing the tweak

Build the tweak using `Ctrl + Shift + B`, then copy the `AdofaiTweaks.dll` file in the `AdofaiTweaks\bin\Release` folder to the AdofaiTweaks mod installation folder where you installed ADOFAI. Since I installed this in the default Steam location, the folder is at `C:\Program Files (x86)\Steam\steamapps\common\A Dance of Fire and Ice\Mods\AdofaiTweaks`.

(Note: The Visual Studio project has a post-build rule that automatically does this for you but will not work if your game installation directory is different.)

Once you have copied the file over, open ADOFAI and press `Ctrl + F10` to look at the settings. You should see it in the list of tweaks disabled.

![log_planet_settings_disabled](.\log_planet_settings_disabled.png)

Enabling it will show the "Write log..." toggle, which you should enable as well.

![log_planet_settings_enabled](.\log_planet_settings_enabled.png)

Then, close the menu and move around a bit. Open the menu back up to look at the logs, and you should see new log messages stating which planet you just switched to when moving.

![log_planet_logs](.\log_planet_logs.png)

## Conclusion

If you've made it to the end of this quick start guide, congratulations, you've made your first tweak!

There are a lot of things to consider when developing a tweak including translations, Unity objects, searching for the right methods to patch, and more. I'll be adding more pages to this wiki explaining some of the features I've added that you can utilize when creating your tweaks.

If you would like more assistance with developing your tweak (or any other mods in general), feel free to join the [ADOFAI Modding Discord](https://discord.gg/YfVKH4WtvP)! I and several other mod creators are active there and are willing to help you out.