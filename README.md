<h1 align="center">VRCOscLib</h1>
<h2 align="center">The OSC library for VRChat (.NET Standard)</h2>
<p align="center"><a href="https://discord.gg/u2BE6W5NMy"><img alt="Discord" src="https://img.shields.io/badge/Discord-Buildsoft%27s%20Discord%20Server-blue?logo=discord"></a></p>

**VRCOscLib is now released!**

## How to Install

Download & import the .nupkg from the [Releases](https://github.com/ChanyaVRC/VRCOscLib/releases) page.

I can also download from [NuGet Package Manager](https://docs.microsoft.com/nuget/quickstart/install-and-use-a-package-in-visual-studio). See [nuget.org](https://www.nuget.org/packages/VRCOscLib/) for the NuGet package latest version.


## Usage in code
Please check the `Sample` for the combination with the actual application.

### About avatar parameters
If you want to control avatar parameters, use classes in `BuildSoft.VRChat.Osc.Avatar`.

#### e.g.) Get and use avatar config model

```cs
using System;
using BuildSoft.VRChat.Osc.Avatar;

// get avatar config by avatar id.
var config = OscAvatarConfig.Create("avtr_XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX")!;

foreach (var parameter in config.Parameters.Items)
{
    Console.WriteLine($"{parameter.Name}: " +
        $"input {(parameter.Input != null ? "○" : "×")}, " +
        $"output {(parameter.Output != null ? "○" : "×")}"
    );
}
```

#### e.g.) Get current avatar config model

```cs
using System;
using BuildSoft.VRChat.Osc.Avatar;

var config = OscAvatarConfig.CreateAtCurrent();
if (config == null) {
    Console.WriteLine("Failed to get the current avatar, Do \"Reset Avatar\" or start VRChat.");
}

// Wait until you can get an avatar config.
config = await OscAvatarConfig.WaitAndCreateAtCurrentAsync();
```

#### e.g.) Send avatar parameter

```cs
using BuildSoft.VRChat.Osc.Avatar;

var config = await OscAvatarConfig.WaitAndCreateAtCurrentAsync();

config.Parameters["IntParameterName"] = 1;
config.Parameters["FloatParameterName"] = 12.2f;
config.Parameters["BoolParameterName"] = true;
```

or

```cs
using BuildSoft.VRChat.Osc;

OscParameter.SendAvatarParameter("IntParameterName", 1);
OscParameter.SendAvatarParameter("FloatParameterName", 12.3f);
OscParameter.SendAvatarParameter("BoolParameterName", true);
```

#### e.g.) Get received avatar parameter

```cs
using System;
using System.Threading.Tasks;
using BuildSoft.VRChat.Osc.Avatar;

var config = await OscAvatarConfig.WaitAndCreateAtCurrentAsync();

await Task.Delay(1000);

Console.WriteLine($"parameterName = {config.Parameters["parameterName"]}");
```

### About button input
If you want to use button input with OSC, use classes in `BuildSoft.VRChat.Osc.Input`.  
There are 2 kinds of input, `OscAxisInput` can send a float value, and `OscButtonInput` can send a boolean value, press or release.

#### e.g.) Send Input

```cs
using System.Threading.Tasks;
using BuildSoft.VRChat.Osc.Input;

OscAxisInput.LookVertical.Send(0.2f);
await Task.Delay(1000);
OscAxisInput.LookVertical.Send(0f);

OscButtonInput.Jump.Press();
await Task.Delay(1000);
OscButtonInput.Jump.Release();
```

### About Chatbox
If you want to use button input with OSC, use classes in `BuildSoft.VRChat.Osc.Chatbox`.  

#### e.g.) Send a string to Chatbox
```cs
using BuildSoft.VRChat.Osc.Chatbox;

OscChatbox.SendMessage("some message", direct: true);
```

#### e.g.) Send a string to Chatbox UI
```cs
using BuildSoft.VRChat.Osc.Chatbox;

OscChatbox.SendMessage("some message", direct: false);
```

#### e.g.) Act like typing
```cs
using BuildSoft.VRChat.Osc.Chatbox;
using System.Threading.Tasks;

// typing during 3 second.
OscChatbox.SetIsTyping(true);
await Task.Delay(3000);

OscChatbox.SendMessage("some message", direct: true);
```

### How to change an avatar
Can be changed an avatar by one of the following methods.
* use `OscAvatarUtility.ChangeAvatar(id)`
* use `OscAvatarConfig.Change()`
* use `OscAvatar.Change()`

#### e.g) Change avatar
```cs
using BuildSoft.VRChat.Osc.Avatar;

OscAvatarUtility.ChangeAvatar("avtr_XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX");
```
or
```cs
using BuildSoft.VRChat.Osc.Chatbox;
using System.Threading.Tasks;

var avatar = await OscAvatarConfig.WaitAndCreateAtCurrentAsync();

await Task.Delay(1000);

avatar.Change();
```


<!-- ## Contact
* Twitter: [@ChanyaVRChat1](https://twitter.com/ChanyaVRChat1)
* Discord: [Chanya#0386](https://discord.com/channels/@me/924538047373115392) -->
