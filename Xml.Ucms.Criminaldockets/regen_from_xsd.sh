#!/bin/bash

# using this tool
# https://www.nuget.org/packages/dotnet-xscgen/
# vscode might have already installed it for us with config
# tldr: `dotnet tool install --global dotnet-xscgen --version 2.0.633` and logout/in

dotnet tool run xscgen UCE-UCMSCriminalDockets.xsd --compactTypeNames --netCore --uniqueTypeNames --pascal --nullable
