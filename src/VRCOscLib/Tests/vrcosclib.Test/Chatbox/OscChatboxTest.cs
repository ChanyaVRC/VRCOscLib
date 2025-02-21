using System.Text;
using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Chatbox;
using BuildSoft.VRChat.Osc.Test.Utility;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test.Chatbox;

[TestOf(typeof(OscChatbox))]
public class OscChatboxTest
{
    private OscServer _server = null!;

    [SetUp]
    public void Setup()
    {
        OscParameter.Items.Clear();
    }

    [TearDown]
    public void TearDown()
    {

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


    [TestCase("", true, true)]
    [TestCase("ASCII", true, false)]
    [TestCase("ASCII", false, true)]
    [TestCase("ＵＴＦ-８", true, false)]
    [TestCase("😂😭😪😥😰😅😓😩😫😨😱", true, true)]
    public async Task SendMessageTest(string message, bool direct, bool messageComplete)
    {
        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        _server.TryAddMethod(OscChatbox.InputAddress, valueReadMethod);
        var receivedMessage = new byte[2024];


        OscChatbox.SendMessage(message, direct, messageComplete);
        await TestHelper.WaitWhile(() => value == null, TestHelper.LatencyTimeout);
        var length = value.ReadStringElementBytes(0, receivedMessage);
        var receivedDirect = value.ReadBooleanElement(1);
        var receivedMessageComplete = value.ReadBooleanElement(2);


        Assert.That(message, Is.EqualTo(Encoding.UTF8.GetString(receivedMessage, 0, length)));
        Assert.That(direct, Is.EqualTo(receivedDirect));
        Assert.That(messageComplete, Is.EqualTo(receivedMessageComplete));
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task SetIsTypingTest(bool isTyping)
    {
        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        _server.TryAddMethod(OscChatbox.TypingAddress, valueReadMethod);


        OscChatbox.SetIsTyping(isTyping);
        await TestHelper.WaitWhile(() => value == null, TestHelper.LatencyTimeout);
        var receivedIsTyping = value.ReadBooleanElement(0);


        Assert.That(receivedIsTyping, Is.EqualTo(isTyping));
    }

}
