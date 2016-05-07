using Arma3BE.Server.Recognizers;
using NUnit.Framework;

namespace Arma3BEClient.Tests.ServerMessages.Recognizers
{
    [TestFixture]
    public class AdminListLineRecognizerTest
    {
        [Test]
        [TestCase("0 94.181.44.100:50960")]
        [TestCase("1 213.21.12.135:54678")]
        public void Line_Correct_Test(string line)
        {
            var recognizer = new AdminListRecognizer();
            Assert.IsTrue(recognizer.CanRecognizeLine(line));
        }

        [Test]
        [TestCase("0 94.181..44.100:50960")]
        [TestCase("1s 213.21.12.135:54678")]
        public void Line_Not_Correct_Test(string line)
        {
            var recognizer = new AdminListRecognizer();
            Assert.IsFalse(recognizer.CanRecognizeLine(line));
        }
    }
}