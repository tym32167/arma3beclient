using Arma3BE.Server.Messaging.Recognizers;
using NUnit.Framework;

namespace Arma3BEClient.Tests.ServerMessages.Recognizers
{
    [TestFixture]
    public class PlayerListLineRecognizerTest
    {
        [Test]
        [TestCase("0   5.141.88.80:59558     31   someGUIDnumbers(OK) hobot")]
        [TestCase("5   62.122.208.170:2304   16   someGUIDnumbers(OK) Ranger  (Lobby)")]
        [TestCase("10  188.244.37.117:2304   15   someGUIDnumbers(OK) ����177")]
        [TestCase("14  178.34.203.78:18944   47   someGUIDnumbers(OK) Aleksandres (Lobby)")]
        [TestCase("31  46.231.212.34:2304    0    someGUIDnumbers(OK) [GH] Listopad")]
        [TestCase("31  46.231.212.34:2304    0    someGUIDnumbers(OK) [GH] Listopad (Lobby)")]
        public void Line_Correct_Test(string line)
        {
            var recognizer = new PlayerListRecognizer();
            Assert.IsTrue(recognizer.CanRecognizeLine(line));
        }

        [Test]
        [TestCase("0s   5.141.88.80:59558     31   someGUIDnumbers(OK) hobot")]
        [TestCase("5   62..12.2.208.170:2304   16   someGUIDnumbers(OK) Ranger  (Lobby)")]
        [TestCase("10  188.244.37.117:2304   1d5   someGUIDnumbers(OK) ����177")]
        public void Line_Not_Correct_Test(string line)
        {
            var recognizer = new PlayerListRecognizer();
            Assert.IsFalse(recognizer.CanRecognizeLine(line));
        }
    }
}