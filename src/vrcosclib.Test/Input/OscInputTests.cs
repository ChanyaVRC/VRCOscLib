#nullable disable
using BlobHandles;
using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Test;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Input.Test;

[TestOf(typeof(OscInput))]
public class OscInputTests
{
    OscClient _client = null!;
    OscServer _server = null!;
    private int _defaultReceivePort;
    private int _defaultSendPort;
    const int TestClientPort = 8001;
    const int TestServerPort = 8002;

    [SetUp]
    public void Setup()
    {
        _defaultReceivePort = OscUtility.ReceivePort;
        _defaultSendPort = OscUtility.SendPort;
        
        _client = new OscClient("127.0.0.1", OscUtility.ReceivePort);
        _server = OscServer.GetOrCreate(OscUtility.SendPort);
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        OscUtility.ReceivePort = TestClientPort;
        OscUtility.SendPort = TestServerPort;

        OscParameter.Parameters.Clear();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        OscUtility.ReceivePort = _defaultReceivePort;
        OscUtility.SendPort = _defaultSendPort;
    }

    [TestCase(OscButtonInput.AFKToggle)]
    [TestCase(OscButtonInput.Back)]
    [TestCase(OscButtonInput.MoveBackward)]
    public async Task TestSend(OscButtonInput buttonInput)
    {
        OscMessageValues values = null;
        string address = null;

        void Callback(BlobString a, OscMessageValues v)
            => (address, values) = (a.ToString(), v);
        _server.AddMonitorCallback(Callback);

        buttonInput.Send();
        await TestUtility.LoopWhile(() => values == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(buttonInput.CreateAddress(), address);
        Assert.AreEqual(1, values.ReadIntElementUnchecked(0));
        values = null;

        buttonInput.Send(true);
        await TestUtility.LoopWhile(() => values == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(buttonInput.CreateAddress(), address);
        Assert.AreEqual(1, values.ReadIntElementUnchecked(0));
        values = null;

        buttonInput.Send(false);
        await TestUtility.LoopWhile(() => values == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(buttonInput.CreateAddress(), address);
        Assert.AreEqual(0, values.ReadIntElementUnchecked(0));
        values = null;

        _server.RemoveMonitorCallback(Callback);
    }

    [TestCase(OscAxisInput.SpinHoldUD)]
    [TestCase(OscAxisInput.Horizontal)]
    [TestCase(OscAxisInput.LookHorizontal)]
    public async Task TestSend(OscAxisInput axisInput)
    {
        OscMessageValues values = null;
        string address = null;

        void Callback(BlobString a, OscMessageValues v)
            => (address, values) = (a.ToString(), v);
        _server.AddMonitorCallback(Callback);

        axisInput.Send(1f);
        await TestUtility.LoopWhile(() => values == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(axisInput.CreateAddress(), address);
        Assert.AreEqual(1, values.ReadFloatElementUnchecked(0));
        values = null;

        axisInput.Send(0.25f);
        await TestUtility.LoopWhile(() => values == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(axisInput.CreateAddress(), address);
        Assert.AreEqual(0.25f, values.ReadFloatElementUnchecked(0));
        values = null;

        axisInput.Send(-1);
        await TestUtility.LoopWhile(() => values == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(axisInput.CreateAddress(), address);
        Assert.AreEqual(-1, values.ReadFloatElementUnchecked(0));
        values = null;

        axisInput.Send(-1.002f);
        await TestUtility.LoopWhile(() => values == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(axisInput.CreateAddress(), address);
        Assert.AreEqual(-1, values.ReadFloatElementUnchecked(0));
        values = null;

        axisInput.Send(1.2f);
        await TestUtility.LoopWhile(() => values == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(axisInput.CreateAddress(), address);
        Assert.AreEqual(1, values.ReadFloatElementUnchecked(0));
        values = null;

        _server.RemoveMonitorCallback(Callback);
    }

    [TestCase(OscButtonInput.AFKToggle)]
    [TestCase(OscButtonInput.Back)]
    [TestCase(OscButtonInput.MoveBackward)]
    public async Task TestPressRelease(OscButtonInput buttonInput)
    {
        OscMessageValues values = null;
        string address = null;

        void Callback(BlobString a, OscMessageValues v) 
            => (address, values) = (a.ToString(), v);
        _server.AddMonitorCallback(Callback);

        buttonInput.Press();
        await TestUtility.LoopWhile(() => values == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(buttonInput.CreateAddress(), address);
        Assert.AreEqual(1, values.ReadIntElementUnchecked(0));
        values = null;

        buttonInput.Release();
        await TestUtility.LoopWhile(() => values == null, TestUtility.LatencyTimeout);
        Assert.AreEqual(buttonInput.CreateAddress(), address);
        Assert.AreEqual(0, values.ReadIntElementUnchecked(0));
        values = null;

        _server.RemoveMonitorCallback(Callback);
    }

    [TestCase(OscButtonInput.AFKToggle)]
    [TestCase(OscButtonInput.Run)]
    [TestCase(OscButtonInput.ComfortRight)]
    public void TestCreateAddress(OscButtonInput buttonInput)
    {
        Assert.AreEqual("/input/" + buttonInput.ToString(), buttonInput.CreateAddress());
        Assert.AreEqual("/input/" + buttonInput.ToString(), buttonInput.CreateAddress());
    }

    [TestCase(OscAxisInput.SpinHoldUD)]
    [TestCase(OscAxisInput.Vertical)]
    [TestCase(OscAxisInput.LookHorizontal)]
    public void TestCreateAddress(OscAxisInput axisInput)
    {
        Assert.AreEqual("/input/" + axisInput.ToString(), axisInput.CreateAddress());
        Assert.AreEqual("/input/" + axisInput.ToString(), axisInput.CreateAddress());
    }
}
