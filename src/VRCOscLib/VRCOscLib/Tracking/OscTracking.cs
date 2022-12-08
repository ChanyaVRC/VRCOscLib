using System.Collections.Immutable;

namespace BuildSoft.VRChat.Osc.Tracking;

public class OscTracking
{
    private static OscTracker? _headTracker;
    private static readonly Lazy<ImmutableArray<OscTracker>> _trackers = new(CreateTrackers);

    public static OscTracker HeadTracker => _headTracker ??= new OscTracker("head");
    public static ImmutableArray<OscTracker> Trackers => _trackers.Value;

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
