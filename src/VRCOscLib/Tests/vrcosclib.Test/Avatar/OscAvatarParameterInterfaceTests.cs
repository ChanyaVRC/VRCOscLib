using BlobHandles;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Avatar.Test;

[TestOf(typeof(OscAvatarParameterInterface))]
public class OscAvatarParameterInterfaceTests
{
    [Test]
    public void CtorTest()
    {
        const string Address = "/address/to/parameter";
        var parameterInterface1 = new OscAvatarParameterInterface(Address, OscType.Bool);

        Assert.That(parameterInterface1.Address, Is.EqualTo(Address));
        Assert.That(parameterInterface1.AddressBlob, Is.EqualTo(new BlobString(Address)));

        Assert.That(parameterInterface1.OscType, Is.EqualTo(OscType.Bool));
        Assert.That(parameterInterface1.Type, Is.EqualTo("Bool"));
    }
}
