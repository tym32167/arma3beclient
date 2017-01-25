using NUnit.Framework;

namespace Arma3BE.Client.Modules.CoreModule.Tests
{
    [TestFixture]
    public class StringTemplaterTests
    {
        [Test]
        public void TemplateSimpleTest()
        {
            var templater = new StringTemplater();
            var actual = templater.Template("some text");
            Assert.AreEqual("some text", actual);
        }


        [Test]
        public void TemplateParametersTest()
        {
            var templater = new StringTemplater();
            templater.AddParameter("Param", "value");
            var actual = templater.Template("some text {Param}");
            Assert.AreEqual("some text value", actual);
        }
    }
}