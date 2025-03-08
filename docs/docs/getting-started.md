# Getting Started

## Installation

### Via CLI

1. Create new project
   ```bash
   dotnet new console
   ```
2. Add the VRCOscLib package to your project:
   ```bash
   dotnet add package vrcosclib
   ```
3. For example, write a simple code like this to test the library:
   ```csharp
   using BuildSoft.VRChat.Osc.Chatbox;

   Console.WriteLine("Sending message to VRChat chatbox...");
   OscChatbox.SendMessage("Hello from VRCOscLib!", direct: true);
   ```
4. Build your project to ensure the package is properly referenced:
   ```bash
   dotnet build
   ```

### Via Visual Studio (NuGet Package Manager)

1. Open your project in Visual Studio
2. Right-click on your project in Solution Explorer
3. Select "Manage NuGet Packages"
4. Search for "VRCOscLib"
5. Click "Install"

## Enabling OSC in VRChat
To use this library with VRChat, you need to enable OSC in the VRChat settings.
If you are distributing your application to end users, please inform them that OSC must be enabled in VRChat for it to work properly.

Please refer to the [Official Document](https://docs.vrchat.com/docs/osc-overview#enabling-it) for instructions on how to enable it.
