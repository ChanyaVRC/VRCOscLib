#nullable disable

using BuildSoft;
using BuildSoft.OscCore;
using BuildSoft.OscCore.UnityObjects;
using NUnit.Framework;

using static BuildSoft.VRChat.Osc.Test.Utility.TestHelper;

namespace BuildSoft.VRChat.Osc.Test;

[TestOf(typeof(OscParameter))]
public class OscParameterTests
{
    private OscClient _client;
    private OscServer _server;

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
        _client = new OscClient(OscConnectionSettings.VrcIPAddress, OscConnectionSettings.ReceivePort);
        _server = new OscServer(OscConnectionSettings.SendPort);
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
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadFloatElementUnchecked(0), Is.EqualTo(1.2f));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, 10);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadIntElementUnchecked(0), Is.EqualTo(10));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, 1234567890123456789);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadInt64ElementUnchecked(0), Is.EqualTo(1234567890123456789));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, true);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadBooleanElement(0), Is.EqualTo(true));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, false);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadBooleanElement(0), Is.EqualTo(false));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, "value");
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadStringElement(0), Is.EqualTo("value"));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, 10.1);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadFloat64ElementUnchecked(0), Is.EqualTo(10.1));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new Vector2(1, 2));
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadFloatElementUnchecked(0), Is.EqualTo(1));
        Assert.That(value.ReadFloatElementUnchecked(1), Is.EqualTo(2));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new Vector3(1, 2, 3));
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadFloatElementUnchecked(0), Is.EqualTo(1));
        Assert.That(value.ReadFloatElementUnchecked(1), Is.EqualTo(2));
        Assert.That(value.ReadFloatElementUnchecked(2), Is.EqualTo(3));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new Color32(1, 2, 3, 4));
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadIntElementUnchecked(0), Is.EqualTo(0x04030201));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, new MidiMessage(1, 2, 3, 4));
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadMidiElementUnchecked(0), Is.EqualTo(new MidiMessage(1, 2, 3, 4)));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, [1, 2, 3, 4]);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadBlobElement(0), Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, 'c');
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadAsciiCharElement(0), Is.EqualTo('c'));
        value = null;

        OscParameter.SendAvatarParameter(ParamName, (object)10);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadIntElementUnchecked(0), Is.EqualTo(10));
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
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadFloatElementUnchecked(0), Is.EqualTo(1.2f));
        value = null;

        OscParameter.SendValue(Address, 10);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadIntElementUnchecked(0), Is.EqualTo(10));
        value = null;

        OscParameter.SendValue(Address, 1234567890123456789);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadInt64ElementUnchecked(0), Is.EqualTo(1234567890123456789));
        value = null;

        OscParameter.SendValue(Address, true);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadBooleanElement(0), Is.EqualTo(true));
        value = null;

        OscParameter.SendValue(Address, false);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadBooleanElement(0), Is.EqualTo(false));
        value = null;

        OscParameter.SendValue(Address, "value");
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadStringElement(0), Is.EqualTo("value"));
        value = null;

        OscParameter.SendValue(Address, 10.1);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadFloat64ElementUnchecked(0), Is.EqualTo(10.1));
        value = null;

        OscParameter.SendValue(Address, new Vector2(1, 2));
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadFloatElementUnchecked(0), Is.EqualTo(1));
        Assert.That(value.ReadFloatElementUnchecked(1), Is.EqualTo(2));
        value = null;

        OscParameter.SendValue(Address, new Vector3(1, 2, 3));
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadFloatElementUnchecked(0), Is.EqualTo(1));
        Assert.That(value.ReadFloatElementUnchecked(1), Is.EqualTo(2));
        Assert.That(value.ReadFloatElementUnchecked(2), Is.EqualTo(3));
        value = null;

        OscParameter.SendValue(Address, new Color32(1, 2, 3, 4));
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadIntElementUnchecked(0), Is.EqualTo(0x04030201));
        value = null;

        OscParameter.SendValue(Address, new MidiMessage(1, 2, 3, 4));
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadMidiElementUnchecked(0), Is.EqualTo(new MidiMessage(1, 2, 3, 4)));
        value = null;

        OscParameter.SendValue(Address, [1, 2, 3, 4]);
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadBlobElement(0), Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
        value = null;

        OscParameter.SendValue(Address, 'c');
        await WaitWhile(() => value == null, LatencyTimeout);
        Assert.That(value.ReadAsciiCharElement(0), Is.EqualTo('c'));
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
        Assert.That(OscParameter.Items[Address], Is.EqualTo(value));
    }

    [TestCase(new byte[] { 1, 2, 3, 4 })]
    public void ParametersTest1Blob(byte[] value)
    {
        const string ParamName = "paramName";
        const string Address = OscConst.AvatarParameterAddressSpace + ParamName;
        OscParameter.SendAvatarParameter(ParamName, value);

        Assert.That((byte[])OscParameter.Items[Address]!, Is.EqualTo(value));
        Assert.That((byte[])OscParameter.GetValue(Address)!, Is.EqualTo(value));
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
        OscParameter.Items.Remove(Address);

        _client.Send(Address, (dynamic)value);

        await WaitWhile(() => !OscParameter.Items.ContainsKey(Address), LatencyTimeout);

        Assert.That(OscParameter.Items[Address], Is.EqualTo(value));
    }

    [TestCase(new byte[] { 1, 2, 3, 4 })]
    public async Task ParametersTest2Blob(byte[] value)
    {
        const string ParamName = "paramName";
        const string Address = OscConst.AvatarParameterAddressSpace + ParamName;
        OscParameter.Items.Remove(Address);

        _client.Send(Address, value, value.Length);

        await WaitWhile(() => !OscParameter.Items.ContainsKey(Address), LatencyTimeout);

        Assert.That((byte[])OscParameter.Items[Address]!, Is.EqualTo(value));
    }

    [Test]
    public async Task ChangedEventArgsTest()
    {
        const string Address = "/test/address";
        ParameterChangedEventArgs expected = null;
        var isCalledValueChanged = false;

        var parameters = OscParameter.Items;
        parameters.ValueChanged += valueChangedAssertion;

        await TestValueChangedEvent(new(null, 1, Address, ValueChangedReason.Added, ValueSource.VRChat), () => _client.Send(Address, 1));
        await TestValueChangedEvent(null, () => _client.Send(Address, 1));
        await TestValueChangedEvent(new(1, 2, Address, ValueChangedReason.Substituted, ValueSource.VRChat), () => _client.Send(Address, 2));
        await TestValueChangedEvent(new(2, 1, Address, ValueChangedReason.Substituted, ValueSource.VRChat), () => _client.Send(Address, 1));

        parameters.ValueChanged -= valueChangedAssertion;


        void valueChangedAssertion(IReadOnlyOscParameterCollection sender, ParameterChangedEventArgs e)
        {
            Assert.That(e.NewValue, Is.EqualTo(expected!.NewValue));
            Assert.That(e.OldValue, Is.EqualTo(expected!.OldValue));
            Assert.That(e.Reason, Is.EqualTo(expected!.Reason));
            Assert.That(e.Address, Is.EqualTo(expected!.Address));
            Assert.That(e.ValueSource, Is.EqualTo(expected!.ValueSource));
            isCalledValueChanged = true;
        }

        async Task TestValueChangedEvent(ParameterChangedEventArgs expectedEventArgs, Action testAction)
        {
            expected = expectedEventArgs;
            testAction();
            await Task.Delay(LatencyTimeout);
            Assert.That(isCalledValueChanged, Is.EqualTo(expectedEventArgs != null));
            isCalledValueChanged = false;
        }
    }
}
