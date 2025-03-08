---
_layout: landing
---

# The Documents for **VRCOscLib**.

## What is the **VRCOscLib**?

**VRCOscLib** is a **C# library** designed for **VRChat (VRC)** developers to simplify **OSC (Open Sound Control)** communication within their projects. This library streamlines integrating OSC into VRChat avatars or worlds, enabling features like triggering animations, syncing with everything, or controlling external tools.

## Quick Start Notes:

1. Create new C# Project.
2. Add NuGet package "VRCOscLib"
3. Done!

**!ATTENTION!: Enable OSC in VRChat!**

### Quick Start at The Console Project

1. Create Project
   ```bash
   dotnet new console
   dotnet add package vrcosclib
   ```
2. Edit `Program.cs`
   ```csharp
   using BuildSoft.VRChat.Osc.Chatbox;

   Console.WriteLine("The 'Hello World!' message is displayed in the chatbox.");
   OscChatbox.SendMessage("Hello World!", direct: true);
   ```
3. Run Program
   ```bash
   dotnet run
   ```
