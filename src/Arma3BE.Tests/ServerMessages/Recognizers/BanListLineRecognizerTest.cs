using Arma3BE.Server.Messaging.Recognizers;
using NUnit.Framework;

namespace Arma3BEClient.Tests.ServerMessages.Recognizers
{
    [TestFixture]
    public class BanListLineRecognizerTest
    {
        [Test]
        [TestCase("0  d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill.Ban perm")]
        [TestCase("4  d0ee5caf1b8b6282349e79fb998c2ee2 perm Cheater.Ban perm!")]
        [TestCase("5  d0ee5caf1b8b6282349e79fb998c2ee2 perm Cheater(Ban PERM)")]
        [TestCase("219 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][15.09.14 12:25:59] Teamkill")]
        [TestCase("508 d0ee5caf1b8b6282349e79fb998c2ee2 8971 [Rembowest14][18.07.15 13:38:14] Sabotage")]
        public void Line_Correct_Test(string line)
        {
            var recognizer = new BanListRecognizer();
            Assert.IsTrue(recognizer.CanRecognizeLine(line));
        }

        [Test]
        [TestCase("20w  d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill.Ban perm")]
        [TestCase("4  GU")]
        [TestCase("5d  d0ee5caf1b8b6282349e79fb998c2ee2 perm Cheater(Ban PERM)")]
        public void Line_Not_Correct_Test(string line)
        {
            var recognizer = new BanListRecognizer();
            Assert.IsFalse(recognizer.CanRecognizeLine(line));
        }
    }
}