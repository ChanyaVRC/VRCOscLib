using System.Net;
using System.Net.Sockets;
using BuildSoft.OscCore;
using NUnit.Framework;

using static BuildSoft.VRChat.Osc.Test.TestUtility;

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
        OscParameter.Parameters.Clear();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {

    }

    [Test]
    public async Task TestAroundGetCurrentAvatarConfigPath()
    {
        Assert.IsNull(OscUtility.GetCurrentOscAvatarConfigPath());
        Assert.ThrowsAsync<TaskCanceledException>(async () => await OscUtility.WaitAndGetCurrentOscAvatarConfigPathAsync(CanceledToken));

        const string TestAvatarId = "avtr_test_avatar_id";

        using (var client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort))
        {
            client.Send(OscConst.AvatarIdAddress, TestAvatarId);
            await LoopWhile(() => Avatar.OscAvatarUtility.CurrentAvatar.Id == null, LatencyTimeout);
        }

        Assert.Throws<FileNotFoundException>(() => OscUtility.GetCurrentOscAvatarConfigPath());
        Assert.ThrowsAsync<FileNotFoundException>(async () => await OscUtility.WaitAndGetCurrentOscAvatarConfigPathAsync());

        var testAvatarDirectory = Path.Combine(OscUtility.VRChatOscPath, @"usr_test_user_id", "Avatars");
        var path = CreateConfigFileForTest(TestAvatarId, "TestAvatar", testAvatarDirectory, true);

        var configPath = OscUtility.GetCurrentOscAvatarConfigPath();
        var configPathAsync = await OscUtility.WaitAndGetCurrentOscAvatarConfigPathAsync();

        Assert.AreEqual(path, configPath);
        Assert.AreEqual(path, configPathAsync);
    }

    [Test]
    public void TestGetAvatarConfigPath()
    {
        const string TestAvatarId = "avtr_test_avatar_id";
        Assert.Throws<FileNotFoundException>(() => OscUtility.GetOscAvatarConfigPath(TestAvatarId));

        var testAvatarDirectory = Path.Combine(OscUtility.VRChatOscPath, @"usr_test_user_id", "Avatars");
        Assert.Throws<FileNotFoundException>(() => OscUtility.GetOscAvatarConfigPath(TestAvatarId));

        var path = CreateConfigFileForTest(TestAvatarId, "TestAvatar", testAvatarDirectory, true);
        Assert.AreEqual(path, OscUtility.GetOscAvatarConfigPath(TestAvatarId));
    }

    [Test]
    public void TestGetCurrentAvatarConfigPathes()
    {
        CollectionAssert.IsEmpty(OscUtility.GetOscAvatarConfigPathes());

        var testAvatarDirectory = Path.Combine(OscUtility.VRChatOscPath, @"usr_test_user_id", "Avatars");
        Directory.CreateDirectory(testAvatarDirectory);

        CollectionAssert.IsEmpty(OscUtility.GetOscAvatarConfigPathes());

        var path1 = CreateConfigFileForTest("avtr_test_avatar_id1", "TestAvatar", testAvatarDirectory, true);
        CollectionAssert.AreEqual(new[] { path1 }, OscUtility.GetOscAvatarConfigPathes());
        var path2 = CreateConfigFileForTest("avtr_test_avatar_id2", "TestAvatar", testAvatarDirectory, true);
        CollectionAssert.AreEqual(new[] { path1, path2 }, OscUtility.GetOscAvatarConfigPathes().Sort());
    }

    [TestCase(0)]
    [TestCase(8001)]
    [TestCase(65535)]
    public void TestSendPort(int port)
    {
        int oldPort = OscConnectionSettings.SendPort;
        var oldClient = OscConnectionSettings.Client;

        OscConnectionSettings.SendPort = port;
        Assert.AreEqual(port, OscConnectionSettings.SendPort);
        Assert.AreEqual(port, OscConnectionSettings.Client.Destination.Port);
        Assert.AreNotSame(oldClient, OscConnectionSettings.Client);

        OscConnectionSettings.SendPort = oldPort;
    }

    [TestCase(-1)]
    [TestCase(65536)]
    public void TestSendPortOutOfRange(int port)
    {
        int oldPort = OscConnectionSettings.SendPort;

        Assert.Throws<ArgumentOutOfRangeException>(() => OscConnectionSettings.SendPort = port);
        Assert.AreEqual(oldPort, OscConnectionSettings.SendPort);
    }

    [TestCase(0)]
    [TestCase(8001)]
    [TestCase(65535)]
    public void TestReceivePort(int port)
    {
        OscConnectionSettings.ReceivePort = port;
        Assert.AreEqual(port, OscConnectionSettings.ReceivePort);
        Assert.AreEqual(port, OscConnectionSettings.Server.Port);
    }

    [TestCase(-1)]
    [TestCase(65536)]
    public void TestReceivePortOutOfRange(int port)
    {
        int oldPort = OscConnectionSettings.ReceivePort;
        Assert.Throws<ArgumentOutOfRangeException>(() => OscConnectionSettings.ReceivePort = port);
        Assert.AreEqual(oldPort, OscConnectionSettings.ReceivePort);
        Assert.AreEqual(oldPort, OscConnectionSettings.Server.Port);
    }

    [TestCase("127.0.0.1")]
    [TestCase("192.168.1.1")]
    [TestCase("8.8.8.8")]
    public void TestVrcIPAddress(string ipAddress)
    {
        string oldAddress = OscConnectionSettings.VrcIPAddress;

        OscConnectionSettings.VrcIPAddress = ipAddress;
        Assert.AreEqual(ipAddress, OscConnectionSettings.VrcIPAddress);
        Assert.AreEqual(ipAddress, OscConnectionSettings.Client.Destination.Address.ToString());

        OscConnectionSettings.VrcIPAddress = oldAddress;
    }

    [TestCase("example.com")]
    [TestCase("ipaddress")]
    public void TestVrcIPAddressInvalidFormat(string ipAddress)
    {
        string oldAddress = OscConnectionSettings.VrcIPAddress;

        Assert.Throws<FormatException>(() => OscConnectionSettings.VrcIPAddress = ipAddress);
        Assert.AreEqual(oldAddress, OscConnectionSettings.VrcIPAddress);
    }

    [Test]
    public void TestClientIPAddressNull()
    {
        string oldAddress = OscConnectionSettings.VrcIPAddress;

        Assert.Throws<ArgumentNullException>(() => OscConnectionSettings.VrcIPAddress = null!);
        Assert.AreEqual(oldAddress, OscConnectionSettings.VrcIPAddress);
    }

    [Test]
    public async Task TestRegisterMonitorCallback()
    {
        int value = 0;
        OscUtility.RegisterMonitorCallback((_, _) => value++);

        int oldPort = OscConnectionSettings.ReceivePort;

        OscConnectionSettings.ReceivePort = 12345;
        using (var client = new OscClient("127.0.0.1", 12345))
        {
            client.Send("/value/send", 1);
            await LoopWhile(() => value == 0, LatencyTimeout);
            Assert.AreEqual(1, value);

            client.Send("/value/send", 1);
            await LoopWhile(() => value == 1, LatencyTimeout);
            Assert.AreEqual(2, value);
        }

        OscConnectionSettings.ReceivePort = 54321;
        using (var client = new OscClient("127.0.0.1", 54321))
        {
            client.Send("/value/send", 1);
            await LoopWhile(() => value == 2, LatencyTimeout);
            Assert.AreEqual(3, value);
        }

        OscConnectionSettings.ReceivePort = oldPort;
    }

    [Test]
    public async Task TestSendPortWithSending()
    {
        int oldPort = OscConnectionSettings.SendPort;

        OscConnectionSettings.SendPort = 12345;
        using (var client = new UdpClient(12345))
        {
            OscParameter.SendValue("/value/send", 1);
            var result = await client.ReceiveAsync().WaitAsync(LatencyTimeout);
            Assert.AreEqual(OscConnectionSettings.VrcIPAddress, result.RemoteEndPoint.Address.ToString());
        }

        OscConnectionSettings.SendPort = 54321;
        using (var client = new UdpClient(54321))
        {
            OscParameter.SendValue("/value/send", 1);
            var result = await client.ReceiveAsync().WaitAsync(LatencyTimeout);
            Assert.AreEqual(OscConnectionSettings.VrcIPAddress, result.RemoteEndPoint.Address.ToString());
        }

        OscConnectionSettings.SendPort = oldPort;
    }

    [Test]
    public async Task TestVrcIPAddressWithSending()
    {
        string oldAddress = OscConnectionSettings.VrcIPAddress;

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
