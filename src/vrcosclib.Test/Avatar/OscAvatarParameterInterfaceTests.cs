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

        Assert.AreEqual(Address, parameterInterface1.Address);
        Assert.AreEqual(new BlobString(Address), parameterInterface1.AddressBlob);

        Assert.AreEqual("Bool", parameterInterface1.Type);
    }
}
