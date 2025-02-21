using BuildSoft.OscCore;
using BuildSoft.OscCore.UnityObjects;
using BuildSoft.VRChat.Osc.Test;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Tracking.Test;

[TestOf(typeof(OscTracking))]
public class OscTrackingTest
{
    private OscClient _client = null!;
    private OscServer _server = null!;

    [SetUp]
    public void Setup()
    {
        OscParameter.Parameters.Clear();
    }

    [TearDown]
    public void TearDown()
    {

    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _client = new OscClient("127.0.0.1", OscUtility.ReceivePort);
        _server = new OscServer(OscUtility.SendPort);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }


    [Test]
    public void HeadTracker_AddressTest()
    {
        var headTracker = OscTracking.HeadTracker;

        Assert.That(headTracker.PositionAddress, Is.EqualTo("/tracking/trackers/head/position"));
        Assert.That(headTracker.RotationAddress, Is.EqualTo("/tracking/trackers/head/rotation"));
    }

    [Test]
    public async Task HeadTracker_PositionSetTest()
    {
        var headTracker = OscTracking.HeadTracker;

        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        _server.TryAddMethod(headTracker.PositionAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);

        headTracker.Position = expected;
        await TestUtility.LoopWhile(() => value == null, TestUtility.LatencyTimeout);

        Assert.That(headTracker.Position, Is.EqualTo(expected));
        Assert.That(value.ReadFloatElement(0), Is.EqualTo(expected.x));
        Assert.That(value.ReadFloatElement(1), Is.EqualTo(expected.y));
        Assert.That(value.ReadFloatElement(2), Is.EqualTo(expected.z));
    }

    [Test]
    public async Task HeadTracker_RotationSetTest()
    {
        var headTracker = OscTracking.HeadTracker;

        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        _server.TryAddMethod(headTracker.RotationAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);

        headTracker.Rotation = expected;
        await TestUtility.LoopWhile(() => value == null, TestUtility.LatencyTimeout);

        Assert.That(headTracker.Rotation, Is.EqualTo(expected));
        Assert.That(value.ReadFloatElement(0), Is.EqualTo(expected.x));
        Assert.That(value.ReadFloatElement(1), Is.EqualTo(expected.y));
        Assert.That(value.ReadFloatElement(2), Is.EqualTo(expected.z));
    }


    [Test]
    public async Task HeadTracker_PositionGetTest()
    {
        var headTracker = OscTracking.HeadTracker;
        Assert.That(headTracker.Position, Is.EqualTo(new Vector3()));

        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        OscUtility.Server.TryAddMethod(headTracker.PositionAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);
        _client.Send(headTracker.PositionAddress, expected);

        await TestUtility.LoopWhile(() => value == null, TestUtility.LatencyTimeout);
        Assert.That(headTracker.Position, Is.EqualTo(expected));

        OscUtility.Server.RemoveMethod(headTracker.PositionAddress, valueReadMethod);
    }

    [Test]
    public async Task HeadTracker_RotationGetTest()
    {
        var headTracker = OscTracking.HeadTracker;
        Assert.That(headTracker.Position, Is.EqualTo(new Vector3()));

        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        OscUtility.Server.TryAddMethod(headTracker.RotationAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);
        _client.Send(headTracker.RotationAddress, expected);

        await TestUtility.LoopWhile(() => value == null, TestUtility.LatencyTimeout);
        Assert.That(headTracker.Rotation, Is.EqualTo(expected));

        OscUtility.Server.RemoveMethod(headTracker.RotationAddress, valueReadMethod);
    }


    [Test]
    public void TrackersTest()
    {
        var trackers = OscTracking.Trackers;

        Assert.That(trackers.Length, Is.EqualTo(OscTracker.SupportedTrackerCount));
        Assert.That(trackers[0].PositionAddress, Is.EqualTo("/tracking/trackers/1/position"));
        Assert.That(trackers[0].RotationAddress, Is.EqualTo("/tracking/trackers/1/rotation"));

        for (int i = 0; i < OscTracker.SupportedTrackerCount; i++)
        {
            Assert.That(trackers[i].PositionAddress, Is.EqualTo($"/tracking/trackers/{i + 1}/position"));
            Assert.That(trackers[i].RotationAddress, Is.EqualTo($"/tracking/trackers/{i + 1}/rotation"));
        }
    }
}
