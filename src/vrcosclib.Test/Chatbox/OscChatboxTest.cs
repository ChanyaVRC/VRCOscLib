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
        _server = new OscServer(OscUtility.SendPort);

        OscParameter.Parameters.Clear();
    }

    [TearDown]
    public void TearDown()
    {
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
        byte[] recievedMessage = new byte[2024];


        OscChatbox.SendMessage(message, direct, messageComplete);
        await TestUtility.LoopWhile(() => value == null, TestUtility.LatencyTimeout);
        int length = value.ReadStringElementBytes(0, recievedMessage);
        bool recievedDirect = value.ReadBooleanElement(1);
        bool recievedMessageComplete = value.ReadBooleanElement(2);


        Assert.AreEqual(message, Encoding.UTF8.GetString(recievedMessage, 0, length));
        Assert.AreEqual(direct, recievedDirect);
        Assert.AreEqual(messageComplete, recievedMessageComplete);
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
        bool recievedIsTyping = value.ReadBooleanElement(0);


        Assert.AreEqual(isTyping, recievedIsTyping);
    }

}
