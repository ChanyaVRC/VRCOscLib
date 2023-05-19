using System;
using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Test;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Avatar.Test;

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
        TestUtility.StashOscDirectory();

        TestUtility.CreateConfigFileForTest(AvatarId, "Test Avatar", TestUtility.GetAvatarConfigDirectory());
        _avatar = OscAvatarConfig.Create(AvatarId)!;


        OscParameter.Parameters.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        TestUtility.RestoreOscDirectory();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _server = new OscServer(OscUtility.SendPort);
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
        Assert.AreEqual(PhysBoneParam, physBone.ParamName);
        Assert.IsFalse(physBone.IsGrabbed);
        Assert.Zero(physBone.Angle);
        Assert.Zero(physBone.Stretch);

        var exception = Assert.Throws<ArgumentException>(() => new OscPhysBone(_avatar, PhysBoneParam + "_"));
        Assert.AreEqual("avatar", exception?.ParamName);
    }

    [Test]
    public void TestCtor2()
    {
        var physBone = new OscPhysBone(_avatar.Parameters, PhysBoneParam);
        Assert.AreEqual(PhysBoneParam, physBone.ParamName);
        Assert.IsFalse(physBone.IsGrabbed);
        Assert.Zero(physBone.Angle);
        Assert.Zero(physBone.Stretch);

        var exception = Assert.Throws<ArgumentException>(() => new OscPhysBone(_avatar.Parameters, PhysBoneParam + "_"));
        Assert.AreEqual("parameters", exception?.ParamName);
    }

    [Test]
    public void TestIsGrabbed()
    {
        var physBone = new OscPhysBone(_avatar, PhysBoneParam);
        Assert.IsFalse(physBone.IsGrabbed);

        const string IsGrabbedParamName = PhysBoneParam + "_IsGrabbed";
        var parameters = _avatar.Parameters;
        var isGrabbedParam = parameters.Get(IsGrabbedParamName);
        int passedCount = 0;

        void Handler(OscAvatarParameter sender, ValueChangedEventArgs e)
        {
            Assert.AreEqual(isGrabbedParam, sender);
            passedCount++;
        }

        physBone.ParameterChanged += Handler;

        parameters[IsGrabbedParamName] = true;
        Assert.IsTrue(physBone.IsGrabbed);
        Assert.AreEqual(1, passedCount);

        parameters[IsGrabbedParamName] = false;
        Assert.IsFalse(physBone.IsGrabbed);
        Assert.AreEqual(2, passedCount);

        parameters[IsGrabbedParamName] = false;
        Assert.IsFalse(physBone.IsGrabbed);
        Assert.AreEqual(2, passedCount);

        physBone.ParameterChanged -= Handler;
    }

    [Test]
    public void TestIsPosed()
    {
        var physBone = new OscPhysBone(_avatar, PhysBoneParam);
        Assert.IsFalse(physBone.IsPosed);

        const string IsPosedParamName = PhysBoneParam + "_IsPosed";
        var parameters = _avatar.Parameters;
        var IsPosedParam = parameters.Get(IsPosedParamName);
        int passedCount = 0;

        void Handler(OscAvatarParameter sender, ValueChangedEventArgs e)
        {
            Assert.AreEqual(IsPosedParam, sender);
            passedCount++;
        }

        physBone.ParameterChanged += Handler;

        parameters[IsPosedParamName] = true;
        Assert.IsTrue(physBone.IsPosed);
        Assert.AreEqual(1, passedCount);

        parameters[IsPosedParamName] = false;
        Assert.IsFalse(physBone.IsPosed);
        Assert.AreEqual(2, passedCount);

        parameters[IsPosedParamName] = false;
        Assert.IsFalse(physBone.IsPosed);
        Assert.AreEqual(2, passedCount);

        physBone.ParameterChanged -= Handler;
    }

    [Test]
    public void TestAngle()
    {
        var physBone = new OscPhysBone(_avatar, PhysBoneParam);
        Assert.AreEqual(0f, physBone.Angle);

        const string AngleParamName = PhysBoneParam + "_Angle";
        var parameters = _avatar.Parameters;
        var angleParam = parameters.Get(AngleParamName);
        int passedCount = 0;

        void Handler(OscAvatarParameter sender, ValueChangedEventArgs e)
        {
            Assert.AreEqual(angleParam, sender);
            passedCount++;
        }

        physBone.ParameterChanged += Handler;

        parameters[AngleParamName] = 0.1f;
        Assert.AreEqual(0.1f, physBone.Angle);
        Assert.AreEqual(1, passedCount);

        parameters[AngleParamName] = -1.2345f;
        Assert.AreEqual(-1.2345f, physBone.Angle);
        Assert.AreEqual(2, passedCount);

        parameters[AngleParamName] = -1.2345f;
        Assert.AreEqual(-1.2345f, physBone.Angle);
        Assert.AreEqual(2, passedCount);

        physBone.ParameterChanged -= Handler;
    }

    [Test]
    public void TestStretch()
    {
        var physBone = new OscPhysBone(_avatar, PhysBoneParam);
        Assert.AreEqual(0f, physBone.Stretch);

        const string StretchParamName = PhysBoneParam + "_Stretch";
        var parameters = _avatar.Parameters;
        var stretchParam = parameters.Get(StretchParamName);
        int passedCount = 0;

        void Handler(OscAvatarParameter sender, ValueChangedEventArgs e)
        {
            Assert.AreEqual(stretchParam, sender);
            passedCount++;
        }

        physBone.ParameterChanged += Handler;

        parameters[StretchParamName] = 0.1f;
        Assert.AreEqual(0.1f, physBone.Stretch);
        Assert.AreEqual(1, passedCount);

        parameters[StretchParamName] = -1.2345f;
        Assert.AreEqual(-1.2345f, physBone.Stretch);
        Assert.AreEqual(2, passedCount);

        parameters[StretchParamName] = -1.2345f;
        Assert.AreEqual(-1.2345f, physBone.Stretch);
        Assert.AreEqual(2, passedCount);

        physBone.ParameterChanged -= Handler;
    }

    [Test]
    public void TestSquish()
    {
        var physBone = new OscPhysBone(_avatar, PhysBoneParam);
        Assert.AreEqual(0f, physBone.Squish);

        const string SquishParamName = PhysBoneParam + "_Squish";
        var parameters = _avatar.Parameters;
        var squishParam = parameters.Get(SquishParamName);
        int passedCount = 0;

        void Handler(OscAvatarParameter sender, ValueChangedEventArgs e)
        {
            Assert.AreEqual(squishParam, sender);
            passedCount++;
        }

        physBone.ParameterChanged += Handler;

        parameters[SquishParamName] = 0.1f;
        Assert.AreEqual(0.1f, physBone.Squish);
        Assert.AreEqual(1, passedCount);

        parameters[SquishParamName] = -1.2345f;
        Assert.AreEqual(-1.2345f, physBone.Squish);
        Assert.AreEqual(2, passedCount);

        parameters[SquishParamName] = -1.2345f;
        Assert.AreEqual(-1.2345f, physBone.Squish);
        Assert.AreEqual(2, passedCount);

        physBone.ParameterChanged -= Handler;
    }
}
