﻿using BuildSoft.OscCore;
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

        Assert.AreEqual("/tracking/trackers/head/position", headTracker.PositionAddress);
        Assert.AreEqual("/tracking/trackers/head/rotation", headTracker.RotationAddress);
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

        Assert.AreEqual(expected, headTracker.Position);
        Assert.AreEqual(expected.x, value.ReadFloatElement(0));
        Assert.AreEqual(expected.y, value.ReadFloatElement(1));
        Assert.AreEqual(expected.z, value.ReadFloatElement(2));
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

        Assert.AreEqual(expected, headTracker.Rotation);
        Assert.AreEqual(expected.x, value.ReadFloatElement(0));
        Assert.AreEqual(expected.y, value.ReadFloatElement(1));
        Assert.AreEqual(expected.z, value.ReadFloatElement(2));
    }


    [Test]
    public async Task HeadTracker_PositionGetTest()
    {
        var headTracker = OscTracking.HeadTracker;
        Assert.AreEqual(new Vector3(), headTracker.Position);

        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        OscUtility.Server.TryAddMethod(headTracker.PositionAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);
        _client.Send(headTracker.PositionAddress, expected);

        await TestUtility.LoopWhile(() => value == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(expected, headTracker.Position);

        OscUtility.Server.RemoveMethod(headTracker.PositionAddress, valueReadMethod);
    }

    [Test]
    public async Task HeadTracker_RotationGetTest()
    {
        var headTracker = OscTracking.HeadTracker;
        Assert.AreEqual(new Vector3(), headTracker.Position);

        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        OscUtility.Server.TryAddMethod(headTracker.RotationAddress, valueReadMethod);

        var expected = new Vector3(10.1f, 20.2f, 30.3f);
        _client.Send(headTracker.RotationAddress, expected);

        await TestUtility.LoopWhile(() => value == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(expected, headTracker.Rotation);

        OscUtility.Server.RemoveMethod(headTracker.RotationAddress, valueReadMethod);
    }


    [Test]
    public void TrackersTest()
    {
        var trackers = OscTracking.Trackers;

        Assert.AreEqual(OscTracker.SupportedTrackerCount, trackers.Length);
        Assert.AreEqual("/tracking/trackers/1/position", trackers[0].PositionAddress);
        Assert.AreEqual("/tracking/trackers/1/rotation", trackers[0].RotationAddress);

        for (int i = 0; i < OscTracker.SupportedTrackerCount; i++)
        {
            Assert.AreEqual($"/tracking/trackers/{i + 1}/position", trackers[i].PositionAddress);
            Assert.AreEqual($"/tracking/trackers/{i + 1}/rotation", trackers[i].RotationAddress);
        }
    }
}
