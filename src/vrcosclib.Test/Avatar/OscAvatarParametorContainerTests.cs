using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Test;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Avatar.Test;

[TestOf(typeof(OscAvatarParametorContainer))]
public class OscAvatarParametorContainerTest
{
    private const string AvatarId = "avtr_TestAvatar";
    string _configFile = null!;
    OscAvatarConfig _config = null!;
    OscClient _client = null!;
    private OscAvatarParametorContainer _parameters = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _configFile = TestUtility.CreateConfigFileForTest(new(AvatarId, "TestAvatar", new OscAvatarParameterJson[]
        {
            new("ValidParam1_IsGrabbed",   OscType.Bool,  hasInput: true),
            new("ValidParam1_Angle",       OscType.Float, hasInput: true),
            new("ValidParam1_Stretch",     OscType.Float, hasInput: true),
            new("ValidParam2_IsGrabbed",   OscType.Bool,  hasInput: false),
            new("ValidParam2_Angle",       OscType.Float, hasInput: false),
            new("ValidParam2_Stretch",     OscType.Float, hasInput: false),
            new("ValidParam3_Angle",       OscType.Float, hasInput: true),
            new("ValidParam3_Stretch",     OscType.Float, hasInput: true),
            new("ValidParam3_IsGrabbed",   OscType.Bool,  hasInput: true),
            new("ValidParam3_SomeValue",   OscType.Float, hasInput: true),

            new("InvalidParam1_IsGrabbed", OscType.Bool,  hasInput: true),
            new("InvalidParam1_Angle",     OscType.Int,   hasInput: true),
            new("InvalidParam1_Stretch",   OscType.Float, hasInput: true),
            new("InvalidParam2_IsGrabbed", OscType.Float, hasInput: true),
            new("InvalidParam2_Angle",     OscType.Float, hasInput: true),
            new("InvalidParam2_Stretch",   OscType.Float, hasInput: true),
            new("InvalidParam3_IsGrabbed", OscType.Bool,  hasInput: true),
            new("InvalidParam3_Angle",     OscType.Float, hasInput: true),
            new("InvalidParam3_Stretch",   OscType.Bool,  hasInput: true),
            new("InvalidParam4_Angle",     OscType.Float, hasInput: true),
            new("InvalidParam4_Stretch",   OscType.Float, hasInput: true),
            new("InvalidParam5_IsGrabbed", OscType.Bool,  hasInput: true),
            new("InvalidParam5_Stretch",   OscType.Float, hasInput: true),
            new("InvalidParam6_SomeValue", OscType.Bool,  hasInput: true),
            new("InvalidParam6_Angle",     OscType.Float, hasInput: true),
            new("InvalidParam6_Stretch",   OscType.Float, hasInput: true),
            new("InvalidParam7IsGrabbed",  OscType.Bool,  hasInput: true),
            new("InvalidParam7Angle",      OscType.Float, hasInput: true),
            new("InvalidParam7Stretch",    OscType.Float, hasInput: true),

            new("TestParam",               OscType.Float, hasInput: true),
        }), Path.Combine(OscUtility.VRChatOscPath, "Test"));

        _config = new OscAvatar { Id = AvatarId }.ToConfig()!;
        _client = new OscClient("127.0.0.1", OscUtility.ReceivePort);
        _parameters = _config.Parameters;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Directory.Delete(Path.GetDirectoryName(_configFile)!, true);
        _client.Dispose();
    }

    [Test]
    public async Task ParameterChangedTest()
    {
        int newValue = 100;
        bool isCalled = false;

        _parameters.ParameterChanged += Handler;

        _client.Send(OscConst.AvatarParameterAddressSpace + "TestParam", newValue);
        await TestUtility.LoopWhile(() => !isCalled, TestUtility.LatencyTimeout);

        _parameters.ParameterChanged -= Handler;

        void Handler(OscAvatarParameter param, ValueChangedEventArgs e)
        {
            Assert.AreEqual("TestParam", param.Name);
            Assert.IsNull(e.OldValue);
            Assert.AreEqual(newValue, e.NewValue);
            isCalled = true;
        }
    }

    [Test]
    public void PhysBonesTest()
    {
        var physbones = _parameters.PhysBones;

        Assert.IsNotNull(physbones);
        CollectionAssert.AreEquivalent(
            new[] { "ValidParam1", "ValidParam2", "ValidParam3", },
            physbones.Select(v => v.ParamName));
    }
}
