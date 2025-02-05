#!/usr/bin/env bash

dotnet build -c SharpHook
dotnet build -c SkyHook

dotnet build -c Release
cd ./AdofaiTweaks.Generator/bin/Release
mono AdofaiTweaks.Generator.exe

cd ../../..
version=$(<VERSION.txt)
mkdir -p tmp/AdofaiTweaks
cd ./tmp/AdofaiTweaks
cp ../../Info.json .
cp ../../UnityProject/Assets/AssetBundles/adofai_tweaks.assets .
cp ../../AdofaiTweaks/bin/Release/AdofaiTweaks.dll .
cp ../../AdofaiTweaks/bin/Release/LiteDB.dll .
cp ../../AdofaiTweaks/bin/Release/System.Buffers.dll .
cp ../../AdofaiTweaks/bin/Release/IndexRange.dll .
cp ../../AdofaiTweaks.Translation/bin/Release/AdofaiTweaks.Translation.dll .
cp ../../AdofaiTweaks.Generator/bin/Release/TweakStrings.db .
cp ../../AdofaiTweaks.Generator/bin/Release/AdofaiTweaks.Strings.dll .
cp ../../AdofaiTweaks.Compat.Async/bin/SkyHook/AdofaiTweaks.Compat.Async.dll AdofaiTweaks.Compat.AsyncSkyHook.dll
cp ../../AdofaiTweaks.Compat.Async/bin/SharpHook/AdofaiTweaks.Compat.Async.dll AdofaiTweaks.Compat.AsyncSharpHook.dll
cp ../../AdofaiTweaks.Compat.Async/bin/Release/AdofaiTweaks.Compat.Async.dll AdofaiTweaks.Compat.AsyncPolyfill.dll
cd ..
zip -r AdofaiTweaks-$version.zip AdofaiTweaks
mv AdofaiTweaks-$version.zip ..
cd ..
rm -rf tmp