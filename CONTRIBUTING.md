## Table of Contents
- [Prepare Development Environment](#prepare-development-environment)
- [First Build](#first-build)

## Prepare Development Environment
- [Download and Install the Azure Kinect SDK and Firmware](./README.md#software)
- Download and Install [Visual Studio 2022](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&workload=dotnet-dotnetwebcloud&passive=false#dotnet)
- Download and Install [.NET Core](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-7.0.400-windows-x64-installer)
- Download and Install [Unity Hub](https://download.unity3d.com/download_unity/b16b3b16c7a0/Windows64EditorInstaller/UnitySetup64-2022.3.7f1.exe)

## First Build
On the first build you need to publish the NugetBridge project, it will output to OpenPool2/Assets/Plugins.
- Open a terminal window to the directory where you cloned the repo to
- dotnet publish NugetBridge -p:PublishProfile=UnityPrep

  
