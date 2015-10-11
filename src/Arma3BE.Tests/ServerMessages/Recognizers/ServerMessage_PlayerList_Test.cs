using Arma3BE.Server.Messaging.Recognizers;
using Arma3BE.Server.Models;
using NUnit.Framework;

namespace Arma3BEClient.Tests.ServerMessages.Recognizers
{
    [TestFixture]
    public class PlayerListRecognizer_Test
    {
        [Test]
        public void Player_List_Test()
        {
            var str = @"
Players on server:
[#] [IP Address]:[Port] [Ping] [GUID] [Name]
--------------------------------------------------
0   213.27.75.36:2304     63   ff53d0b454e948f7c4bae714b50f4f80(OK) RatniK
1   194.135.155.161:21519 62   ae5c9a928135b11714e521d87ace0957(OK) Wifleem
2   188.242.156.212:2304  32   9748b3f0134f95839aca62d94a42e48f(OK) kovt
3   176.62.108.109:2304   47   21b52f31435522b611f54777743c96cb(OK) Ice
4   213.88.77.71:2304     16   c7bf58aeac99b8892dbc90818563baaa(OK) 12s
5   95.24.235.69:2304     0    83cec4148ec7cfd2cad4425896e5a0d1(OK) Frenzy (Lobby)
7   213.141.128.143:2304  0    621a8081419d4a66274e044251fdb6a2(OK) Горизонт
8   95.59.46.59:17540     62   91346f28052db9cceb321b284a957129(OK) FaZa
9   31.130.124.118:56750  31   5a5028727735510e69d857ce47133797(OK) ReLogic
11  185.13.112.89:18891   0    f856097eacf552ee03345c8ffb6a4819(OK) Veles(RU)
12  87.117.12.210:57364   63   d7be77705169db5f3ea45db0a373cd8e(OK) bezpointov
13  89.251.75.101:2304    31   7447e92ee0497af1f38b475be86171a7(OK) Rolevik
14  178.88.11.177:13094   93   ac93a589cc1b5b89396caa6f322fb2f3(OK) GhostWarrior
15  95.82.216.154:2304    31   59d000425e3aaa8010f9010f29acd3b1(OK) Artem_Mario
17  77.79.128.173:2304    31   fa3641ae52d2d3d5c9db86602d99a8ba(OK) tomson
(15 players in total)
]
";
            var serverMessage = new ServerMessage(0, str);
            var recognizer = new PlayerListRecognizer();
            Assert.IsTrue(recognizer.CanRecognize(serverMessage));
        }


        [Test]
        public void Player_List_NOT_CORECT_Test()
        {
            var str = @"
Players on server:
[#] [IP Address]:[Port] [Ping] [GUID] [Name]
--------------------------------------------------
0   213.27.75.36:2304     63   ff53d0b454e948f7c4bae714b50f4f80(OK) RatniK
1   194.135.155.161:21519 62   ae5c9a928135b11714e521d87ace0957(OK) Wifleem
2   188.242.156.212:2304  32   9748b3f0134f95839aca62d94a42e48f(OK) kovt
3   176.62.108.109:2304   47   21b52f31435522b611f54777743c96cb(OK) Ice
4   213.88.77.71:2304     16   c7bf58aeac99b8892dbc90818563baaa(OK) 12s
5   95.24.235.69:2304     0    83cec4148ec7cfd2cad4425896e5a0d1(OK) Frenzy (Lobby)
7   213.141.128.143:2304  0    621a8081419d4a66274e044251fdb6a2(OK) Горизонт
8   95.59.46.59:17540     62   91346f28052db9cceb321b284a957129(OK) FaZa
9   31.130.124.118:56750  31   5a5028727735510e69d857ce47133797(OK) ReLogic
11  185.13.112.89:18891   0    f856097eacf552ee03345c8ffb6a4819(OK) Veles(RU)
12  87.117.12.210:57364   63   d7be77705169db5f3ea45db0a373cd8e(OK) bezpointov
13  89.251.75.101:2304    31   7447e92ee0497af1f38b475be86171a7(OK) Rolevik
14  178.88.11.177:13094   93   ac93a589cc1b5b89396caa6f322fb2f3(OK) GhostWarrior
15  95.82.216.154:2304    31   59d000425e3aaa8010f9010f29acd3b1(OK) Artem_Mario
17  77.79.128.173:2304    31   fa3641ae52d2d3d5c9db86602d99a8ba(OK) tomson
]
";
            var serverMessage = new ServerMessage(0, str);
            var recognizer = new PlayerListRecognizer();
            Assert.IsFalse(recognizer.CanRecognize(serverMessage));
        }
    }
}