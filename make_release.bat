msbuild /p:Configuration=Release
cd AdofaiTweaks.Generator/bin/Release
.\AdofaiTweaks.Generator.exe
cd ../../..
set /p version=<VERSION.txt
mkdir tmp
cd tmp
mkdir AdofaiTweaks
cp ../Info.json AdofaiTweaks
cp ../UnityProject/Assets/AssetBundles/adofai_tweaks.assets AdofaiTweaks
cp ../AdofaiTweaks/bin/Release/AdofaiTweaks.dll AdofaiTweaks
cp ../AdofaiTweaks/bin/Release/LiteDB.dll AdofaiTweaks
cp ../AdofaiTweaks.Translation/bin/Release/AdofaiTweaks.Translation.dll AdofaiTweaks
cp ../AdofaiTweaks.Generator/bin/Release/TweakStrings.db AdofaiTweaks
cp ../AdofaiTweaks.Generator/bin/Release/AdofaiTweaks.Strings.dll AdofaiTweaks
tar -a -c -f AdofaiTweaks-%version%.zip AdofaiTweaks
mv AdofaiTweaks-%version%.zip ..
cd ..
rm -rf tmp
pause