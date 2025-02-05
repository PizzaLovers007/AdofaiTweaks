msbuild /p:Configuration=Release -m
cd AdofaiTweaks.Generator\bin\Release
.\AdofaiTweaks.Generator.exe
cd ..\..\..
set /p version=<VERSION.txt
mkdir tmp
cd tmp
mkdir AdofaiTweaks
copy ..\Info.json AdofaiTweaks
copy ..\UnityProject\Assets\AssetBundles\adofai_tweaks.assets AdofaiTweaks
copy ..\AdofaiTweaks\bin\Release\AdofaiTweaks.dll AdofaiTweaks
copy ..\AdofaiTweaks\bin\Release\LiteDB.dll AdofaiTweaks
copy ..\AdofaiTweaks\bin\Release\System.Buffers.dll AdofaiTweaks
copy ..\AdofaiTweaks\bin\Release\IndexRange.dll AdofaiTweaks
copy ..\AdofaiTweaks.Translation\bin\Release\AdofaiTweaks.Translation.dll AdofaiTweaks
copy ..\AdofaiTweaks.Generator\bin\Release\TweakStrings.db AdofaiTweaks
copy ..\AdofaiTweaks.Generator\bin\Release\AdofaiTweaks.Strings.dll AdofaiTweaks
copy ..\AdofaiTweaks.Compat.Async\bin\SkyHook\AdofaiTweaks.Compat.Async.dll AdofaiTweaks\AdofaiTweaks.Compat.AsyncSkyHook.dll
copy ..\AdofaiTweaks.Compat.Async\bin\SharpHook\AdofaiTweaks.Compat.Async.dll AdofaiTweaks\AdofaiTweaks.Compat.AsyncSharpHook.dll
copy ..\AdofaiTweaks.Compat.Async\bin\Release\AdofaiTweaks.Compat.Async.dll AdofaiTweaks\AdofaiTweaks.Compat.AsyncPolyfill.dll
tar -a -c -f AdofaiTweaks-%version%.zip AdofaiTweaks
move AdofaiTweaks-%version%.zip ..
cd ..
rmdir /s /q tmp
pause