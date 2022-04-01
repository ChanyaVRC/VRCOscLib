using BuildSoft.OscCore;
using NUnit.Framework;

using static BuildSoft.VRChat.Osc.Test.TestUtility;

namespace BuildSoft.VRChat.Osc.Test;

[TestOf(typeof(OscUtility))]
public class OscUtilityTests
{
    private OscClient _client = null!;
    private OscServer _server = null!;
    private int _defaultReceivePort;
    private int _defaultSendPort;
    private const int TestClientPort = 8001;
    private const int TestServerPort = 8002;

    [SetUp]
    public void Setup()
    {
        _defaultReceivePort = OscUtility.ReceivePort;
        _defaultSendPort = OscUtility.SendPort;

        _client = new OscClient("127.0.0.1", OscUtility.ReceivePort);
        _server = OscServer.GetOrCreate(OscUtility.SendPort);

        Directory.CreateDirectory(OscUtility.VRChatOscPath);
        Directory.Move(OscUtility.VRChatOscPath, OscUtility.VRChatOscPath + "_Renamed");
        Directory.CreateDirectory(OscUtility.VRChatOscPath);
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _server.Dispose();

        Directory.Delete(OscUtility.VRChatOscPath, true);
        Directory.Move(OscUtility.VRChatOscPath + "_Renamed", OscUtility.VRChatOscPath);
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        OscUtility.ReceivePort = TestClientPort;
        OscUtility.SendPort = TestServerPort;

        OscParameter.Parameters.Clear();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        OscUtility.ReceivePort = _defaultReceivePort;
        OscUtility.SendPort = _defaultSendPort;

        Directory.Delete(OscUtility.VRChatOscPath, true);
        Directory.CreateDirectory(OscUtility.VRChatOscPath);
    }

    [Test]
    public async Task TestAroundGetCurrentAvatarConfigPath()
    {
        Assert.IsNull(OscUtility.GetCurrentOscAvatarConfigPath());
        Assert.ThrowsAsync<TaskCanceledException>(async () => await OscUtility.WaitAndGetCurrentOscAvatarConfigPathAsync(CanceledToken));

        const string TestAvatarId = "avtr_test_avatar_id";
        _client.Send(OscConst.AvatarIdAddress, TestAvatarId);

        await LoopWhile(() => Avatar.OscAvatarUtility.CurrentAvatar.Id == null, LatencyTimeout);

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
        var oldClient = OscUtility.Client;

        Assert.Throws<ArgumentOutOfRangeException>(() => OscUtility.SendPort = port);
        Assert.AreEqual(oldPort, OscUtility.SendPort);
        Assert.AreSame(oldClient, OscUtility.Client);
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
        var _oldClient = OscUtility.Client;

        OscUtility.VrcIPAddress = ipAddress;
        Assert.AreEqual(ipAddress, OscUtility.VrcIPAddress);
        Assert.AreEqual(ipAddress, OscUtility.Client.Destination.Address.ToString());
        Assert.AreNotSame(_oldClient, OscUtility.Client);

        OscUtility.VrcIPAddress = oldAddress;
    }

    [TestCase("example.com")]
    [TestCase("ipaddress")]
    public void TestVrcIPAddressInvalidFormat(string ipAddress)
    {
        string oldAddress = OscUtility.VrcIPAddress;
        var _oldClient = OscUtility.Client;

        Assert.Throws<FormatException>(() => OscUtility.VrcIPAddress = ipAddress);
        Assert.AreEqual(oldAddress, OscUtility.VrcIPAddress);
        Assert.AreSame(_oldClient, OscUtility.Client);
    }

    [Test]
    public void TestClientIPAddressNull()
    {
        string oldAddress = OscUtility.VrcIPAddress;
        var _oldClient = OscUtility.Client;

        Assert.Throws<ArgumentNullException>(() => OscUtility.VrcIPAddress = null!);
        Assert.AreEqual(oldAddress, OscUtility.VrcIPAddress);
        Assert.AreSame(_oldClient, OscUtility.Client);
    }

    [Test]
    public async Task TestRegisterMonitorCallback()
    {
        int value = 0;
        OscUtility.RegisterMonitorCallback((_, _) => value++);

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
    }
}
