using ParamChangedHandler = BuildSoft.VRChat.Osc.OscParameterChangedEventHandler<BuildSoft.VRChat.Osc.IReadOnlyOscParameterCollection>;

namespace BuildSoft.VRChat.Osc;

public interface IReadOnlyOscParameterCollection : IReadOnlyDictionary<string, object?>
{
    event ParamChangedHandler? ValueChanged;

    public void AddValueChangedEventByAddress(string address, ParamChangedHandler handler);
    bool RemoveValueChangedEventByAddress(string address, ParamChangedHandler handler);
}
