using BuildSoft.VRChat.Osc.Avatar;
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
    public void Ctor_BothNullTest()
    {
        Assert.Throws<ArgumentException>(() => new OscAvatarParameter("param", null, null));
    }
}
