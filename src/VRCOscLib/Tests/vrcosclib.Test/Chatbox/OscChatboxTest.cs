using System.Text;
using BuildSoft.OscCore;
using BuildSoft.VRChat.Osc.Test;
using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Chatbox.Test;

[TestOf(typeof(OscChatbox))]
public class OscChatboxTest
{
    private OscServer _server = null!;

    [SetUp]
    public void Setup()
    {
        OscParameter.Parameters.Clear();
    }

    [TearDown]
    public void TearDown()
    {

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
        byte[] receivedMessage = new byte[2024];


        OscChatbox.SendMessage(message, direct, messageComplete);
        await TestUtility.LoopWhile(() => value == null, TestUtility.LatencyTimeout);
        int length = value.ReadStringElementBytes(0, receivedMessage);
        bool receivedDirect = value.ReadBooleanElement(1);
        bool receivedMessageComplete = value.ReadBooleanElement(2);


        Assert.AreEqual(message, Encoding.UTF8.GetString(receivedMessage, 0, length));
        Assert.AreEqual(direct, receivedDirect);
        Assert.AreEqual(messageComplete, receivedMessageComplete);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task SetIsTypingTest(bool isTyping)
    {
        OscMessageValues value = null!;
        void valueReadMethod(OscMessageValues v) => value = v;
        _server.TryAddMethod(OscChatbox.TypingAddress, valueReadMethod);


        OscChatbox.SetIsTyping(isTyping);
        await TestUtility.LoopWhile(() => value == null, TestUtility.LatencyTimeout);
        bool receivedIsTyping = value.ReadBooleanElement(0);


        Assert.AreEqual(isTyping, receivedIsTyping);
    }

}
