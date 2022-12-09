using BuildSoft.OscCore;
using BuildSoft.OscCore.UnityObjects;
using BuildSoft.VRChat.Osc.Test;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Tracking.Test;

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
        Assert.AreEqual($"/tracking/trackers/{index + 1}/position", tracker.PositionAddress);
        Assert.AreEqual($"/tracking/trackers/{index + 1}/rotation", tracker.RotationAddress);
    }

    [TestCaseSource(nameof(InvalidRangeSource))]
    public void Ctor_InvalidRangeTest(int index)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new OscTracker(index));

        Assert.NotNull(exception);
        Assert.AreEqual("index", exception!.ParamName);
    }


    [Test]
    public async Task PositionTest()
    {
        var tracker = new OscTracker(0);
        Assert.AreEqual(new Vector3(), tracker.Position);

        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        _server.TryAddMethod(tracker.PositionAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);

        tracker.Position = expected;
        await TestUtility.LoopWhile(() => value == null, TestUtility.LatencyTimeout);

        Assert.AreEqual(expected, tracker.Position);
        Assert.AreEqual(expected.x, value.ReadFloatElement(0));
        Assert.AreEqual(expected.y, value.ReadFloatElement(1));
        Assert.AreEqual(expected.z, value.ReadFloatElement(2));

        Assert.AreEqual(expected, new OscTracker(0).Position);
    }

    [Test]
    public async Task RotationTest()
    {
        var tracker = new OscTracker(0);
        Assert.AreEqual(new Vector3(), tracker.Rotation);

        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        _server.TryAddMethod(tracker.RotationAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);

        tracker.Rotation = expected;
        await TestUtility.LoopWhile(() => value == null, TestUtility.LatencyTimeout);

        Assert.AreEqual(expected, tracker.Rotation);
        Assert.AreEqual(expected.x, value.ReadFloatElement(0));
        Assert.AreEqual(expected.y, value.ReadFloatElement(1));
        Assert.AreEqual(expected.z, value.ReadFloatElement(2));

        Assert.AreEqual(expected, new OscTracker(0).Rotation);
    }
}
