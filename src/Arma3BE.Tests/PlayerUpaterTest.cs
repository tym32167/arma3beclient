using Arma3BE.Server.Models;
using NUnit.Framework;

namespace Arma3BEClient.Tests
{
    [TestFixture]
    public class PlayerUpaterTest
    {
        [Test]
        public void Parse1()
        {
            var p =
                Player.Parse(
                    "17  188.162.228.192:63255 172  d074b6194fa864e50956a8294077f24a(OK) Cpt. JET");
            Assert.IsNotNull(p);
        }
    }
}