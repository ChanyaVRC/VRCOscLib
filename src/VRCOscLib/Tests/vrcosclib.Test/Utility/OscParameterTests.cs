#nullable disable

using System;
using System.Net;
using System.Threading.Tasks;
using BuildSoft.OscCore;
using BuildSoft.OscCore.UnityObjects;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

using static BuildSoft.VRChat.Osc.Test.TestUtility;

namespace BuildSoft.VRChat.Osc.Test;

[TestOf(typeof(OscParameter))]
public class OscParameterTests
{
    private OscClient _client;
    private OscServer _server;

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
        _client = new OscClient(OscUtility.VrcIPAddress, OscUtility.ReceivePort);
        _server = new OscServer(OscUtility.SendPort);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _server.Dispose();
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
        Assert.AreEqual(1.2f, value.ReadFloatElementUnchecked(0));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, 10);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(10, value.ReadIntElementUnchecked(0));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, 1234567890123456789);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(1234567890123456789, value.ReadInt64ElementUnchecked(0));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, true);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(true, value.ReadBooleanElement(0));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, false);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(false, value.ReadBooleanElement(0));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, "value");
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual("value", value.ReadStringElement(0));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, 10.1);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(10.1, value.ReadFloat64ElementUnchecked(0));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new Vector2(1, 2));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(1, value.ReadFloatElementUnchecked(0));
        Assert.AreEqual(2, value.ReadFloatElementUnchecked(1));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new Vector3(1, 2, 3));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(1, value.ReadFloatElementUnchecked(0));
        Assert.AreEqual(2, value.ReadFloatElementUnchecked(1));
        Assert.AreEqual(3, value.ReadFloatElementUnchecked(2));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new Color32(1, 2, 3, 4));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(0x04030201, value.ReadIntElementUnchecked(0));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new MidiMessage(1, 2, 3, 4));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(new MidiMessage(1, 2, 3, 4), value.ReadMidiElementUnchecked(0));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new byte[] { 1, 2, 3, 4 });
        await LoopWhile(() => value == null, LatencyTimeout);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, value.ReadBlobElement(0));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, 'c');
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual('c', value.ReadAsciiCharElement(0));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, (object)10);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(10, value.ReadIntElementUnchecked(0));
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
        Assert.AreEqual(1.2f, value.ReadFloatElementUnchecked(0));
        value = null;

        OscParameter.SendValue(Address, 10);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(10, value.ReadIntElementUnchecked(0));
        value = null;

        OscParameter.SendValue(Address, 1234567890123456789);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(1234567890123456789, value.ReadInt64ElementUnchecked(0));
        value = null;

        OscParameter.SendValue(Address, true);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(true, value.ReadBooleanElement(0));
        value = null;

        OscParameter.SendValue(Address, false);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(false, value.ReadBooleanElement(0));
        value = null;

        OscParameter.SendValue(Address, "value");
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual("value", value.ReadStringElement(0));
        value = null;

        OscParameter.SendValue(Address, 10.1);
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(10.1, value.ReadFloat64ElementUnchecked(0));
        value = null;

        OscParameter.SendValue(Address, new Vector2(1, 2));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(1, value.ReadFloatElementUnchecked(0));
        Assert.AreEqual(2, value.ReadFloatElementUnchecked(1));
        value = null;

        OscParameter.SendValue(Address, new Vector3(1, 2, 3));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(1, value.ReadFloatElementUnchecked(0));
        Assert.AreEqual(2, value.ReadFloatElementUnchecked(1));
        Assert.AreEqual(3, value.ReadFloatElementUnchecked(2));
        value = null;

        OscParameter.SendValue(Address, new Color32(1, 2, 3, 4));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(0x04030201, value.ReadIntElementUnchecked(0));
        value = null;

        OscParameter.SendValue(Address, new MidiMessage(1, 2, 3, 4));
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual(new MidiMessage(1, 2, 3, 4), value.ReadMidiElementUnchecked(0));
        value = null;

        OscParameter.SendValue(Address, new byte[] { 1, 2, 3, 4 });
        await LoopWhile(() => value == null, LatencyTimeout);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, value.ReadBlobElement(0));
        value = null;

        OscParameter.SendValue(Address, 'c');
        await LoopWhile(() => value == null, LatencyTimeout);
        Assert.AreEqual('c', value.ReadAsciiCharElement(0));
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

    [Test]
    public async Task ChangedEventArgsTest()
    {
        const string Address = "/test/address";
        ParameterChangedEventArgs expected = null;
        bool isCalledValueChanged = false;

        var parameters = OscParameter.Parameters;
        parameters.ValueChanged += valueChangedAssertion;

        await TestValueChangedEvent(new(null, 1, Address, ValueChangedReason.Added, ValueSource.VRChat), () => _client.Send(Address, 1));
        await TestValueChangedEvent(null, () => _client.Send(Address, 1));
        await TestValueChangedEvent(new(1, 2, Address, ValueChangedReason.Substituted, ValueSource.VRChat), () => _client.Send(Address, 2));
        await TestValueChangedEvent(new(2, 1, Address, ValueChangedReason.Substituted, ValueSource.VRChat), () => _client.Send(Address, 1));

        parameters.ValueChanged -= valueChangedAssertion;


        void valueChangedAssertion(IReadOnlyOscParameterCollection sender, ParameterChangedEventArgs e)
        {
            Assert.AreEqual(expected!.NewValue, e.NewValue);
            Assert.AreEqual(expected!.OldValue, e.OldValue);
            Assert.AreEqual(expected!.Reason, e.Reason);
            Assert.AreEqual(expected!.Address, e.Address);
            Assert.AreEqual(expected!.ValueSource, e.ValueSource);
            isCalledValueChanged = true;
        }

        async Task TestValueChangedEvent(ParameterChangedEventArgs expectedEventArgs, Action testAction)
        {
            expected = expectedEventArgs;
            testAction();
            await Task.Delay(LatencyTimeout);
            Assert.AreEqual(expectedEventArgs != null, isCalledValueChanged);
            isCalledValueChanged = false;
        }
    }
}
