using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Avatar;
using BuildSoft.VRChat.Osc.Test.Utility;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test.Avatar;

[TestOf(typeof(OscPhysBone))]
public class OscPhysBoneTests
{
    private OscAvatarConfig _avatar = null!;
    private OscServer _server = null!;
    private const string AvatarId = "avtr_id_for_test";
    private const string PhysBoneParam = "PhysBoneParam";

    [SetUp]
    public void Setup()
    {
        TestHelper.StashOscDirectory();

        TestHelper.CreateConfigFileForTest(AvatarId, "Test Avatar", TestHelper.GetAvatarConfigDirectory(), 123456);
        _avatar = OscAvatarConfig.Create(AvatarId)!;


        OscParameter.Items.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        TestHelper.RestoreOscDirectory();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _server = new OscServer(OscConnectionSettings.SendPort);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _server.Dispose();
    }

    [Test]
    public void TestCtor1()
    {
        var physBone = new OscPhysBone(_avatar, PhysBoneParam);
        Assert.That(physBone.ParamName, Is.EqualTo(PhysBoneParam));
        Assert.That(physBone.IsGrabbed, Is.False);
        Assert.That(physBone.Angle, Is.Zero);
        Assert.That(physBone.Stretch, Is.Zero);

        var exception = Assert.Throws<ArgumentException>(() => new OscPhysBone(_avatar, PhysBoneParam + "_"));
        Assert.That(exception?.ParamName, Is.EqualTo("avatar"));
    }

    [Test]
    public void TestCtor2()
    {
        var physBone = new OscPhysBone(_avatar.Parameters, PhysBoneParam);
        Assert.That(physBone.ParamName, Is.EqualTo(PhysBoneParam));
        Assert.That(physBone.IsGrabbed, Is.False);
        Assert.That(physBone.Angle, Is.Zero);
        Assert.That(physBone.Stretch, Is.Zero);

        var exception = Assert.Throws<ArgumentException>(() => new OscPhysBone(_avatar.Parameters, PhysBoneParam + "_"));
        Assert.That(exception?.ParamName, Is.EqualTo("parameters"));
    }

    [Test]
    public void TestIsGrabbed()
    {
        var physBone = new OscPhysBone(_avatar, PhysBoneParam);
        Assert.That(physBone.IsGrabbed, Is.False);

        const string IsGrabbedParamName = PhysBoneParam + "_IsGrabbed";
        var parameters = _avatar.Parameters;
        var isGrabbedParam = parameters.Get(IsGrabbedParamName);
        var passedCount = 0;

        void Handler(OscAvatarParameter sender, ValueChangedEventArgs e)
        {
            Assert.That(sender, Is.EqualTo(isGrabbedParam));
            passedCount++;
        }

        physBone.ParameterChanged += Handler;

        parameters[IsGrabbedParamName] = true;
        Assert.That(physBone.IsGrabbed, Is.True);
        Assert.That(passedCount, Is.EqualTo(1));

        parameters[IsGrabbedParamName] = false;
        Assert.That(physBone.IsGrabbed, Is.False);
        Assert.That(passedCount, Is.EqualTo(2));

        parameters[IsGrabbedParamName] = false;
        Assert.That(physBone.IsGrabbed, Is.False);
        Assert.That(passedCount, Is.EqualTo(2));

        physBone.ParameterChanged -= Handler;
    }

    [Test]
    public void TestIsPosed()
    {
        var physBone = new OscPhysBone(_avatar, PhysBoneParam);
        Assert.That(physBone.IsPosed, Is.False);

        const string IsPosedParamName = PhysBoneParam + "_IsPosed";
        var parameters = _avatar.Parameters;
        var IsPosedParam = parameters.Get(IsPosedParamName);
        var passedCount = 0;

        void Handler(OscAvatarParameter sender, ValueChangedEventArgs e)
        {
            Assert.That(sender, Is.EqualTo(IsPosedParam));
            passedCount++;
        }

        physBone.ParameterChanged += Handler;

        parameters[IsPosedParamName] = true;
        Assert.That(physBone.IsPosed, Is.True);
        Assert.That(passedCount, Is.EqualTo(1));

        parameters[IsPosedParamName] = false;
        Assert.That(physBone.IsPosed, Is.False);
        Assert.That(passedCount, Is.EqualTo(2));

        parameters[IsPosedParamName] = false;
        Assert.That(physBone.IsPosed, Is.False);
        Assert.That(passedCount, Is.EqualTo(2));

        physBone.ParameterChanged -= Handler;
    }

    [Test]
    public void TestAngle()
    {
        var physBone = new OscPhysBone(_avatar, PhysBoneParam);
        Assert.That(physBone.Angle, Is.EqualTo(0f));

        const string AngleParamName = PhysBoneParam + "_Angle";
        var parameters = _avatar.Parameters;
        var angleParam = parameters.Get(AngleParamName);
        var passedCount = 0;

        void Handler(OscAvatarParameter sender, ValueChangedEventArgs e)
        {
            Assert.That(sender, Is.EqualTo(angleParam));
            passedCount++;
        }

        physBone.ParameterChanged += Handler;

        parameters[AngleParamName] = 0.1f;
        Assert.That(physBone.Angle, Is.EqualTo(0.1f));
        Assert.That(passedCount, Is.EqualTo(1));

        parameters[AngleParamName] = -1.2345f;
        Assert.That(physBone.Angle, Is.EqualTo(-1.2345f));
        Assert.That(passedCount, Is.EqualTo(2));

        parameters[AngleParamName] = -1.2345f;
        Assert.That(physBone.Angle, Is.EqualTo(-1.2345f));
        Assert.That(passedCount, Is.EqualTo(2));

        physBone.ParameterChanged -= Handler;
    }

    [Test]
    public void TestStretch()
    {
        var physBone = new OscPhysBone(_avatar, PhysBoneParam);
        Assert.That(physBone.Stretch, Is.EqualTo(0f));

        const string StretchParamName = PhysBoneParam + "_Stretch";
        var parameters = _avatar.Parameters;
        var stretchParam = parameters.Get(StretchParamName);
        var passedCount = 0;

        void Handler(OscAvatarParameter sender, ValueChangedEventArgs e)
        {
            Assert.That(sender, Is.EqualTo(stretchParam));
            passedCount++;
        }

        physBone.ParameterChanged += Handler;

        parameters[StretchParamName] = 0.1f;
        Assert.That(physBone.Stretch, Is.EqualTo(0.1f));
        Assert.That(passedCount, Is.EqualTo(1));

        parameters[StretchParamName] = -1.2345f;
        Assert.That(physBone.Stretch, Is.EqualTo(-1.2345f));
        Assert.That(passedCount, Is.EqualTo(2));

        parameters[StretchParamName] = -1.2345f;
        Assert.That(physBone.Stretch, Is.EqualTo(-1.2345f));
        Assert.That(passedCount, Is.EqualTo(2));

        physBone.ParameterChanged -= Handler;
    }

    [Test]
    public void TestSquish()
    {
        var physBone = new OscPhysBone(_avatar, PhysBoneParam);
        Assert.That(physBone.Squish, Is.EqualTo(0f));

        const string SquishParamName = PhysBoneParam + "_Squish";
        var parameters = _avatar.Parameters;
        var squishParam = parameters.Get(SquishParamName);
        var passedCount = 0;

        void Handler(OscAvatarParameter sender, ValueChangedEventArgs e)
        {
            Assert.That(sender, Is.EqualTo(squishParam));
            passedCount++;
        }

        physBone.ParameterChanged += Handler;

        parameters[SquishParamName] = 0.1f;
        Assert.That(physBone.Squish, Is.EqualTo(0.1f));
        Assert.That(passedCount, Is.EqualTo(1));

        parameters[SquishParamName] = -1.2345f;
        Assert.That(physBone.Squish, Is.EqualTo(-1.2345f));
        Assert.That(passedCount, Is.EqualTo(2));

        parameters[SquishParamName] = -1.2345f;
        Assert.That(physBone.Squish, Is.EqualTo(-1.2345f));
        Assert.That(passedCount, Is.EqualTo(2));

        physBone.ParameterChanged -= Handler;
    }
}
