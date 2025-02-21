﻿#nullable disable
using BlobHandles;
using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Input;
using BuildSoft.VRChat.Osc.Test.Utility;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test.Input;

[TestOf(typeof(OscInput))]
public class OscInputTests
{
    OscClient _client = null!;
    OscServer _server = null!;

    public static IEnumerable<TestCaseData> AllOscButtonInput
        => TestHelper.GetValues<OscButtonInput>().Select(item => new TestCaseData(item));
    public static IEnumerable<TestCaseData> AllOscAxisInput
        => TestHelper.GetValues<OscAxisInput>().Select(item => new TestCaseData(item));
    public static IEnumerable<TestCaseData> OscAxisInputTestCases
    {
        get
        {
            foreach (var item in TestHelper.GetValues<OscAxisInput>())
            {
                yield return new(item, 1f) { ExpectedResult = 1f };
                yield return new(item, 0f) { ExpectedResult = 0f };
                yield return new(item, 0.25f) { ExpectedResult = 0.25f };
                yield return new(item, -1f) { ExpectedResult = -1f };
                yield return new(item, +1.001f) { ExpectedResult = 1f };
                yield return new(item, -1.001f) { ExpectedResult = -1f };
                yield return new(item, 2.5f) { ExpectedResult = 1f };
                yield return new(item, -12.34f) { ExpectedResult = -1f };
                yield return new(item, float.MaxValue) { ExpectedResult = 1f };
                yield return new(item, float.MinValue) { ExpectedResult = -1f };
            }
        }
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

    [SetUp]
    public void Setup()
    {
        OscParameter.Parameters.Clear();
    }

    [TearDown]
    public void TearDown()
    {

    }

    [TestCaseSource(nameof(AllOscButtonInput))]
    public async Task TestSend_OscButtonInput(OscButtonInput buttonInput)
    {
        OscMessageValues values = null;
        string address = null;

        void Callback(BlobString a, OscMessageValues v)
            => (address, values) = (a.ToString(), v);
        _server.AddMonitorCallback(Callback);

        buttonInput.Send();
        await TestHelper.WaitWhile(() => values == null, TestHelper.LatencyTimeout);
        Assert.That(values.ReadIntElementUnchecked(0), Is.EqualTo(1));
        Assert.That(address, Is.EqualTo(buttonInput.CreateAddress()));
        values = null;

        buttonInput.Send(true);
        await TestHelper.WaitWhile(() => values == null, TestHelper.LatencyTimeout);
        Assert.That(values.ReadIntElementUnchecked(0), Is.EqualTo(1));
        Assert.That(address, Is.EqualTo(buttonInput.CreateAddress()));
        values = null;

        buttonInput.Send(false);
        await TestHelper.WaitWhile(() => values == null, TestHelper.LatencyTimeout);
        Assert.That(values.ReadIntElementUnchecked(0), Is.EqualTo(0));
        Assert.That(address, Is.EqualTo(buttonInput.CreateAddress()));
        values = null;

        _server.RemoveMonitorCallback(Callback);
    }

    [TestCaseSource(nameof(OscAxisInputTestCases))]
    public async Task<float> TestSend_OscAxisInput(OscAxisInput axisInput, float value)
    {
        OscMessageValues values = null;
        string address = null;

        void Callback(BlobString a, OscMessageValues v)
            => (address, values) = (a.ToString(), v);

        _server.AddMonitorCallback(Callback);

        axisInput.Send(value);
        await TestHelper.WaitWhile(() => values == null, TestHelper.LatencyTimeout);
        Assert.That(address, Is.EqualTo(axisInput.CreateAddress()));
        Assert.That(_server.RemoveMonitorCallback(Callback), Is.True);

        return values.ReadFloatElement(0);
    }

    [TestCaseSource(nameof(AllOscButtonInput))]
    public async Task TestPressRelease(OscButtonInput buttonInput)
    {
        OscMessageValues values = null;
        string address = null;

        void Callback(BlobString a, OscMessageValues v)
            => (address, values) = (a.ToString(), v);
        _server.AddMonitorCallback(Callback);

        buttonInput.Press();
        await TestHelper.WaitWhile(() => values == null, TestHelper.LatencyTimeout);
        Assert.That(address, Is.EqualTo(buttonInput.CreateAddress()));
        Assert.That(values.ReadIntElementUnchecked(0), Is.EqualTo(1));
        values = null;

        buttonInput.Release();
        await TestHelper.WaitWhile(() => values == null, TestHelper.LatencyTimeout);
        Assert.That(address, Is.EqualTo(buttonInput.CreateAddress()));
        Assert.That(values.ReadIntElementUnchecked(0), Is.EqualTo(0));
        values = null;

        _server.RemoveMonitorCallback(Callback);
    }

    [TestCaseSource(nameof(AllOscButtonInput))]
    public void TestCreateAddress(OscButtonInput buttonInput)
    {
        Assert.That(buttonInput.CreateAddress(), Is.EqualTo("/input/" + buttonInput.ToString()));
        Assert.That(buttonInput.CreateAddress(), Is.EqualTo("/input/" + buttonInput.ToString()));
    }

    [TestCaseSource(nameof(AllOscAxisInput))]
    public void TestCreateAddress(OscAxisInput axisInput)
    {
        Assert.That(axisInput.CreateAddress(), Is.EqualTo("/input/" + axisInput.ToString()));
        Assert.That(axisInput.CreateAddress(), Is.EqualTo("/input/" + axisInput.ToString()));
    }


    [TestCaseSource(nameof(AllOscAxisInput))]
    public void TestActiveAxisInputs(OscAxisInput axisInput)
    {
        var hasObsoleteAttribute = typeof(OscAxisInput).GetField(axisInput.ToString()).IsDefined(typeof(ObsoleteAttribute), true);

        Assert.That(OscInput.ActiveAxisInputs.Contains(axisInput), Is.Not.EqualTo(hasObsoleteAttribute));
    }

    [TestCaseSource(nameof(AllOscButtonInput))]
    public void TestActiveButtonInputs(OscButtonInput axisInput)
    {
        var hasObsoleteAttribute = typeof(OscButtonInput).GetField(axisInput.ToString()).IsDefined(typeof(ObsoleteAttribute), true);

        Assert.That(OscInput.ActiveButtonInputs.Contains(axisInput), Is.Not.EqualTo(hasObsoleteAttribute));
    }
}
