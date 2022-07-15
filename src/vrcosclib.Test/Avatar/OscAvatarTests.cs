using BuildSoft.VRChat.Osc.Test;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Avatar.Test;

[TestOf(typeof(OscAvatar))]
public class OscAvatarTests
{
    [SetUp]
    public void Setup()
    {
        TestUtility.StashOscDirectory();
    }

    [TearDown]
    public void TearDown()
    {
        TestUtility.RestoreOscDirectory();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {

    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {

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
