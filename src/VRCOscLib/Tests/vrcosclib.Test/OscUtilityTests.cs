using System.Net;
using System.Net.Sockets;
using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Avatar;
using BuildSoft.VRChat.Osc.Test.Utility;
using NUnit.Framework;

using static BuildSoft.VRChat.Osc.Test.Utility.TestHelper;

namespace BuildSoft.VRChat.Osc.Test;

[TestOf(typeof(OscUtility))]
public class OscUtilityTests
{
    [SetUp]
    public void Setup()
    {
        StashOscDirectory();
    }

    [TearDown]
    public void TearDown()
    {
        RestoreOscDirectory();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        OscParameter.Items.Clear();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {

    }

    [Test]
    public async Task TestAroundGetCurrentAvatarConfigPath()
    {
        Assert.That(OscUtility.GetCurrentOscAvatarConfigPath(), Is.Null);
        Assert.ThrowsAsync<TaskCanceledException>(async () => await OscUtility.WaitAndGetCurrentOscAvatarConfigPathAsync(CanceledToken));

        const string TestAvatarId = "avtr_test_avatar_id";

        using (var client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort))
        {
            client.Send(OscConst.AvatarIdAddress, TestAvatarId);
            await WaitWhile(() => OscAvatarUtility.CurrentAvatar.Id == null, LatencyTimeout);
        }

        Assert.Throws<FileNotFoundException>(() => OscUtility.GetCurrentOscAvatarConfigPath());
        Assert.ThrowsAsync<FileNotFoundException>(async () => await OscUtility.WaitAndGetCurrentOscAvatarConfigPathAsync());

        var testAvatarDirectory = Path.Combine(OscUtility.VRChatOscPath, @"usr_test_user_id", "Avatars");
        var path = CreateConfigFileForTest(TestAvatarId, "TestAvatar", testAvatarDirectory, true);

        var configPath = OscUtility.GetCurrentOscAvatarConfigPath();
        var configPathAsync = await OscUtility.WaitAndGetCurrentOscAvatarConfigPathAsync();

        Assert.That(configPath, Is.EqualTo(path));
        Assert.That(configPathAsync, Is.EqualTo(path));
    }

    [Test]
    public void TestGetAvatarConfigPath()
    {
        const string TestAvatarId = "avtr_test_avatar_id";
        Assert.Throws<FileNotFoundException>(() => OscUtility.GetOscAvatarConfigPath(TestAvatarId));

        var testAvatarDirectory = Path.Combine(OscUtility.VRChatOscPath, @"usr_test_user_id", "Avatars");
        Assert.Throws<FileNotFoundException>(() => OscUtility.GetOscAvatarConfigPath(TestAvatarId));

        var path = CreateConfigFileForTest(TestAvatarId, "TestAvatar", testAvatarDirectory, true);
        Assert.That(OscUtility.GetOscAvatarConfigPath(TestAvatarId), Is.EqualTo(path));
    }

    [Test]
    public void TestGetCurrentAvatarConfigPaths()
    {
        Assert.That(OscUtility.GetOscAvatarConfigPathes(), Is.Empty);

        var testAvatarDirectory = Path.Combine(OscUtility.VRChatOscPath, @"usr_test_user_id", "Avatars");
        Directory.CreateDirectory(testAvatarDirectory);

        Assert.That(OscUtility.GetOscAvatarConfigPathes(), Is.Empty);

        var path1 = CreateConfigFileForTest("avtr_test_avatar_id1", "TestAvatar", testAvatarDirectory, true);
        Assert.That(OscUtility.GetOscAvatarConfigPathes(), Is.EqualTo(new[] { path1 }));
        var path2 = CreateConfigFileForTest("avtr_test_avatar_id2", "TestAvatar", testAvatarDirectory, true);
        Assert.That(OscUtility.GetOscAvatarConfigPathes().Sort(), Is.EqualTo(new[] { path1, path2 }));
    }
}
