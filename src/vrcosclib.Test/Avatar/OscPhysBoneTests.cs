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
        Directory.CreateDirectory(OscUtility.VRChatOscPath);
        Directory.Move(OscUtility.VRChatOscPath, OscUtility.VRChatOscPath + "_Renamed");
        Directory.CreateDirectory(OscUtility.VRChatOscPath);

        TestUtility.CreateConfigFileForTest(AvatarId, "Test Avatar", TestUtility.GetAvatarConfigDirectory());
        _avatar = OscAvatarConfig.Create(AvatarId)!;

        _server = new OscServer(OscUtility.SendPort);

        OscParameter.Parameters.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        Directory.Delete(OscUtility.VRChatOscPath, true);
        Directory.Move(OscUtility.VRChatOscPath + "_Renamed", OscUtility.VRChatOscPath);
        _server.Dispose();
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
}
