using BuildSoft.OscCore.UnityObjects;

namespace BuildSoft.VRChat.Osc.Tracking;

/// <summary>
/// Provides an OSC tracker
/// </summary>
public class OscTracker
{
    /// <summary>
    /// The number of supported trackers.
    /// </summary>
    public static readonly int SupportedTrackerCount = 8;

    /// <summary>
    /// The OSC address for the position of the tracker.
    /// </summary>
    public string PositionAddress { get; }

    /// <summary>
    /// The OSC address for the rotation of the tracker.
    /// </summary>
    public string RotationAddress { get; }

    /// <summary>
    /// Gets or sets the position of the tracker.
    /// </summary>
    public Vector3 Position
    {
        get => OscParameter.GetValueAsVector3(PositionAddress) ?? default;
        set => OscParameter.SendValue(PositionAddress, value);
    }

    /// <summary>
    /// Gets or sets the rotation of the tracker.
    /// </summary>
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

    /// <summary>
    /// Create the OSC tracker.
    /// </summary>
    /// <param name="part">The part of the OSC address for the tracker.</param>
    internal OscTracker(string part)
    {
        PositionAddress = $"/tracking/trackers/{part}/position";
        RotationAddress = $"/tracking/trackers/{part}/rotation";
    }
}
