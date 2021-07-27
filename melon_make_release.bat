set config=Release
msbuild /p:Configuration=%config%
cd AdofaiTweaks.Generator/bin/%config%
.\AdofaiTweaks.Generator.exe
cd ../../..
set /p version=<VERSION.txt
mkdir tmp
cp UnityProject/Assets/AssetBundles/adofai_tweaks.assets tmp
cp AdofaiTweaks/bin/%config%/AdofaiTweaks.dll tmp
cp AdofaiTweaks/bin/%config%/DotNetZip.dll tmp
cp AdofaiTweaks/bin/%config%/LiteDB.dll tmp
cp AdofaiTweaks.Translation/bin/%config%/AdofaiTweaks.Translation.dll tmp
cp AdofaiTweaks.Generator/bin/%config%/TweakStrings.db tmp
cp AdofaiTweaks.Generator/bin/%config%/AdofaiTweaks.Strings.dll tmp
cd tmp
tar --exclude=AdofaiTweaks-ML-%version%.zip -a -c -f AdofaiTweaks-ML-%version%.zip *
mv AdofaiTweaks-ML-%version%.zip ..
cd ..
rm -rf tmp
pause