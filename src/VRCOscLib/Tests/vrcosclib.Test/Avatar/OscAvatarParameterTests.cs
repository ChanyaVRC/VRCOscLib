using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Avatar;
using BuildSoft.VRChat.Osc.Test;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Avatar.Test;

[TestOf(typeof(OscAvatarParameter))]
public class OscAvatarParameterTests
{
    [Test]
    public void CtorTest()
    {
        var input = new OscAvatarParameterInterface("/address/to/input", OscType.Bool);
        var output = new OscAvatarParameterInterface("/address/to/output", OscType.Float);

        var parameter1 = new OscAvatarParameter("param1", input: input, output: output);
        var parameter2 = new OscAvatarParameter("param2", output: output);
        var parameter3 = new OscAvatarParameter("param3", input: input);

        Assert.AreEqual("param1", parameter1.Name);
        Assert.AreSame(input, parameter1.Input);
        Assert.AreSame(output, parameter1.Output);
        Assert.AreEqual(output.Address, parameter1.ReadableAddress);

        Assert.AreEqual("param2", parameter2.Name);
        Assert.IsNull(parameter2.Input);
        Assert.AreSame(output, parameter2.Output);
        Assert.AreEqual(output.Address, parameter2.ReadableAddress);

        Assert.AreEqual("param3", parameter3.Name);
        Assert.AreSame(input, parameter3.Input);
        Assert.IsNull(parameter3.Output);
        Assert.AreEqual(input.Address, parameter3.ReadableAddress);
    }

    [Test]
    public void CreateTest()
    {
        OscAvatarParameter parameter = OscAvatarParameter.Create("param", OscType.Int);

        Assert.That(parameter.Name, Is.EqualTo("param"));

        Assert.That(parameter.Input, Is.Not.Null);
        Assert.That(parameter.Input!.OscType, Is.EqualTo(OscType.Int));
        Assert.That(parameter.Input!.Address, Is.EqualTo("/avatar/parameters/param"));

        Assert.That(parameter.Output, Is.Not.Null);
        Assert.That(parameter.Output!.OscType, Is.EqualTo(OscType.Int));
        Assert.That(parameter.Output!.Address, Is.EqualTo("/avatar/parameters/param"));
    }

    [Test]
    public void Ctor_BothNullTest()
    {
        Assert.Throws<ArgumentException>(() => new OscAvatarParameter("param", null, null));
    }

    [Test]
    public async Task ParameterChangedTest()
    {
        int newValue = -1;
        string paramName = "";
        bool isCalled;
        var param1 = OscAvatarParameter.Create("param1", OscType.Int);
        var param2 = OscAvatarParameter.Create("param2", OscType.Int);

        param1.ValueChanged += Handler;

        newValue = 100;
        paramName = param1.Name;
        isCalled = false;
        using (var client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort))
        {
            client.Send(OscConst.AvatarParameterAddressSpace + paramName, newValue);
            await TestUtility.LoopWhile(() => !isCalled, TestUtility.LatencyTimeout);
        }
        Assert.That(isCalled);

        param1.ValueChanged -= Handler;

        param2.ValueChanged += Handler;
        param1.ValueChanged -= Handler;

        newValue = 200;
        paramName = param2.Name;
        isCalled = false;
        using (var client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort))
        {
            client.Send(OscConst.AvatarParameterAddressSpace + paramName, newValue);
            await TestUtility.LoopWhile(() => !isCalled, TestUtility.LatencyTimeout);
        }
        Assert.That(isCalled);

        param2.ValueChanged -= Handler;

        isCalled = false;
        using (var client = new OscClient("127.0.0.1", OscConnectionSettings.ReceivePort))
        {
            client.Send(OscConst.AvatarParameterAddressSpace + paramName, newValue);
            Assert.ThrowsAsync<TimeoutException>(async () => await TestUtility.LoopWhile(() => !isCalled, TestUtility.LatencyTimeout));
        }
        Assert.That(!isCalled);


        void Handler(OscAvatarParameter param, ValueChangedEventArgs e)
        {
            Assert.That(param.Name, Is.EqualTo(paramName));
            Assert.That(e.OldValue, Is.Null);
            Assert.That(e.NewValue, Is.EqualTo(newValue));
            isCalled = true;
        }
    }
}
