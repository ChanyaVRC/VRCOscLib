using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace BuildSoft.VRChat.Osc.Test.Utility;

public static class TestHelper
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

    public static async Task WaitWhile(Func<bool> conditions, TimeSpan timeout)
    {
        await Task.Run(() => { while (conditions()) ; }).WaitAsync(timeout);
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
        var configJson = JsonConvert.SerializeObject(
            new OscAvatarConfigJson(avatarId, name, [
                new("TestParam",                OscType.Int,   hasInput: true),
                new("PhysBoneParam_IsGrabbed",  OscType.Bool,  hasInput: true),
                new("PhysBoneParam_IsPosed",    OscType.Bool,  hasInput: true),
                new("PhysBoneParam_Angle",      OscType.Float, hasInput: true),
                new("PhysBoneParam_Stretch",    OscType.Float, hasInput: true),
                new("PhysBoneParam_Squish",     OscType.Float, hasInput: true),
                new("PhysBoneParam__IsGrabbed", OscType.Float, hasInput: true),
                new("PhysBoneParam__Angle",     OscType.Float, hasInput: true),
                new("PhysBoneParam__Stretch",   OscType.Bool,  hasInput: true),
                new("VelocityZ",                OscType.Float, hasInput: false),
            ]));
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


    static readonly string _destinationDirName = OscUtility.VRChatOscPath + "_Renamed";

    public static void StashOscDirectory()
    {
        Directory.CreateDirectory(OscUtility.VRChatOscPath);
        Directory.Move(OscUtility.VRChatOscPath, _destinationDirName);
        Directory.CreateDirectory(OscUtility.VRChatOscPath);
    }

    public static void RestoreOscDirectory()
    {
        if (Directory.Exists(_destinationDirName))
        {
            Directory.Delete(OscUtility.VRChatOscPath, true);
            Directory.Move(_destinationDirName, OscUtility.VRChatOscPath);
        }
    }

#if !NET6_0_OR_GREATER
    public static async Task WaitAsync(this Task task, TimeSpan timeout)
    {
        var source = CreateCancellationTokenSourceWithDelay(timeout);

        await Task.Run(() =>
        {
            while (true)
            {
                if (source.IsCancellationRequested) { throw new TimeoutException(); }
                if (task.IsCompleted) { break; }
            }
        });
        if (task.IsFaulted)
        {
            throw task.Exception!.InnerException!;
        }
    }

    public static async Task<T> WaitAsync<T>(this Task<T> task, TimeSpan timeout)
    {
        await ((Task)task).WaitAsync(timeout);
        return task.Result;
    }

    private static CancellationTokenSource CreateCancellationTokenSourceWithDelay(TimeSpan delay)
    {
        CancellationTokenSource source = new(delay);
        if (delay <= TimeSpan.Zero)
        {
            source.Cancel();
        }
        return source;
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum[] GetValues<TEnum>() where TEnum: struct, Enum
    {
#if NET8_0_OR_GREATER
        return Enum.GetValues<TEnum>();
#else
        return (TEnum[])Enum.GetValues(typeof(TEnum));
#endif
    }
}
