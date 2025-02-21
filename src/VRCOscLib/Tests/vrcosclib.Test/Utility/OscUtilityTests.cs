using System.Net;
using System.Net.Sockets;
using BuildSoft.OscCore;
using NUnit.Framework;

using static BuildSoft.VRChat.Osc.Test.TestUtility;

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
        OscParameter.Parameters.Clear();
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

        using (var client = new OscClient("127.0.0.1", OscUtility.ReceivePort))
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
    public void TestGetCurrentAvatarConfigPathes()
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

    [TestCase(0)]
    [TestCase(8001)]
    [TestCase(65535)]
    public void TestSendPort(int port)
    {
        int oldPort = OscUtility.SendPort;
        var oldClient = OscUtility.Client;

        OscUtility.SendPort = port;
        Assert.That(OscUtility.SendPort, Is.EqualTo(port));
        Assert.That(OscUtility.Client.Destination.Port, Is.EqualTo(port));
        Assert.That(OscUtility.Client, Is.Not.SameAs(oldClient));

        OscUtility.SendPort = oldPort;
    }

    [TestCase(-1)]
    [TestCase(65536)]
    public void TestSendPortOutOfRange(int port)
    {
        int oldPort = OscUtility.SendPort;

        Assert.Throws<ArgumentOutOfRangeException>(() => OscUtility.SendPort = port);
        Assert.That(OscUtility.SendPort, Is.EqualTo(oldPort));
    }

    [TestCase(0)]
    [TestCase(8001)]
    [TestCase(65535)]
    public void TestReceivePort(int port)
    {
        OscUtility.ReceivePort = port;
        Assert.That(OscUtility.ReceivePort, Is.EqualTo(port));
        Assert.That(OscUtility.Server.Port, Is.EqualTo(port));
    }

    [TestCase(-1)]
    [TestCase(65536)]
    public void TestReceivePortOutOfRange(int port)
    {
        int oldPort = OscUtility.ReceivePort;
        Assert.Throws<ArgumentOutOfRangeException>(() => OscUtility.ReceivePort = port);
        Assert.That(OscUtility.ReceivePort, Is.EqualTo(oldPort));
        Assert.That(OscUtility.Server.Port, Is.EqualTo(oldPort));
    }

    [TestCase("127.0.0.1")]
    [TestCase("192.168.1.1")]
    [TestCase("8.8.8.8")]
    public void TestVrcIPAddress(string ipAddress)
    {
        string oldAddress = OscUtility.VrcIPAddress;

        OscUtility.VrcIPAddress = ipAddress;
        Assert.That(OscUtility.VrcIPAddress, Is.EqualTo(ipAddress));
        Assert.That(OscUtility.Client.Destination.Address.ToString(), Is.EqualTo(ipAddress));

        OscUtility.VrcIPAddress = oldAddress;
    }

    [TestCase("example.com")]
    [TestCase("ipaddress")]
    public void TestVrcIPAddressInvalidFormat(string ipAddress)
    {
        string oldAddress = OscUtility.VrcIPAddress;

        Assert.Throws<FormatException>(() => OscUtility.VrcIPAddress = ipAddress);
        Assert.That(OscUtility.VrcIPAddress, Is.EqualTo(oldAddress));
    }

    [Test]
    public void TestClientIPAddressNull()
    {
        string oldAddress = OscUtility.VrcIPAddress;

        Assert.Throws<ArgumentNullException>(() => OscUtility.VrcIPAddress = null!);
        Assert.That(OscUtility.VrcIPAddress, Is.EqualTo(oldAddress));
    }

    [Test]
    public async Task TestRegisterMonitorCallback()
    {
        int value = 0;
        OscUtility.RegisterMonitorCallback((_, _) => value++);

        int oldPort = OscUtility.ReceivePort;

        OscUtility.ReceivePort = 12345;
        using (var client = new OscClient("127.0.0.1", 12345))
        {
            client.Send("/value/send", 1);
            await LoopWhile(() => value == 0, LatencyTimeout);
            Assert.That(value, Is.EqualTo(1));

            client.Send("/value/send", 1);
            await LoopWhile(() => value == 1, LatencyTimeout);
            Assert.That(value, Is.EqualTo(2));
        }

        OscUtility.ReceivePort = 54321;
        using (var client = new OscClient("127.0.0.1", 54321))
        {
            client.Send("/value/send", 1);
            await LoopWhile(() => value == 2, LatencyTimeout);
            Assert.That(value, Is.EqualTo(3));
        }

        OscUtility.ReceivePort = oldPort;
    }

    [Test]
    public async Task TestSendPortWithSending()
    {
        int oldPort = OscUtility.SendPort;

        OscUtility.SendPort = 12345;
        using (var client = new UdpClient(12345))
        {
            OscParameter.SendValue("/value/send", 1);
            var result = await client.ReceiveAsync().WaitAsync(LatencyTimeout);
            Assert.That(result.RemoteEndPoint.Address.ToString(), Is.EqualTo(OscUtility.VrcIPAddress));
        }

        OscUtility.SendPort = 54321;
        using (var client = new UdpClient(54321))
        {
            OscParameter.SendValue("/value/send", 1);
            var result = await client.ReceiveAsync().WaitAsync(LatencyTimeout);
            Assert.That(result.RemoteEndPoint.Address.ToString(), Is.EqualTo(OscUtility.VrcIPAddress));
        }

        OscUtility.SendPort = oldPort;
    }

    [Test]
    public async Task TestVrcIPAddressWithSending()
    {
        string oldAddress = OscUtility.VrcIPAddress;

        using (var client = new UdpClient(new IPEndPoint(
                IPAddress.Parse("127.0.0.1"),
                OscUtility.SendPort)))
        {
            OscParameter.SendValue("/value/send", 1);
            await client.ReceiveAsync().WaitAsync(LatencyTimeout);
        }

        using (var client = new UdpClient(new IPEndPoint(
                IPAddress.Parse("127.0.0.2"),
                OscUtility.SendPort)))
        {
            OscUtility.VrcIPAddress = "127.0.0.2"; //127.0.0.1 to 127.0.0.2
            OscParameter.SendValue("/value/send", 2);
            await client.ReceiveAsync().WaitAsync(LatencyTimeout);
        }

        OscUtility.VrcIPAddress = oldAddress;
    }
}
