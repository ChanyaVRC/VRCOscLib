using BuildSoft.VRChat.Osc.Avatar;
using BuildSoft.VRChat.Osc.Test.Utility;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test.Avatar;

[TestOf(typeof(OscAvatarConfig))]
public class OscAvatarConfigPerformanceTests
{
    private static IEnumerable<OscAvatarConfigJson> ConfigJsonsForPerformanceTest
    {
        get
        {
            List<OscAvatarParameterJson> parameters = [];

            for (var j = 0; j < 10000; j++)
            {
                parameters.Clear();

                for (var k = 0; k < 100; k++)
                {
                    parameters.Add(new OscAvatarParameterJson($"param{k}", OscType.Float));
                }
                yield return new OscAvatarConfigJson($"avtr_{j}", $"name{j}", [.. parameters], j);
            }
        }
    }


    [SetUp]
    public void Setup()
    {

    }

    [TearDown]
    public void TearDown()
    {

    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        OscUtility.Initialize();

        TestHelper.StashOscDirectory();

        var directory = Path.Combine(OscUtility.VRChatOscPath, "Avatars");
        foreach (var config in ConfigJsonsForPerformanceTest)
        {
            TestHelper.CreateConfigFileForTest(config, directory);
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TestHelper.RestoreOscDirectory();
    }


    [Timeout(8000)]
    [Test]
    public void CreateAll_PerformanceTest()
    {
        _ = OscAvatarConfig.CreateAll();
    }
}
