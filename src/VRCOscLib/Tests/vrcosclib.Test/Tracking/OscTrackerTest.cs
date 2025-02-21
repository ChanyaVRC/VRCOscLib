using BuildSoft.OscCore;
using BuildSoft.OscCore.UnityObjects;
using BuildSoft.VRChat.Osc.Test.Utility;
using BuildSoft.VRChat.Osc.Tracking;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test.Tracking;

[TestOf(typeof(OscTracker))]
public class OscTrackerTest
{
    private static IEnumerable<int> ValidRangeSource
    {
        get
        {
            yield return 0;
            yield return OscTracker.SupportedTrackerCount / 2;
            yield return OscTracker.SupportedTrackerCount - 1;
        }
    }

    private static IEnumerable<int> InvalidRangeSource
    {
        get
        {
            yield return -1;
            yield return OscTracker.SupportedTrackerCount;
            yield return int.MaxValue;
            yield return int.MinValue;
        }
    }


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


    [TestCaseSource(nameof(ValidRangeSource))]
    public void Ctor_ValidRangeTest(int index)
    {
        Assert.DoesNotThrow(() => new OscTracker(index));

        OscTracker tracker = new(index);
        Assert.That(tracker.PositionAddress, Is.EqualTo($"/tracking/trackers/{index + 1}/position"));
        Assert.That(tracker.RotationAddress, Is.EqualTo($"/tracking/trackers/{index + 1}/rotation"));
    }

    [TestCaseSource(nameof(InvalidRangeSource))]
    public void Ctor_InvalidRangeTest(int index)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new OscTracker(index));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.ParamName, Is.EqualTo("index"));
    }


    [Test]
    public async Task PositionTest()
    {
        var tracker = new OscTracker(0);
        Assert.That(tracker.Position, Is.EqualTo(new Vector3()));

        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        _server.TryAddMethod(tracker.PositionAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);

        tracker.Position = expected;
        await TestHelper.LoopWhile(() => value == null, TestHelper.LatencyTimeout);

        Assert.That(tracker.Position, Is.EqualTo(expected));
        Assert.That(value.ReadFloatElement(0), Is.EqualTo(expected.x));
        Assert.That(value.ReadFloatElement(1), Is.EqualTo(expected.y));
        Assert.That(value.ReadFloatElement(2), Is.EqualTo(expected.z));

        Assert.That(new OscTracker(0).Position, Is.EqualTo(expected));
    }

    [Test]
    public async Task RotationTest()
    {
        var tracker = new OscTracker(0);
        Assert.That(tracker.Rotation, Is.EqualTo(new Vector3()));

        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        _server.TryAddMethod(tracker.RotationAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);

        tracker.Rotation = expected;
        await TestHelper.LoopWhile(() => value == null, TestHelper.LatencyTimeout);

        Assert.That(tracker.Rotation, Is.EqualTo(expected));
        Assert.That(value.ReadFloatElement(0), Is.EqualTo(expected.x));
        Assert.That(value.ReadFloatElement(1), Is.EqualTo(expected.y));
        Assert.That(value.ReadFloatElement(2), Is.EqualTo(expected.z));

        Assert.That(new OscTracker(0).Rotation, Is.EqualTo(expected));
    }
}
