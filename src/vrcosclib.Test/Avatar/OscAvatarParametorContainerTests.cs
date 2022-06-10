using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Test;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace BuildSoft.VRChat.Osc.Avatar.Test;

[TestOf(typeof(OscAvatarParametorContainer))]
public class OscAvatarParametorContainerTest
{
    private const string AvatarId = "avtr_TestAvatar";
    ImmutableArray<OscAvatarParameter> _parameters;
    string _configFile = null!;
    OscAvatarConfig _config = null!;
    OscClient _client = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _configFile = TestUtility.CreateConfigFileForTest(AvatarId, "TestAvatar", Path.Combine(OscUtility.VRChatOscPath, "Test"));
        _config = new OscAvatar { Id = AvatarId }.ToConfig()!;
        _client = new OscClient("127.0.0.1", OscUtility.ReceivePort);
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
        var parameters = _config.Parameters;
        int newValue = 100;
        bool isCalled = false;

        parameters.ParameterChanged += Handler;

        _client.Send(OscConst.AvatarParameterAddressSpace + "TestParam", newValue);
        await TestUtility.LoopWhile(() => !isCalled, TestUtility.LatencyTimeout);

        parameters.ParameterChanged -= Handler;

        void Handler(OscAvatarParameter param, ValueChangedEventArgs e)
        {
            Assert.AreEqual("TestParam", param.Name);
            Assert.IsNull(e.OldValue);
            Assert.AreEqual(newValue, e.NewValue);
            isCalled = true;
        }
    }
}
