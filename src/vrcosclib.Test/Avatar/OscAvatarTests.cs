using BuildSoft.VRChat.Osc.Test;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Avatar.Test;

[TestOf(typeof(OscAvatar))]
public class OscAvatarTests
{
    [SetUp]
    public void Setup()
    {
        Directory.CreateDirectory(OscUtility.VRChatOscPath);
        Directory.Move(OscUtility.VRChatOscPath, OscUtility.VRChatOscPath + "_Renamed");
        Directory.CreateDirectory(OscUtility.VRChatOscPath);
    }

    [TearDown]
    public void TearDown()
    {
        Directory.Delete(OscUtility.VRChatOscPath, true);
        Directory.Move(OscUtility.VRChatOscPath + "_Renamed", OscUtility.VRChatOscPath);
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {

    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Directory.Delete(OscUtility.VRChatOscPath, true);
        Directory.CreateDirectory(OscUtility.VRChatOscPath);
    }

    [Test]
    public void TestToConfig()
    {
        const string AvatarId = "avtr_id_for_test";
        Assert.AreEqual(null, default(OscAvatar).ToConfig());
        Assert.Throws<FileNotFoundException>(() => new OscAvatar { Id = AvatarId }.ToConfig());

        TestUtility.CreateConfigFileForTest(AvatarId, "Test Avatar", TestUtility.GetAvatarConfigDirectory());
        var config = new OscAvatar { Id = AvatarId }.ToConfig();
        Assert.IsNotNull(config);
        Assert.AreEqual(AvatarId, config!.Id);
    }
}
