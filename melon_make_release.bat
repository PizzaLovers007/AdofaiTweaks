msbuild /p:Configuration=Release
cd AdofaiTweaks.Generator/bin/Release
.\AdofaiTweaks.Generator.exe
cd ../../..
set /p version=<VERSION.txt
mkdir tmp
cp UnityProject/Assets/AssetBundles/adofai_tweaks.assets tmp
cp AdofaiTweaks/bin/Release/AdofaiTweaks.dll tmp
cp AdofaiTweaks/bin/Release/DotNetZip.dll tmp
cp AdofaiTweaks/bin/Release/LiteDB.dll tmp
cp AdofaiTweaks.Translation/bin/Release/AdofaiTweaks.Translation.dll tmp
cp AdofaiTweaks.Generator/bin/Release/TweakStrings.db tmp
cp AdofaiTweaks.Generator/bin/Release/AdofaiTweaks.Strings.dll tmp
cd tmp
tar --exclude=AdofaiTweaks-ML-%version%.zip -a -c -f AdofaiTweaks-ML-%version%.zip *
mv AdofaiTweaks-ML-%version%.zip ..
cd ..
rm -rf tmp
pause