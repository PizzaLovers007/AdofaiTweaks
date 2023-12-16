#!/bin/bash
dotnet build /p:Configuration=Release
cd AdofaiTweaks.Generator/bin/Release/net7.0
./AdofaiTweaks.Generator
cd ../../../..
version=$(cat VERSION.txt)
mkdir tmp
cd tmp
mkdir AdofaiTweaks
cp ../Info.json AdofaiTweaks
cp ../UnityProject/Assets/AssetBundles/adofai_tweaks.assets AdofaiTweaks
cp ../AdofaiTweaks/bin/Release/net7.0/AdofaiTweaks.dll AdofaiTweaks
cp ~/.nuget/packages/litedb/5.0.10/LiteDB.dll AdofaiTweaks
cp ../AdofaiTweaks.Translation/bin/Release/net7.0/AdofaiTweaks.Translation.dll AdofaiTweaks
cp ../AdofaiTweaks.Generator/bin/Release/net7.0/TweakStrings.db AdofaiTweaks
cp ../AdofaiTweaks.Generator/bin/Release/net7.0/AdofaiTweaks.Strings.dll AdofaiTweaks
cp ../AdofaiTweaks.Compat.Async/bin/SkyHook/net7.0/AdofaiTweaks.Compat.Async.dll AdofaiTweaks/AdofaiTweaks.Compat.AsyncSkyHook.dll
cp ../AdofaiTweaks.Compat.Async/bin/SharpHook/net7.0/AdofaiTweaks.Compat.Async.dll AdofaiTweaks/AdofaiTweaks.Compat.AsyncSharpHook.dll
cp ../AdofaiTweaks.Compat.Async/bin/Release/net7.0/AdofaiTweaks.Compat.Async.dll AdofaiTweaks/AdofaiTweaks.Compat.AsyncPolyfill.dll
zip -r AdofaiTweaks-$version.zip AdofaiTweaks
mv AdofaiTweaks-$version.zip ..
cd ..
rm -rf tmp
