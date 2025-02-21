using BuildSoft.OscCore;
using BuildSoft.OscCore.UnityObjects;
using BuildSoft.VRChat.Osc.Test.Utility;
using BuildSoft.VRChat.Osc.Tracking;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test.Tracking;

[TestOf(typeof(OscTracking))]
public class OscTrackingTest
{
    private OscClient _client = null!;
    private OscServer _server = null!;

    [SetUp]
    public void Setup()
    {
        OscParameter.Items.Clear();
    }

    [TearDown]
    public void TearDown()
    {

    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort);
        _server = new OscServer(OscConnectionSettings.SendPort);
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
        await TestHelper.WaitWhile(() => value == null, TestHelper.LatencyTimeout);

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
        await TestHelper.WaitWhile(() => value == null, TestHelper.LatencyTimeout);

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
        OscConnectionSettings.Server.TryAddMethod(headTracker.PositionAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);
        _client.Send(headTracker.PositionAddress, expected);

        await TestHelper.WaitWhile(() => value == null, TestHelper.LatencyTimeout);
        Assert.That(headTracker.Position, Is.EqualTo(expected));

        OscConnectionSettings.Server.RemoveMethod(headTracker.PositionAddress, valueReadMethod);
    }

    [Test]
    public async Task HeadTracker_RotationGetTest()
    {
        var headTracker = OscTracking.HeadTracker;
        Assert.That(headTracker.Position, Is.EqualTo(new Vector3()));

        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        OscConnectionSettings.Server.TryAddMethod(headTracker.RotationAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);
        _client.Send(headTracker.RotationAddress, expected);

        await TestHelper.WaitWhile(() => value == null, TestHelper.LatencyTimeout);
        Assert.That(headTracker.Rotation, Is.EqualTo(expected));

        OscConnectionSettings.Server.RemoveMethod(headTracker.RotationAddress, valueReadMethod);
    }


    [Test]
    public void TrackersTest()
    {
        var trackers = OscTracking.Trackers;

        Assert.That(trackers.Length, Is.EqualTo(OscTracker.SupportedTrackerCount));
        Assert.That(trackers[0].PositionAddress, Is.EqualTo("/tracking/trackers/1/position"));
        Assert.That(trackers[0].RotationAddress, Is.EqualTo("/tracking/trackers/1/rotation"));

        for (var i = 0; i < OscTracker.SupportedTrackerCount; i++)
        {
            Assert.That(trackers[i].PositionAddress, Is.EqualTo($"/tracking/trackers/{i + 1}/position"));
            Assert.That(trackers[i].RotationAddress, Is.EqualTo($"/tracking/trackers/{i + 1}/rotation"));
        }
    }
}
