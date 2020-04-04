using Arma3BE.Client.Modules.SyncModule.SyncCore;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3BE.Client.Modules.SyncModule.Tests
{
    [TestFixture]
    public class BackendClientTests
    {
        [SetUp]
        public void CreateBackend()
        {
            var moqClient = new Mock<IHttpGenericClient>();
            moqClient.Setup(c => c.PostAsync<AuthRequest, AuthResponse>(It.IsAny<string>(), It.IsAny<AuthRequest>()))
                .Returns(() => Task.FromResult(new AuthResponse()));

            var players = Enumerable.Range(0, 10).Select(x => new PlayerSyncDto()).ToArray();

            moqClient.Setup(c => c.GetAsync<PlayerSyncResponse>(It.IsAny<string>()))
                .Returns(() => Task.FromResult(new PlayerSyncResponse() { Players = players, Count = players.Length }));

            var backend = new BackendClient(new SyncCredentials(string.Empty, string.Empty, string.Empty),
                    moqClient.Object);

            _classUnderTest = backend;
            _expectedPlayers = players;
        }

        private BackendClient _classUnderTest;
        private PlayerSyncDto[] _expectedPlayers;

        [Test]
        public async Task BackendClientTests_GetPlayers_Positive_Test()
        {
            var response = await _classUnderTest.GetPlayers(0, 10);
            CollectionAssert.AreEqual(_expectedPlayers, response.Players);
        }
    }
}
