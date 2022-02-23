using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BlobHandles;
using BuildSoft.OscCore;

namespace BuildSoft.VRChat.Osc;

public class OscAvatarUtility
{
    internal static readonly Dictionary<string, object?> _commonParameters = new()
    {
        { "VRCFaceBlendV", null },
        { "VRCFaceBlendH", null },
        { "VRCEmote", null },
        { "VelocityY", null },
        { "VelocityX", null },
        { "VelocityZ", null },
        { "InStation", null },
        { "Seated", null },
        { "AFK", null },
        { "Upright", null },
        { "AngularY", null },
        { "Grounded", null },
        { "MuteSelf", null },
        { "VRMode", null },
        { "TrackingType", null },
        { "GestureRightWeight", null },
        { "GestureRight", null },
        { "GestureLeftWeight", null },
        { "GestureLeft", null },
        { "Voice", null },
        { "Viseme", null },
    };

    internal static List<WeakReference<OscAvatarConfig>> _avatarConfigs = new();
    internal static void RegisterAvaterConfig(OscAvatarConfig avatarConfig)
    {
        _avatarConfigs.Add(new WeakReference<OscAvatarConfig>(avatarConfig));
    }

    public IReadOnlyDictionary<string, object?> CommonParameters => _commonParameters;

    public static OscAvatar CurrentAvatar => _currentAvatar;

    private static OscAvatar _currentAvatar;
    private static OscAvatar _changedAvatar;

    public static event OscValueChangedEventHandler<OscAvatar, OscAvatar>? OnAvatarChanged;

    internal static void Initialize()
    {
    }

    static OscAvatarUtility()
    {
        var server = OscUtility.Server;
        server.TryAddMethod("/avatar/change", ReadAvatarIdFromApp);
        server.AddMonitorCallback(ReadCommonParamsFromApp);
    }

    public static void ReadCommonParamsFromApp(BlobString blobString, OscMessageValues message)
    {
        string str = blobString.ToString();
        if (!str.StartsWith("/avatar/parameters/"))
        {
            return;
        }

        var name = str["/avatar/parameters/".Length..];
        if (!_commonParameters.ContainsKey(name))
        {
            return;
        }
        for (int i = 0; i < message.ElementCount; i++)
        {
            var oldValue = _commonParameters[name];
            var newValue = message.ReadValue(i);
            _commonParameters[name] = newValue;
            CallOnCommonParamaterChanged(name, new ValueChangedEventArgs(oldValue, newValue));
        }
    }

    private static void CallOnCommonParamaterChanged(string paramName, ValueChangedEventArgs e)
    {
        var configs = _avatarConfigs;

        for (int i = configs.Count - 1; i >= 0; i--)
        {
            if (!configs[i].TryGetTarget(out var config))
            {
                configs.RemoveAt(i);
                continue;
            }
            if (config.IsCreatedParameters)
            {
                var parameters = config.Parameters;
                parameters.OnParameterChanged(parameters.GetParameter(paramName), e);
            }
        }
    }

    public static void ReadAvatarIdFromApp(OscMessageValues message)
    {
        _changedAvatar.AvatarId = message.ReadStringElement(message.ElementCount - 1)!;
        CallOnAvatarChanged();
    }

    public static bool IsCommonParameter(string paramName) => _commonParameters.ContainsKey(paramName);
    public static object? GetCommonParameterValue(string paramName) => _commonParameters[paramName];
    public static IEnumerable<object?> GetCommonParameterValues() => _commonParameters.Values;

    private static void CallOnAvatarChanged()
    {
        var oldAvatar = _currentAvatar;
        var newAvatar = _changedAvatar;

        _currentAvatar = newAvatar;
        _changedAvatar = default;

        OnAvatarChanged?.Invoke(_currentAvatar, new ValueChangedEventArgs<OscAvatar>(oldAvatar, newAvatar));
    }
}
