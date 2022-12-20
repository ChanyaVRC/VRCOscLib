using System.Collections.Immutable;

namespace BuildSoft.VRChat.Osc.Tracking;

/// <summary>
/// Provide methods for interacting with OSC tracking.
/// </summary>
public class OscTracking
{
    /// <summary>
    /// The head tracker.
    /// </summary>
    private static OscTracker? _headTracker;
    /// <summary>
    /// A lazy-initialized array of all trackers.
    /// </summary>
    private static readonly Lazy<ImmutableArray<OscTracker>> _trackers = new(CreateTrackers);

    /// <summary>
    /// Gets the head tracker.
    /// </summary>
    public static OscTracker HeadTracker => _headTracker ??= new OscTracker("head");

    /// <summary>
    /// Gets all trackers.
    /// </summary>
    public static ImmutableArray<OscTracker> Trackers => _trackers.Value;

    /// <summary>
    /// Creates an array of all trackers.
    /// </summary>
    /// <returns>An array of all trackers.</returns>
    private static ImmutableArray<OscTracker> CreateTrackers()
    {
        var trackers = ImmutableArray.CreateBuilder<OscTracker>(OscTracker.SupportedTrackerCount);
        for (int i = 0; i < OscTracker.SupportedTrackerCount; i++)
        {
            trackers.Add(new OscTracker(i));
        }
        return trackers.ToImmutable();
    }

    private OscTracking()
    {
    }
}
