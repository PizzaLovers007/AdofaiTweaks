#!/bin/bash
dotnet build -c Release
cd AdofaiTweaks.Generator/bin/Release/netstandard2.1
dotnet AdofaiTweaks.Generator.dll
cd ../../../..
version=$(cat VERSION.txt)
mkdir tmp
cd tmp
mkdir AdofaiTweaks
cp ../Info.json AdofaiTweaks
cp ../UnityProject/Assets/AssetBundles/adofai_tweaks.assets AdofaiTweaks
cp ../AdofaiTweaks/bin/Release/netstandard2.1/AdofaiTweaks.dll AdofaiTweaks
cp ~/.nuget/packages/litedb/5.0.10/lib/netstandard2.0/LiteDB.dll AdofaiTweaks
cp ../AdofaiTweaks.Translation/bin/Release/netstandard2.1/AdofaiTweaks.Translation.dll AdofaiTweaks
cp ../AdofaiTweaks.Generator/bin/Release/netstandard2.1/TweakStrings.db AdofaiTweaks
cp ../AdofaiTweaks.Generator/bin/Release/netstandard2.1/AdofaiTweaks.Strings.dll AdofaiTweaks
cp ../AdofaiTweaks.Compat.Async/bin/SkyHook/netstandard2.1/AdofaiTweaks.Compat.Async.dll AdofaiTweaks/AdofaiTweaks.Compat.AsyncSkyHook.dll
cp ../AdofaiTweaks.Compat.Async/bin/SharpHook/netstandard2.1/AdofaiTweaks.Compat.Async.dll AdofaiTweaks/AdofaiTweaks.Compat.AsyncSharpHook.dll
cp ../AdofaiTweaks.Compat.Async/bin/Release/netstandard2.1/AdofaiTweaks.Compat.Async.dll AdofaiTweaks/AdofaiTweaks.Compat.AsyncPolyfill.dll
zip -r AdofaiTweaks-$version.zip AdofaiTweaks
mv AdofaiTweaks-$version.zip ..
cd ..
rm -rf tmp
