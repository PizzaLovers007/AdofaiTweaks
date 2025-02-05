#!/usr/bin/env bash

# Run the downloader script
python3 -m venv myenv
source myenv/bin/activate

pip install google-api-python-client
pip install google-auth-oauthlib
pip install xlsxwriter

python3 downloader.py

deactivate

# Build the Translation project
dotnet build "../AdofaiTweaks.Translation/AdofaiTweaks.Translation.csproj" -c Debug

# Create output directory if it doesn't exist
mkdir -p "bin/Debug"

# Copy build artifacts
cp "../AdofaiTweaks.Translation/bin/Debug/AdofaiTweaks.Translation.dll" "bin/Debug/"
cp "translations.xlsx" "bin/Debug/"

# Build the Generator project
dotnet build "AdofaiTweaks.Generator.csproj" -c Debug

# Run the generator executable
mono "bin/Debug/AdofaiTweaks.Generator.exe"

# Copy the generated strings assembly
cp "AdofaiTweaks.Strings.dll" "bin/Debug/"
