using Arma3BE.Server.Messaging.Recognizers;
using Arma3BE.Server.Models;
using NUnit.Framework;

namespace Arma3BEClient.Tests.ServerMessages.Recognizers
{
    [TestFixture]
    public class AdminListRecognizerTest
    {
        [Test]
        [TestCase(@"Connected RCon admins:
[#] [IP Address]:[Port]
-----------------------------
0 94.181.44.100:50960
1 213.21.12.135:54678")]
        [TestCase(@"Connected RCon admins:
[#] [IP Address]:[Port]
-----------------------------
0 94.181.44.100:50960
1 213.21.12.135:54678
2 191.234.50.135:1885
")]
        public void Player_List_Test(string message)
        {
            var serverMessage = new ServerMessage(0, message);
            var recognizer = new AdminListRecognizer();
            Assert.IsTrue(recognizer.CanRecognize(serverMessage));
        }


        [Test]
        [TestCase(@"Connected RCon admins:
[#] [IP Address]:[Port]
-----------------------------
0 94.181.44.1s00:50960
1 213.21.12.135:54678")]
        [TestCase(@"Connected RCon admins:
[#] [IP Address]:[Port]
-----------------------------
0 94.181.44.10
")]
        public void Player_List_NOT_CORECT_Test(string message)
        {
            var serverMessage = new ServerMessage(0, message);
            var recognizer = new AdminListRecognizer();
            Assert.IsFalse(recognizer.CanRecognize(serverMessage));
        }
    }
}