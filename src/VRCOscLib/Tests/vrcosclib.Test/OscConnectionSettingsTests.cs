using System.Net;
using System.Net.Sockets;
using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Avatar;
using BuildSoft.VRChat.Osc.Test.Utility;
using NUnit.Framework;

using static BuildSoft.VRChat.Osc.Test.Utility.TestHelper;

namespace BuildSoft.VRChat.Osc.Test;

[TestOf(typeof(OscConnectionSettings))]
public class OscConnectionSettingsTests
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
        var path = CreateConfigFileForTest(TestAvatarId, "TestAvatar", testAvatarDirectory, 123456, true);

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

        var path = CreateConfigFileForTest(TestAvatarId, "TestAvatar", testAvatarDirectory, 123456, true);
        Assert.That(OscUtility.GetOscAvatarConfigPath(TestAvatarId), Is.EqualTo(path));
    }

    [Test]
    public void TestGetCurrentAvatarConfigPaths()
    {
        Assert.That(OscUtility.GetOscAvatarConfigPathes(), Is.Empty);

        var testAvatarDirectory = Path.Combine(OscUtility.VRChatOscPath, @"usr_test_user_id", "Avatars");
        Directory.CreateDirectory(testAvatarDirectory);

        Assert.That(OscUtility.GetOscAvatarConfigPathes(), Is.Empty);

        var path1 = CreateConfigFileForTest("avtr_test_avatar_id1", "TestAvatar", testAvatarDirectory, 123456, true);
        Assert.That(OscUtility.GetOscAvatarConfigPathes(), Is.EqualTo(new[] { path1 }));
        var path2 = CreateConfigFileForTest("avtr_test_avatar_id2", "TestAvatar", testAvatarDirectory, 234567, true);
        Assert.That(OscUtility.GetOscAvatarConfigPathes().Sort(), Is.EqualTo(new[] { path1, path2 }));
    }

    [TestCase(0)]
    [TestCase(8001)]
    [TestCase(65535)]
    public void TestSendPort(int port)
    {
        var oldPort = OscConnectionSettings.SendPort;
        var oldClient = OscConnectionSettings.Client;

        OscConnectionSettings.SendPort = port;
        Assert.That(OscConnectionSettings.SendPort, Is.EqualTo(port));
        Assert.That(OscConnectionSettings.Client.Destination.Port, Is.EqualTo(port));
        Assert.That(OscConnectionSettings.Client, Is.Not.SameAs(oldClient));

        OscConnectionSettings.SendPort = oldPort;
    }

    [TestCase(-1)]
    [TestCase(65536)]
    public void TestSendPortOutOfRange(int port)
    {
        var oldPort = OscConnectionSettings.SendPort;

        Assert.Throws<ArgumentOutOfRangeException>(() => OscConnectionSettings.SendPort = port);
        Assert.That(OscConnectionSettings.SendPort, Is.EqualTo(oldPort));
    }

    [TestCase(0)]
    [TestCase(8001)]
    [TestCase(65535)]
    public void TestReceivePort(int port)
    {
        OscConnectionSettings.ReceivePort = port;
        Assert.That(OscConnectionSettings.ReceivePort, Is.EqualTo(port));
        Assert.That(OscConnectionSettings.Server.Port, Is.EqualTo(port));
    }

    [TestCase(-1)]
    [TestCase(65536)]
    public void TestReceivePortOutOfRange(int port)
    {
        var oldPort = OscConnectionSettings.ReceivePort;
        Assert.Throws<ArgumentOutOfRangeException>(() => OscConnectionSettings.ReceivePort = port);
        Assert.That(OscConnectionSettings.ReceivePort, Is.EqualTo(oldPort));
        Assert.That(OscConnectionSettings.Server.Port, Is.EqualTo(oldPort));
    }

    [TestCase("127.0.0.1")]
    [TestCase("192.168.1.1")]
    [TestCase("8.8.8.8")]
    public void TestVrcIPAddress(string ipAddress)
    {
        var oldAddress = OscConnectionSettings.VrcIPAddress;

        OscConnectionSettings.VrcIPAddress = ipAddress;
        Assert.That(OscConnectionSettings.VrcIPAddress, Is.EqualTo(ipAddress));
        Assert.That(OscConnectionSettings.Client.Destination.Address.ToString(), Is.EqualTo(ipAddress));

        OscConnectionSettings.VrcIPAddress = oldAddress;
    }

    [TestCase("example.com")]
    [TestCase("ipaddress")]
    public void TestVrcIPAddressInvalidFormat(string ipAddress)
    {
        var oldAddress = OscConnectionSettings.VrcIPAddress;

        Assert.Throws<FormatException>(() => OscConnectionSettings.VrcIPAddress = ipAddress);
        Assert.That(OscConnectionSettings.VrcIPAddress, Is.EqualTo(oldAddress));
    }

    [Test]
    public void TestClientIPAddressNull()
    {
        var oldAddress = OscConnectionSettings.VrcIPAddress;

        Assert.Throws<ArgumentNullException>(() => OscConnectionSettings.VrcIPAddress = null!);
        Assert.That(OscConnectionSettings.VrcIPAddress, Is.EqualTo(oldAddress));
    }

    [Test]
    public async Task TestRegisterMonitorCallback()
    {
        var value = 0;
        OscUtility.RegisterMonitorCallback((_, _) => value++);

        var oldPort = OscConnectionSettings.ReceivePort;

        OscConnectionSettings.ReceivePort = 12345;
        using (var client = new OscClient("127.0.0.1", 12345))
        {
            client.Send("/value/send", 1);
            await WaitWhile(() => value == 0, LatencyTimeout);
            Assert.That(value, Is.EqualTo(1));

            client.Send("/value/send", 1);
            await WaitWhile(() => value == 1, LatencyTimeout);
            Assert.That(value, Is.EqualTo(2));
        }

        OscConnectionSettings.ReceivePort = 54321;
        using (var client = new OscClient("127.0.0.1", 54321))
        {
            client.Send("/value/send", 1);
            await WaitWhile(() => value == 2, LatencyTimeout);
            Assert.That(value, Is.EqualTo(3));
        }

        OscConnectionSettings.ReceivePort = oldPort;
    }

    [Test]
    public async Task TestSendPortWithSending()
    {
        var oldPort = OscConnectionSettings.SendPort;

        OscConnectionSettings.SendPort = 12345;
        using (var client = new UdpClient(12345))
        {
            OscParameter.SendValue("/value/send", 1);
            var result = await client.ReceiveAsync().WaitAsync(LatencyTimeout);
            Assert.That(result.RemoteEndPoint.Address.ToString(), Is.EqualTo(OscConnectionSettings.VrcIPAddress));
        }

        OscConnectionSettings.SendPort = 54321;
        using (var client = new UdpClient(54321))
        {
            OscParameter.SendValue("/value/send", 1);
            var result = await client.ReceiveAsync().WaitAsync(LatencyTimeout);
            Assert.That(result.RemoteEndPoint.Address.ToString(), Is.EqualTo(OscConnectionSettings.VrcIPAddress));
        }

        OscConnectionSettings.SendPort = oldPort;
    }

    [Test]
    public async Task TestVrcIPAddressWithSending()
    {
        var oldAddress = OscConnectionSettings.VrcIPAddress;

        using (var client = new UdpClient(new IPEndPoint(
                IPAddress.Parse("127.0.0.1"),
                OscConnectionSettings.SendPort)))
        {
            OscParameter.SendValue("/value/send", 1);
            await client.ReceiveAsync().WaitAsync(LatencyTimeout);
        }

        using (var client = new UdpClient(new IPEndPoint(
                IPAddress.Parse("127.0.0.2"),
                OscConnectionSettings.SendPort)))
        {
            OscConnectionSettings.VrcIPAddress = "127.0.0.2"; //127.0.0.1 to 127.0.0.2
            OscParameter.SendValue("/value/send", 2);
            await client.ReceiveAsync().WaitAsync(LatencyTimeout);
        }

        OscConnectionSettings.VrcIPAddress = oldAddress;
    }
}
