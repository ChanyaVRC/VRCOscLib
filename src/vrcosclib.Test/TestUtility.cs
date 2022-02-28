using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSoft.VRChat.Osc.Test;

public static class TestUtility
{
    public static readonly TimeSpan LatencyTimeout = TimeSpan.FromMilliseconds(100);
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
        await NewMethod(conditions).WaitAsync(timeout);
        static async Task NewMethod(Func<bool> conditions)
        {
            while (conditions())
            {
                await Task.Delay(5);
            }
        }
    }

    public static CancellationToken CanceledToken => CanceledTokenSource.Token;

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
        ""address"":""avatar/parameters/TestParam"",
        ""type"":""Int""
      }}
    }},
    {{
      ""name"":""VelocityZ"",
      ""output"":{{
        ""address"":""avatar/parameters/VelocityZ"",
        ""type"":""Float""
      }}
    }}
  ]
}}");
        return path;
    }
}
