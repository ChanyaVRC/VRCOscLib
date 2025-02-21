using BuildSoft.VRChat.Osc.Test;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Avatar.Test;

[TestOf(typeof(OscAvatarConfig))]
public class OscAvatarConfigPerformanceTests
{
    private static IEnumerable<OscAvatarConfigJson> ConfigJsonsForPerformanceTest
    {
        get
        {
            List<OscAvatarParameterJson> parameters = [];

            for (int j = 0; j < 10000; j++)
            {
                parameters.Clear();

                for (int k = 0; k < 100; k++)
                {
                    parameters.Add(new OscAvatarParameterJson($"param{k}", OscType.Float));
                }
                yield return new OscAvatarConfigJson($"avtr_{j}", $"name{j}", [.. parameters]);
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

        TestUtility.StashOscDirectory();

        var directory = Path.Combine(OscUtility.VRChatOscPath, "Avatars");
        foreach (var config in ConfigJsonsForPerformanceTest)
        {
            TestUtility.CreateConfigFileForTest(config, directory);
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TestUtility.RestoreOscDirectory();
    }


    [Timeout(8000)]
    [Test]
    public void CreateAll_PerformanceTest()
    {
        _ = OscAvatarConfig.CreateAll();
    }
}
