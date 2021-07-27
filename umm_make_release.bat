set config=Release
msbuild /p:Configuration=%config%
cd AdofaiTweaks.Generator/bin/%config%
.\AdofaiTweaks.Generator.exe
cd ../../..
set /p version=<VERSION.txt
mkdir tmp
cd tmp
mkdir AdofaiTweaks
cp ../Info.json AdofaiTweaks
cp ../UnityProject/Assets/AssetBundles/adofai_tweaks.assets AdofaiTweaks
cp ../AdofaiTweaks/bin/%config%/AdofaiTweaks.dll AdofaiTweaks
cp ../AdofaiTweaks/bin/%config%/DotNetZip.dll AdofaiTweaks
cp ../AdofaiTweaks/bin/%config%/LiteDB.dll AdofaiTweaks
cp ../AdofaiTweaks.Translation/bin/%config%/AdofaiTweaks.Translation.dll AdofaiTweaks
cp ../AdofaiTweaks.Generator/bin/%config%/TweakStrings.db AdofaiTweaks
cp ../AdofaiTweaks.Generator/bin/%config%/AdofaiTweaks.Strings.dll AdofaiTweaks
tar -a -c -f AdofaiTweaks-%version%.zip AdofaiTweaks
mv AdofaiTweaks-UMM-%version%.zip ..
cd ..
rm -rf tmp
pause