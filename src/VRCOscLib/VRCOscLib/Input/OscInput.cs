using System.Reflection;
using BuildSoft.VRChat.Osc.Utility;

namespace BuildSoft.VRChat.Osc.Input;

public static class OscInput
{
    private static readonly Dictionary<OscAxisInput, string> _axisInputAddressCache = new();
    private static readonly Dictionary<OscButtonInput, string> _buttonInputAddressCache = new();
    private static IEnumerable<OscButtonInput>? _activeButtonInputs;
    private static IEnumerable<OscAxisInput>? _activeAxisInputs;

    public static void Send(this OscButtonInput content, bool isOn = true)
    {
        OscParameter.SendValue(content.CreateAddress(), isOn ? 1 : 0);
    }
    public static void Press(this OscButtonInput content)
    {
        Send(content, true);
    }
    public static void Release(this OscButtonInput content)
    {
        Send(content, false);
    }

    public static void Send(this OscAxisInput content, float value)
    {
        OscParameter.SendValue(content.CreateAddress(), MathHelper.Clamp(value, -1f, 1f));
    }

    public static string CreateAddress(this OscButtonInput content)
    {
        if (_buttonInputAddressCache.TryGetValue(content, out var address))
        {
            return address;
        }
        lock (_buttonInputAddressCache)
        {
            if (_buttonInputAddressCache.TryGetValue(content, out address))
            {
                return address;
            }
            address = "/input/" + content.ToString();
            _buttonInputAddressCache.Add(content, address);
        }
        return address;
    }

    public static string CreateAddress(this OscAxisInput content)
    {
        if (_axisInputAddressCache.TryGetValue(content, out var address))
        {
            return address;
        }
        lock (_axisInputAddressCache)
        {
            if (_axisInputAddressCache.TryGetValue(content, out address))
            {
                return address;
            }
            address = "/input/" + content.ToString();
            _axisInputAddressCache.Add(content, address);
        }
        return address;
    }

    public static IEnumerable<OscButtonInput> ActiveButtonInputs => _activeButtonInputs ??= CreateActiveFields<OscButtonInput>();
    public static IEnumerable<OscAxisInput> ActiveAxisInputs => _activeAxisInputs ??= CreateActiveFields<OscAxisInput>();

    private static IEnumerable<T> CreateActiveFields<T>() where T : Enum
    {
        return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(s => !s.IsDefined(typeof(ObsoleteAttribute), true))
            .Select(s => (T)s.GetValue(null))
            .ToArray();
    }
}
