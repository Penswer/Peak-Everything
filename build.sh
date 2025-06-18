#! /bin/bash
dotnet build -c Release
cp ./bin/Release/net4.7.1/Everything.dll /home/penswer/.local/share/Steam/steamapps/common/PEAK/BepInEx/plugins/
