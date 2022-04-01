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
        OscParameter.Parameters.Clear();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Directory.Delete(OscUtility.VRChatOscPath, true);
        Directory.CreateDirectory(OscUtility.VRChatOscPath);
    }

    [Test]
    public async Task TestAroundGetCurrentAvatarConfigPath()
    {
        Assert.IsNull(OscUtility.GetCurrentOscAvatarConfigPath());
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
        CollectionAssert.AreEqual(new[] { path1, path2 }, OscUtility.GetOscAvatarConfigPathes());
    }

    [TestCase(0)]
    [TestCase(8001)]
    [TestCase(65535)]
    public void TestSendPort(int port)
    {
        int oldPort = OscUtility.SendPort;
        var oldClient = OscUtility.Client;

        OscUtility.SendPort = port;
        Assert.AreEqual(port, OscUtility.SendPort);
        Assert.AreEqual(port, OscUtility.Client.Destination.Port);
        Assert.AreNotSame(oldClient, OscUtility.Client);

        OscUtility.SendPort = oldPort;
    }

    [TestCase(-1)]
    [TestCase(65536)]
    public void TestSendPortOutOfRange(int port)
    {
        int oldPort = OscUtility.SendPort;

        Assert.Throws<ArgumentOutOfRangeException>(() => OscUtility.SendPort = port);
        Assert.AreEqual(oldPort, OscUtility.SendPort);
    }

    [TestCase(0)]
    [TestCase(8001)]
    [TestCase(65535)]
    public void TestReceivePort(int port)
    {
        OscUtility.ReceivePort = port;
        Assert.AreEqual(port, OscUtility.ReceivePort);
        Assert.AreEqual(port, OscUtility.Server.Port);
    }

    [TestCase(-1)]
    [TestCase(65536)]
    public void TestReceivePortOutOfRange(int port)
    {
        int oldPort = OscUtility.ReceivePort;
        Assert.Throws<ArgumentOutOfRangeException>(() => OscUtility.ReceivePort = port);
        Assert.AreEqual(oldPort, OscUtility.ReceivePort);
        Assert.AreEqual(oldPort, OscUtility.Server.Port);
    }

    [TestCase("127.0.0.1")]
    [TestCase("192.168.1.1")]
    [TestCase("8.8.8.8")]
    public void TestVrcIPAddress(string ipAddress)
    {
        string oldAddress = OscUtility.VrcIPAddress;

        OscUtility.VrcIPAddress = ipAddress;
        Assert.AreEqual(ipAddress, OscUtility.VrcIPAddress);
        Assert.AreEqual(ipAddress, OscUtility.Client.Destination.Address.ToString());

        OscUtility.VrcIPAddress = oldAddress;
    }

    [TestCase("example.com")]
    [TestCase("ipaddress")]
    public void TestVrcIPAddressInvalidFormat(string ipAddress)
    {
        string oldAddress = OscUtility.VrcIPAddress;

        Assert.Throws<FormatException>(() => OscUtility.VrcIPAddress = ipAddress);
        Assert.AreEqual(oldAddress, OscUtility.VrcIPAddress);
    }

    [Test]
    public void TestClientIPAddressNull()
    {
        string oldAddress = OscUtility.VrcIPAddress;

        Assert.Throws<ArgumentNullException>(() => OscUtility.VrcIPAddress = null!);
        Assert.AreEqual(oldAddress, OscUtility.VrcIPAddress);
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
            Assert.AreEqual(1, value);

            client.Send("/value/send", 1);
            await LoopWhile(() => value == 1, LatencyTimeout);
            Assert.AreEqual(2, value);
        }

        OscUtility.ReceivePort = 54321;
        using (var client = new OscClient("127.0.0.1", 54321))
        {
            client.Send("/value/send", 1);
            await LoopWhile(() => value == 2, LatencyTimeout);
            Assert.AreEqual(3, value);
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
            Assert.AreEqual(OscUtility.VrcIPAddress, result.RemoteEndPoint.Address.ToString());
        }

        OscUtility.SendPort = 54321;
        using (var client = new UdpClient(54321))
        {
            OscParameter.SendValue("/value/send", 1);
            var result = await client.ReceiveAsync().WaitAsync(LatencyTimeout);
            Assert.AreEqual(OscUtility.VrcIPAddress, result.RemoteEndPoint.Address.ToString());
        }

        OscUtility.SendPort = oldPort;
    }

    private static IEnumerable<IPAddress> IPv4Addresses => Dns.GetHostAddresses(Dns.GetHostName(), AddressFamily.InterNetwork);

    [Test, TestCaseSource(nameof(IPv4Addresses))]
    public async Task TestVrcIPAddressWithSending(IPAddress address)
    {
        string oldAddress = OscUtility.VrcIPAddress;

        using (var client = new UdpClient(OscUtility.SendPort))
        {
            OscParameter.SendValue("/value/send", 1);
            var result = await client.ReceiveAsync().WaitAsync(LatencyTimeout);
            Assert.AreEqual("127.0.0.1", result.RemoteEndPoint.Address.ToString());
            
            OscUtility.VrcIPAddress = address.ToString();
            OscParameter.SendValue("/value/send", 1);
            result = await client.ReceiveAsync().WaitAsync(LatencyTimeout);
            Assert.AreEqual(address.ToString(), result.RemoteEndPoint.Address.ToString());
        }

        OscUtility.VrcIPAddress = oldAddress;
    }
}
