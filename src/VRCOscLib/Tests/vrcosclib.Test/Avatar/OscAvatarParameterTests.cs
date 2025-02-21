using BuildSoft.VRChat.Osc.Avatar;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test.Avatar;

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

        Assert.That(parameter1.Name, Is.EqualTo("param1"));
        Assert.That(parameter1.Input, Is.SameAs(input));
        Assert.That(parameter1.Output, Is.SameAs(output));
        Assert.That(parameter1.ReadableAddress, Is.EqualTo(output.Address));

        Assert.That(parameter2.Name, Is.EqualTo("param2"));
        Assert.That(parameter2.Input, Is.Null);
        Assert.That(parameter2.Output, Is.SameAs(output));
        Assert.That(parameter2.ReadableAddress, Is.EqualTo(output.Address));

        Assert.That(parameter3.Name, Is.EqualTo("param3"));
        Assert.That(parameter3.Input, Is.SameAs(input));
        Assert.That(parameter3.Output, Is.Null);
        Assert.That(parameter3.ReadableAddress, Is.EqualTo(input.Address));
    }

    [Test]
    public void Ctor_BothNullTest()
    {
        Assert.Throws<ArgumentException>(() => new OscAvatarParameter("param", null, null));
    }
}
