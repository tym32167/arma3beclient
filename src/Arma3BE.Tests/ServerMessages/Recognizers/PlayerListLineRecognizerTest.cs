using Arma3BE.Server.Recognizers;
using NUnit.Framework;

namespace Arma3BEClient.Tests.ServerMessages.Recognizers
{
    [TestFixture]
    public class PlayerListLineRecognizerTest
    {
        [Test]
        [TestCase("0   5.141.88.80:59558     31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) hobot")]
        [TestCase("5   62.122.208.170:2304   16   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Ranger  (Lobby)")]
        [TestCase("10  188.244.37.117:2304   15   d0ee5caf1b8b6282349e79fb998c2ee2(OK) ����177")]
        [TestCase("14  178.34.203.78:18944   47   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Aleksandres (Lobby)")]
        [TestCase("31  46.231.212.34:2304    0    d0ee5caf1b8b6282349e79fb998c2ee2(OK) [GH] Listopad")]
        [TestCase("31  46.231.212.34:2304    0    d0ee5caf1b8b6282349e79fb998c2ee2(OK) [GH] Listopad (Lobby)")]
        public void Line_Correct_Test(string line)
        {
            var recognizer = new PlayerListRecognizer();
            Assert.IsTrue(recognizer.CanRecognizeLine(line));
        }

        [Test]
        [TestCase("0s   5.141.88.80:59558     31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) hobot")]
        [TestCase("5   62..12.2.208.170:2304   16   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Ranger  (Lobby)")]
        [TestCase("10  188.244.37.117:2304   1d5   d0ee5caf1b8b6282349e79fb998c2ee2(OK) ����177")]
        public void Line_Not_Correct_Test(string line)
        {
            var recognizer = new PlayerListRecognizer();
            Assert.IsFalse(recognizer.CanRecognizeLine(line));
        }
    }
}