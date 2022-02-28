#nullable disable

using System;
using System.Threading.Tasks;
using BuildSoft.OscCore;
using BuildSoft.OscCore.UnityObjects;
using NUnit.Framework;

using static BuildSoft.VRChat.Osc.Test.TestUtility;

namespace BuildSoft.VRChat.Osc.Test;

[TestOf(typeof(OscParameter))]
public class OscParameterTests
{
    private OscClient _client;
    private OscServer _server;
    private int _defaultReceivePort;
    private int _defaultSendPort;
    private const int TestClientPort = 8001;
    private const int TestServerPort = 8002;

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

    [Test]
    public async Task SendAvatarParameterTest()
    {
        OscMessageValues value = null;
        const string ParamName = "name";
        void valueReadMethod(OscMessageValues v) => value = v;
        _server.TryAddMethod(OscConst.AvatarParameterAddressSpace + ParamName, valueReadMethod);

        OscParameter.SendAvatarParameter(ParamName, 1.2f);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadFloatElementUnchecked(0), 1.2f);
        value = null;

        OscParameter.SendAvatarParameter(ParamName, 10);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadIntElementUnchecked(0), 10);
        value = null;

        OscParameter.SendAvatarParameter(ParamName, true);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadBooleanElement(0), true);
        value = null;

        OscParameter.SendAvatarParameter(ParamName, false);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadBooleanElement(0), false);
        value = null;

        OscParameter.SendAvatarParameter(ParamName, "value");
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadStringElement(0), "value");
        value = null;

        OscParameter.SendAvatarParameter(ParamName, 10.1);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadFloat64ElementUnchecked(0), 10.1);
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new Vector2(1, 2));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadFloatElementUnchecked(0), 1);
        Assert.AreEqual(value.ReadFloatElementUnchecked(1), 2);
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new Vector3(1, 2, 3));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadFloatElementUnchecked(0), 1);
        Assert.AreEqual(value.ReadFloatElementUnchecked(1), 2);
        Assert.AreEqual(value.ReadFloatElementUnchecked(2), 3);
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new Color32(1, 2, 3, 4));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadIntElementUnchecked(0), 0x04030201);
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new MidiMessage(1, 2, 3, 4));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadMidiElementUnchecked(0), new MidiMessage(1, 2, 3, 4));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new byte[] { 1, 2, 3, 4 });
        await LoopWhile(() => value == null, LatencyTimeout);
        CollectionAssert.AreEqual(value.ReadBlobElement(0), new byte[] { 1, 2, 3, 4 });
        value = null;

        OscParameter.SendAvatarParameter(ParamName, 'c');
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadAsciiCharElement(0), 'c');
        value = null;

        OscParameter.SendAvatarParameter(ParamName, (object)10);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadIntElementUnchecked(0), 10);
        value = null;
    }

    [Test]
    public async Task SendValueTest()
    {
        OscMessageValues value = null;
        const string Address = "/test/address";
        void valueReadMethod(OscMessageValues v) => value = v;
        _server.TryAddMethod(Address, valueReadMethod);

        OscParameter.SendValue(Address, 1.2f);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadFloatElementUnchecked(0), 1.2f);
        value = null;

        OscParameter.SendValue(Address, 10);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadIntElementUnchecked(0), 10);
        value = null;

        OscParameter.SendValue(Address, true);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadBooleanElement(0), true);
        value = null;

        OscParameter.SendValue(Address, false);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadBooleanElement(0), false);
        value = null;

        OscParameter.SendValue(Address, "value");
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadStringElement(0), "value");
        value = null;

        OscParameter.SendValue(Address, 10.1);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadFloat64ElementUnchecked(0), 10.1);
        value = null;

        OscParameter.SendValue(Address, new Vector2(1, 2));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadFloatElementUnchecked(0), 1);
        Assert.AreEqual(value.ReadFloatElementUnchecked(1), 2);
        value = null;

        OscParameter.SendValue(Address, new Vector3(1, 2, 3));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadFloatElementUnchecked(0), 1);
        Assert.AreEqual(value.ReadFloatElementUnchecked(1), 2);
        Assert.AreEqual(value.ReadFloatElementUnchecked(2), 3);
        value = null;

        OscParameter.SendValue(Address, new Color32(1, 2, 3, 4));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadIntElementUnchecked(0), 0x04030201);
        value = null;

        OscParameter.SendValue(Address, new MidiMessage(1, 2, 3, 4));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadMidiElementUnchecked(0), new MidiMessage(1, 2, 3, 4));
        value = null;

        OscParameter.SendValue(Address, new byte[] { 1, 2, 3, 4 });
        await LoopWhile(() => value == null, LatencyTimeout);
        CollectionAssert.AreEqual(value.ReadBlobElement(0), new byte[] { 1, 2, 3, 4 });
        value = null;

        OscParameter.SendValue(Address, 'c');
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(value.ReadAsciiCharElement(0), 'c');
        value = null;
    }

    [TestCase(123)]
    [TestCase(123.4f)]
    [TestCase(123.5)]
    [TestCase(true)]
    [TestCase(false)]
    [TestCase("value")]
    [TestCase(new byte[] { 1, 2, 3, 4 })]
    public void ParametersTest1(object value)
    {
        const string ParamName = "paramName";
        const string Address = OscConst.AvatarParameterAddressSpace + ParamName;
        OscParameter.SendAvatarParameter(ParamName, value);
        Assert.AreEqual(value, OscParameter.Parameters[Address]);
    }

    [TestCase(new byte[] { 1, 2, 3, 4 })]
    public void ParametersTest1Blob(byte[] value)
    {
        const string ParamName = "paramName";
        const string Address = OscConst.AvatarParameterAddressSpace + ParamName;
        OscParameter.SendAvatarParameter(ParamName, value);

        CollectionAssert.AreEqual(value, (byte[])OscParameter.Parameters[Address]!);
        CollectionAssert.AreEqual(value, (byte[])OscParameter.GetValue(Address)!);
    }

    [TestCase(123)]
    [TestCase(123.4f)]
    [TestCase(123.5)]
    [TestCase(true)]
    [TestCase(false)]
    [TestCase("value")]
    public async Task ParametersTest2(object value)
    {
        const string ParamName = "paramName";
        const string Address = OscConst.AvatarParameterAddressSpace + ParamName;
        OscParameter.Parameters.Remove(Address);

        _client.Send(Address, (dynamic)value);

        await LoopWhile(() => !OscParameter.Parameters.ContainsKey(Address), LatencyTimeout);

        Assert.AreEqual(value, OscParameter.Parameters[Address]);
    }

    [TestCase(new byte[] { 1, 2, 3, 4 })]
    public async Task ParametersTest2Blob(byte[] value)
    {
        const string ParamName = "paramName";
        const string Address = OscConst.AvatarParameterAddressSpace + ParamName;
        OscParameter.Parameters.Remove(Address);

        _client.Send(Address, value, value.Length);

        await LoopWhile(() => !OscParameter.Parameters.ContainsKey(Address), LatencyTimeout);

        CollectionAssert.AreEqual(value, (byte[])OscParameter.Parameters[Address]!);
    }
}
