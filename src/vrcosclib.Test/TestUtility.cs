using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSoft.VRChat.Osc.Test;

public static class TestUtility
{
    public static readonly TimeSpan LatencyTimeout = TimeSpan.FromMilliseconds(2000);
    private static CancellationTokenSource? _canceledTokenSource;
    private static CancellationTokenSource CanceledTokenSource
    {
        get
        {
            if (_canceledTokenSource == null)
            {
                var source = new CancellationTokenSource();
                source.Cancel();
                _canceledTokenSource = source;
            }
            return _canceledTokenSource;
        }
    }

    public static async Task LoopWhile(Func<bool> conditions, TimeSpan timeout)
    {
        await WaitWhile(conditions).WaitAsync(timeout);
        static async Task WaitWhile(Func<bool> conditions)
        {
            while (conditions())
            {
                await Task.Delay(0);
            }
        }
    }

    public static CancellationToken CanceledToken => CanceledTokenSource.Token;

    public static string GetAvatarConfigDirectory(string userId = "usr_test_user_id")
    {
        return Path.Combine(OscUtility.VRChatOscPath, userId, "Avatars");
    }

    public static string CreateConfigFileForTest(string avatarId, string name, string directory, bool empty = false)
    {
        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, avatarId + ".json");
        using var writer = File.CreateText(path);
        if (empty)
        {
            return path;
        }

        writer.Write(
@$"{{
  ""id"":""{avatarId}"",
  ""name"":""{name}"",
  ""parameters"":[
    {{
      ""name"":""TestParam"",
      ""input"":{{
        ""address"":""/avatar/parameters/TestParam"",
        ""type"":""Int""
      }},
      ""output"":{{
        ""address"":""/avatar/parameters/TestParam"",
        ""type"":""Int""
      }}
    }},
    {{
      ""name"":""PhysBoneParam_IsGrabbed"",
      ""input"":{{
        ""address"":""/avatar/parameters/PhysBoneParam_IsGrabbed"",
        ""type"":""Bool""
      }},
      ""output"":{{
        ""address"":""/avatar/parameters/PhysBoneParam_IsGrabbed"",
        ""type"":""Bool""
      }}
    }},
    {{
      ""name"":""PhysBoneParam_Angle"",
      ""input"":{{
        ""address"":""/avatar/parameters/PhysBoneParam_Angle"",
        ""type"":""Float""
      }},
      ""output"":{{
        ""address"":""/avatar/parameters/PhysBoneParam_Angle"",
        ""type"":""Float""
      }}
    }},
    {{
      ""name"":""PhysBoneParam_Stretch"",
      ""input"":{{
        ""address"":""/avatar/parameters/PhysBoneParam_Stretch"",
        ""type"":""Float""
      }},
      ""output"":{{
        ""address"":""/avatar/parameters/PhysBoneParam_Stretch"",
        ""type"":""Float""
      }}
    }},
    {{
      ""name"":""VelocityZ"",
      ""output"":{{
        ""address"":""/avatar/parameters/VelocityZ"",
        ""type"":""Float""
      }}
    }}
  ]
}}");
        return path;
    }
}
