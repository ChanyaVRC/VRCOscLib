using System.Net;
using System.Net.Sockets;
using BuildSoft.OscCore;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test;

[TestOf(typeof(OscUtility))]
public class OscInitializeTest
{
    OscServer? _oscServer;
    [SetUp]
    public void Setup()
    {
        _oscServer = new(9001);
    }

    [TearDown]
    public void TearDown()
    {
        _oscServer?.Dispose();
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
    public void InitializeTest()
    {
        Assert.That(OscUtility.IsFailedAutoInitialization);
        Assert.Throws<SocketException>(() => OscUtility.Initialize());

        OscConnectionSettings.ReceivePort = 9003;
        Assert.DoesNotThrow(() => OscUtility.Initialize());
        Assert.That(OscUtility.IsFailedAutoInitialization);
    }
}
