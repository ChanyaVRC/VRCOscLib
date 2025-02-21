using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Avatar;
using BuildSoft.VRChat.Osc.Test.Utility;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test.Avatar;

[TestOf(typeof(OscAvatarConfig))]
public class OscAvatarConfigTests
{
    public static readonly Random SharedRandom = new();

    private const string Id = "avtr_id";
    private const string Name = "avatar";
    private const int Hash = 123456;

    private readonly IEnumerable<OscAvatarParameter> _parameters =
    [
        new("param1", new("address/to/param1", OscType.Float), new("address/to/param1", OscType.Float)),
        new("param2", new("address/to/param2", OscType.Float), new("address/to/param2", OscType.Float)),
        new("param3", new("address/to/param3", OscType.Float), new("address/to/param3", OscType.Float)),
        new("param4", new("address/to/param4", OscType.Float), new("address/to/param4", OscType.Float)),
        new("param5", new("address/to/param5", OscType.Float), new("address/to/param5", OscType.Float)),
        new("param6", new("address/to/param6", OscType.Float), new("address/to/param6", OscType.Float)),
        new("param7", new("address/to/param7", OscType.Float), new("address/to/param7", OscType.Float)),
    ];

    private static IEnumerable<OscAvatarConfigJson[]> ConfigJsons
    {
        get
        {
            List<OscAvatarConfigJson> configs = [];
            List<OscAvatarParameterJson> parameters = [];

            for (var i = 0; i < 5; i++)
            {
                configs.Clear();
                for (var j = 0; j < i; j++)
                {
                    parameters.Clear();

                    var parameterCount = SharedRandom.Next(0, 10);
                    for (var k = 0; k < parameterCount; k++)
                    {
                        parameters.Add(new OscAvatarParameterJson($"param{k}", OscType.Float));
                    }
                    configs.Add(new OscAvatarConfigJson($"avtr_{j}", $"name{j}", [.. parameters], j));
                }
                yield return configs.ToArray();
            }
        }
    }


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
    public void OneTimeSetup()
    {
        OscUtility.Initialize();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {

    }


    [Test]
    public void CtorTest()
    {
        var config = new OscAvatarConfig(Id, Name, _parameters);

        Assert.That(config.Id, Is.EqualTo(Id));
        Assert.That(config.Name, Is.EqualTo(Name));
        Assert.That(config.Parameters.Items, Is.EquivalentTo(_parameters));
    }


    [TestCaseSource(nameof(ConfigJsons))]
    public void CreateAllTest(OscAvatarConfigJson[] configJsons)
    {
        var directory = Path.Combine(OscUtility.VRChatOscPath, "Avatars");
        foreach (var config in configJsons)
        {
            TestHelper.CreateConfigFileForTest(config, directory);
        }

        var result = OscAvatarConfig.CreateAll();

        Assert.That(result, Is.All.Not.Null);
        Assert.That(result.Select(v => (v.Id, v.Name, v.Hash)), 
            Is.EquivalentTo(configJsons.Select(v => (v.id, v.name, v.hash))));

        foreach (var config in result)
        {
            var expected = configJsons.First(v => v.id == config.Id).parameters.Select(v => v.name);
            var actual = config.Parameters.Items.Select(v => v.Name);
            Assert.That(actual, Is.EquivalentTo(actual));
        }
    }

    [Test]
    public async Task CreateAtCurrentTest_CurrentIsNull()
    {
        await MakeCurrentAvatarIdToNull();

        var currentConfig = OscAvatarConfig.CreateAtCurrent();

        Assert.That(currentConfig, Is.Null);
    }

    [Test]
    public async Task CreateAtCurrentTest_DoesntHaveCurrentAvatarFile()
    {
        using (var client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort))
        {
            client.Send(OscConst.AvatarIdAddress, Id);
            await TestHelper.WaitWhile(() => OscAvatarUtility.CurrentAvatar.Id != Id, TestHelper.LatencyTimeout);
        }

        Assert.Throws<FileNotFoundException>(() => OscAvatarConfig.CreateAtCurrent());
    }

    [Test]
    public async Task CreateAtCurrentTest_HasCurrentAvatarFile()
    {
        using (var client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort))
        {
            client.Send(OscConst.AvatarIdAddress, Id);
            await TestHelper.WaitWhile(() => OscAvatarUtility.CurrentAvatar.Id != Id, TestHelper.LatencyTimeout);
        }
        TestHelper.CreateConfigFileForTest(Id, Name, Path.Combine(OscUtility.VRChatOscPath, "Avatar"), Hash);

        var currentConfig = OscAvatarConfig.CreateAtCurrent();

        Assert.That(currentConfig, Is.Not.Null);
        Assert.That(currentConfig!.Id, Is.EqualTo(Id));
        Assert.That(currentConfig!.Name, Is.EqualTo(Name));
        Assert.That(currentConfig!.Hash, Is.EqualTo(Hash));
    }

    [Test]
    public void CreateTest()
    {
        Assert.Throws<FileNotFoundException>(() => OscAvatarConfig.Create(Id));

        TestHelper.CreateConfigFileForTest(Id, Name, Path.Combine(OscUtility.VRChatOscPath, "Avatar"), Hash);

        var config = OscAvatarConfig.Create(Id);

        Assert.That(config, Is.Not.Null);
        Assert.That(config!.Id, Is.EqualTo(Id));
        Assert.That(config!.Name, Is.EqualTo(Name));
        Assert.That(config!.Hash, Is.EqualTo(Hash));
    }

    [Test]
    public async Task WaitAndCreateAtCurrentAsync_ForExceptions()
    {
        var timeout = TimeSpan.FromMilliseconds(1);

        await MakeCurrentAvatarIdToNull();
        Assert.ThrowsAsync<TimeoutException>(async () => await OscAvatarConfig.WaitAndCreateAtCurrentAsync().AsTask().WaitAsync(timeout));

        using (var client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort))
        {
            client.Send(OscConst.AvatarIdAddress, Id);
            await TestHelper.WaitWhile(() => OscAvatarUtility.CurrentAvatar.Id != Id, TestHelper.LatencyTimeout);
        }
        Assert.ThrowsAsync<FileNotFoundException>(async () => await OscAvatarConfig.WaitAndCreateAtCurrentAsync().AsTask().WaitAsync(timeout));

        TestHelper.CreateConfigFileForTest(Id, Name, Path.Combine(OscUtility.VRChatOscPath, "Avatar"), Hash);
        Assert.DoesNotThrowAsync(async () => await OscAvatarConfig.WaitAndCreateAtCurrentAsync().AsTask().WaitAsync(timeout));
    }

    [Test]
    public async Task WaitAndCreateAtCurrentAsyncTest_ForProperties()
    {
        using (var client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort))
        {
            client.Send(OscConst.AvatarIdAddress, Id);
            await TestHelper.WaitWhile(() => OscAvatarUtility.CurrentAvatar.Id != Id, TestHelper.LatencyTimeout);
        }
        TestHelper.CreateConfigFileForTest(Id, Name, Path.Combine(OscUtility.VRChatOscPath, "Avatar"), Hash);

        var config = await OscAvatarConfig.WaitAndCreateAtCurrentAsync();

        Assert.That(config, Is.Not.Null);
        Assert.That(config!.Id, Is.EqualTo(Id));
        Assert.That(config!.Name, Is.EqualTo(Name));
        Assert.That(config!.Hash, Is.EqualTo(Hash));
    }

    [Test]
    public void TestChange()
    {
        const string AvatarId = "avtr_change_test";

        var config = new OscAvatarConfig(AvatarId, Name, _parameters);
        using (new OscServer(OscConnectionSettings.SendPort))
        {
            Assert.DoesNotThrow(() => config.Change());
        }
        Assert.That(OscAvatarUtility.CurrentAvatar.Id, Is.EqualTo(AvatarId));
    }


    private static async ValueTask MakeCurrentAvatarIdToNull()
    {
        if (OscAvatarUtility.CurrentAvatar.Id != null)
        {
            using var client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort);
            client.SendNil(OscConst.AvatarIdAddress);
            await TestHelper.WaitWhile(() => OscAvatarUtility.CurrentAvatar.Id != null, TestHelper.LatencyTimeout);
        }
    }
}
