using BuildSoft.OscCore.UnityObjects;

namespace BuildSoft.VRChat.Osc.Tracking;
public class OscTracker
{
    public static readonly int SupportedTrackerCount = 8;

    public string PositionAddress { get; }
    public string RotationAddress { get; }

    public Vector3 Position
    {
        get => OscParameter.GetValueAsVector3(PositionAddress) ?? default;
        set => OscParameter.SendValue(PositionAddress, value);
    }

    public Vector3 Rotation
    {
        get => OscParameter.GetValueAsVector3(RotationAddress) ?? default;
        set => OscParameter.SendValue(RotationAddress, value);
    }


    /// <summary>
    /// Create the OSC tracker.
    /// </summary>
    /// <param name="index">0 base index. (0 &#x2266; <paramref name="index"/> &lt; <see cref="SupportedTrackerCount"/>)</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is negative or greater than or equal to <see cref="SupportedTrackerCount"/>.
    /// (0 &#x2266; <paramref name="index"/> &lt; <see cref="SupportedTrackerCount"/>)
    /// </exception>
    public OscTracker(int index) : this((index + 1).ToString())
    {
        if (index < 0 || index >= SupportedTrackerCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    internal OscTracker(string part)
    {
        PositionAddress = $"/tracking/trackers/{part}/position";
        RotationAddress = $"/tracking/trackers/{part}/rotation";
    }
}
