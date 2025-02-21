using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Avatar;
using BuildSoft.VRChat.Osc.Test.Utility;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test.Avatar;

[TestOf(typeof(OscAvatarParameterContainer))]
public class OscAvatarParameterContainerTests

{
    private const string AvatarId = "avtr_TestAvatar";
    string _configFile = null!;
    OscAvatarConfig _config = null!;
    OscClient _client = null!;
    private OscAvatarParameterContainer _parameters = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _configFile = TestHelper.CreateConfigFileForTest(new(AvatarId, "TestAvatar",
        [
            new("ValidParam1_IsGrabbed",   OscType.Bool,  hasInput: true),
            new("ValidParam1_IsPosed",     OscType.Bool,  hasInput: true),
            new("ValidParam1_Angle",       OscType.Float, hasInput: true),
            new("ValidParam1_Stretch",     OscType.Float, hasInput: true),
            new("ValidParam1_Squish",      OscType.Float, hasInput: true),
            new("ValidParam2_IsGrabbed",   OscType.Bool,  hasInput: false),
            new("ValidParam2_IsPosed",     OscType.Bool,  hasInput: false),
            new("ValidParam2_Angle",       OscType.Float, hasInput: false),
            new("ValidParam2_Stretch",     OscType.Float, hasInput: false),
            new("ValidParam2_Squish",      OscType.Float, hasInput: false),
            new("ValidParam3_Angle",       OscType.Float, hasInput: true),
            new("ValidParam3_Stretch",     OscType.Float, hasInput: true),
            new("ValidParam3_IsGrabbed",   OscType.Bool,  hasInput: true),
            new("ValidParam3_SomeValue",   OscType.Float, hasInput: true),
            new("ValidParam3_IsPosed",     OscType.Bool,  hasInput: true),
            new("ValidParam3_Squish",      OscType.Float, hasInput: true),

            new("InvalidParam1_IsGrabbed", OscType.Bool,  hasInput: true),
            new("InvalidParam1_Angle",     OscType.Int,   hasInput: true),
            new("InvalidParam1_Stretch",   OscType.Float, hasInput: true),
            new("InvalidParam1_IsPosed",   OscType.Bool,  hasInput: true),
            new("InvalidParam1_Squish",    OscType.Float, hasInput: true),
            new("InvalidParam2_IsGrabbed", OscType.Float, hasInput: true),
            new("InvalidParam2_Angle",     OscType.Float, hasInput: true),
            new("InvalidParam2_Stretch",   OscType.Float, hasInput: true),
            new("InvalidParam2_IsPosed",   OscType.Bool,  hasInput: true),
            new("InvalidParam2_Squish",    OscType.Float, hasInput: true),
            new("InvalidParam3_IsGrabbed", OscType.Bool,  hasInput: true),
            new("InvalidParam3_Angle",     OscType.Float, hasInput: true),
            new("InvalidParam3_Stretch",   OscType.Bool,  hasInput: true),
            new("InvalidParam3_IsPosed",   OscType.Bool,  hasInput: true),
            new("InvalidParam3_Squish",    OscType.Float, hasInput: true),
            new("InvalidParam4_Angle",     OscType.Float, hasInput: true),
            new("InvalidParam4_Stretch",   OscType.Float, hasInput: true),
            new("InvalidParam4_IsPosed",   OscType.Bool,  hasInput: true),
            new("InvalidParam4_Squish",    OscType.Float, hasInput: true),
            new("InvalidParam5_IsGrabbed", OscType.Bool,  hasInput: true),
            new("InvalidParam5_Stretch",   OscType.Float, hasInput: true),
            new("InvalidParam5_IsPosed",   OscType.Bool,  hasInput: true),
            new("InvalidParam5_Squish",    OscType.Float, hasInput: true),
            new("InvalidParam6_SomeValue", OscType.Bool,  hasInput: true),
            new("InvalidParam6_Angle",     OscType.Float, hasInput: true),
            new("InvalidParam6_Stretch",   OscType.Float, hasInput: true),
            new("InvalidParam6_IsPosed",   OscType.Bool,  hasInput: true),
            new("InvalidParam6_Squish",    OscType.Float, hasInput: true),
            new("InvalidParam7IsGrabbed",  OscType.Bool,  hasInput: true),
            new("InvalidParam7Angle",      OscType.Float, hasInput: true),
            new("InvalidParam7Stretch",    OscType.Float, hasInput: true),
            new("InvalidParam7IsPosed",    OscType.Bool,  hasInput: true),
            new("InvalidParam7Squish",     OscType.Float, hasInput: true),

            new("TestParam",               OscType.Float, hasInput: true),
        ]), Path.Combine(OscUtility.VRChatOscPath, "Test"));

        _config = new OscAvatar { Id = AvatarId }.ToConfig()!;
        _client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort);
        _parameters = _config.Parameters;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Directory.Delete(Path.GetDirectoryName(_configFile)!, true);
        _client.Dispose();
    }

    [SetUp]
    public void SetUp()
    {
        OscParameter.Items.Clear();
    }


    [Test]
    public async Task ParameterChangedTest()
    {
        var newValue = 100;
        var isCalled = false;

        _parameters.ParameterChanged += Handler;

        _client.Send(OscConst.AvatarParameterAddressSpace + "TestParam", newValue);
        await TestHelper.WaitWhile(() => !isCalled, TestHelper.LatencyTimeout);

        _parameters.ParameterChanged -= Handler;

        void Handler(OscAvatarParameter param, ValueChangedEventArgs e)
        {
            Assert.That(param.Name, Is.EqualTo(param.Name));
            Assert.That(e.OldValue, Is.Null);
            Assert.That(e.NewValue, Is.EqualTo(newValue));
            isCalled = true;
        }
    }

    [Test]
    public void PhysBonesTest()
    {
        var physBones = _parameters.PhysBones;

        Assert.That(physBones, Is.Not.Null);
        Assert.That(
            physBones.Select(v => v.ParamName),
            Is.EquivalentTo(["ValidParam1", "ValidParam2", "ValidParam3"]));
    }

    [Test]
    public void Get_ExistParameterTest()
    {
        var param = _parameters.Get("TestParam");
        Assert.That(param.Name, Is.EqualTo("TestParam"));
        Assert.That(param.Input!.OscType, Is.EqualTo(OscType.Float));
        Assert.That(param.Output!.OscType, Is.EqualTo(OscType.Float));
    }

    [Test]
    public void Get_NotExistParameterTest()
    {
        Assert.Throws<InvalidOperationException>(() => _parameters.Get("NotTestParam"));
    }

    [Test]
    public async Task OnParameterChanged_NotExistParameterReceivedTest()
    {
        var isCalled = false;

        _parameters.ParameterChanged += ThrowExceptionHandler;
        _parameters.ParameterChanged += MonitorCalledHandler;

        _client.Send(OscConst.AvatarParameterAddressSpace + "TestParam", 1);
        await TestHelper.WaitWhile(() => !isCalled, TestHelper.LatencyTimeout);
        isCalled = false;

        _client.Send(OscConst.AvatarParameterAddressSpace + "TestParam", 2);
        await TestHelper.WaitWhile(() => !isCalled, TestHelper.LatencyTimeout);
        isCalled = false;

        _parameters.ParameterChanged -= ThrowExceptionHandler;
        _parameters.ParameterChanged -= MonitorCalledHandler;

        void ThrowExceptionHandler(OscAvatarParameter param, ValueChangedEventArgs e) => throw new Exception();
        void MonitorCalledHandler(OscAvatarParameter param, ValueChangedEventArgs e) => isCalled = true;
    }
}
