@echo off
py downloader.py

REM set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v2.0.50727
REM set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v3.5
REM set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319
REM set msBuildDir=C:\Program Files (x86)\MSBuild\12.0\Bin
REM set msBuildDir=C:\Program Files (x86)\MSBuild\14.0\Bin
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

call "%msBuildDir%\MSBuild.exe" "..\AdofaiTweaks.Translation\AdofaiTweaks.Translation.csproj" /p:Configuration=Debug /l:FileLogger,Microsoft.Build.Engine;logfile=Manual_MSBuild_ReleaseVersion_LOG.log
copy "..\AdofaiTweaks.Translation\bin\Debug\AdofaiTweaks.Translation.dll" "bin\Debug\"
copy "translations.xlsx" "bin\Debug\"

call "%msBuildDir%\MSBuild.exe" "AdofaiTweaks.Generator.csproj" /p:Configuration=Debug /l:FileLogger,Microsoft.Build.Engine;logfile=Manual_MSBuild_ReleaseVersion_LOG.log

"bin\Debug\AdofaiTweaks.Generator.exe"

copy "AdofaiTweaks.Strings.dll" "bin\Debug\"

set msBuildDir=