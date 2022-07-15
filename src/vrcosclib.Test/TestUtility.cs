using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

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
                await Task.Delay(1);
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
        string configJson = JsonConvert.SerializeObject(
            new OscAvatarConfigJson(avatarId, name, new OscAvatarParameterJson[] {
                new("TestParam",                "Int",   hasInput: true),
                new("PhysBoneParam_IsGrabbed",  "Bool",  hasInput: true),
                new("PhysBoneParam_Angle",      "Float", hasInput: true),
                new("PhysBoneParam_Stretch",    "Float", hasInput: true),
                new("PhysBoneParam__IsGrabbed", "Float", hasInput: true),
                new("PhysBoneParam__Angle",     "Float", hasInput: true),
                new("PhysBoneParam__Stretch",   "Bool",  hasInput: true),
                new("VelocityZ",                "Float", hasInput: false),
            }));
        writer.Write(configJson);
        return path;
    }

    public static string CreateConfigFileForTest(OscAvatarConfigJson avatar, string directory)
    {
        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, avatar.id + ".json");

        using var writer = File.CreateText(path);
        writer.Write(JsonConvert.SerializeObject(avatar));

        return path;
    }
}
