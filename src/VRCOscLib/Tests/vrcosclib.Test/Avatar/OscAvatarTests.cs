using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Avatar;
using BuildSoft.VRChat.Osc.Test.Utility;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test.Avatar;

[TestOf(typeof(OscAvatar))]
public class OscAvatarTests
{
    [SetUp]
    public void Setup()
    {
        TestHelper.StashOscDirectory();
    }

    [TearDown]
    public void TearDown()
    {
        TestHelper.RestoreOscDirectory();
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
        Assert.That(default(OscAvatar).ToConfig(), Is.Null);
        Assert.Throws<FileNotFoundException>(() => new OscAvatar { Id = AvatarId }.ToConfig());

        TestHelper.CreateConfigFileForTest(AvatarId, "Test Avatar", TestHelper.GetAvatarConfigDirectory());
        var config = new OscAvatar { Id = AvatarId }.ToConfig();
        Assert.That(config, Is.Not.Null);
        Assert.That(config!.Id, Is.EqualTo(AvatarId));
    }

    [Test]
    public void TestChange()
    {
        const string AvatarId = "avtr_id_for_test";
        Assert.Throws<InvalidOperationException>(() => default(OscAvatar).Change());
        using (new OscServer(OscConnectionSettings.SendPort))
        {
            Assert.DoesNotThrow(() => new OscAvatar { Id = AvatarId }.Change());
        }
        Assert.That(OscAvatarUtility.CurrentAvatar.Id, Is.EqualTo(AvatarId));
    }
}
