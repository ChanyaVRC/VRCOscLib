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
            new("ValidParam1_IsGrabbed",   "Bool",  hasInput: true),
            new("ValidParam1_Angle",       "Float", hasInput: true),
            new("ValidParam1_Stretch",     "Float", hasInput: true),
            new("ValidParam2_IsGrabbed",   "Bool",  hasInput: false),
            new("ValidParam2_Angle",       "Float", hasInput: false),
            new("ValidParam2_Stretch",     "Float", hasInput: false),
            new("ValidParam3_Angle",       "Float", hasInput: true),
            new("ValidParam3_Stretch",     "Float", hasInput: true),
            new("ValidParam3_IsGrabbed",   "Bool",  hasInput: true),
            new("ValidParam3_SomeValue",   "Float", hasInput: true),

            new("InvalidParam1_IsGrabbed", "Bool",  hasInput: true),
            new("InvalidParam1_Angle",     "Int",   hasInput: true),
            new("InvalidParam1_Stretch",   "Float", hasInput: true),
            new("InvalidParam2_IsGrabbed", "Float", hasInput: true),
            new("InvalidParam2_Angle",     "Float", hasInput: true),
            new("InvalidParam2_Stretch",   "Float", hasInput: true),
            new("InvalidParam3_IsGrabbed", "Bool",  hasInput: true),
            new("InvalidParam3_Angle",     "Float", hasInput: true),
            new("InvalidParam3_Stretch",   "Bool",  hasInput: true),
            new("InvalidParam4_Angle",     "Float", hasInput: true),
            new("InvalidParam4_Stretch",   "Float", hasInput: true),
            new("InvalidParam5_IsGrabbed", "Bool",  hasInput: true),
            new("InvalidParam5_Stretch",   "Float", hasInput: true),
            new("InvalidParam6_SomeValue", "Bool",  hasInput: true),
            new("InvalidParam6_Angle",     "Float", hasInput: true),
            new("InvalidParam6_Stretch",   "Float", hasInput: true),
            new("InvalidParam7IsGrabbed",  "Bool",  hasInput: true),
            new("InvalidParam7Angle",      "Float", hasInput: true),
            new("InvalidParam7Stretch",    "Float", hasInput: true),
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
