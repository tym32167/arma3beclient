using Arma3BE.Server.Messaging.Recognizers;
using Arma3BE.Server.Models;
using NUnit.Framework;

namespace Arma3BEClient.Tests.ServerMessages.Recognizers
{
    [TestFixture]
    public class PlayerListRecognizerTest
    {
        [Test]
        [TestCase(@"Players on server:
[#] [IP Address]:[Port] [Ping] [GUID] [Name]
--------------------------------------------------
0   5.141.88.80:59558     31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) hobot
1   37.204.36.17:2304     15   d0ee5caf1b8b6282349e79fb998c2ee2(OK) ZeRoKoOl
2   94.75.36.1:21455      47   d0ee5caf1b8b6282349e79fb998c2ee2(OK) ~SCORPION~
3   5.139.190.80:2304     47   d0ee5caf1b8b6282349e79fb998c2ee2(OK) StasCherniy
4   188.186.73.4:2304     47   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Dread
5   62.122.208.170:2304   16   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Ranger  (Lobby)
6   37.193.69.47:2304     47   d0ee5caf1b8b6282349e79fb998c2ee2(OK) MORKLEB (Lobby)
7   5.44.174.54:2304      109  d0ee5caf1b8b6282349e79fb998c2ee2(OK) tuman-CS
9   46.180.53.73:52060    62   d0ee5caf1b8b6282349e79fb998c2ee2(OK) AL1R
10  188.244.37.117:2304   15   d0ee5caf1b8b6282349e79fb998c2ee2(OK) ����177
11  95.25.234.161:2304    78   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Jo_Boo
12  176.100.92.208:2304   110  d0ee5caf1b8b6282349e79fb998c2ee2(OK) Kvazar
13  31.163.200.93:2304    78   d0ee5caf1b8b6282349e79fb998c2ee2(OK) 34th
14  178.34.203.78:18944   47   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Aleksandres (Lobby)
15  46.150.163.240:2304   16   d0ee5caf1b8b6282349e79fb998c2ee2(OK) (F)TypucT
16  81.1.232.163:56395    78   d0ee5caf1b8b6282349e79fb998c2ee2(OK) [BB]Ghostis
17  217.197.240.227:2304  16   d0ee5caf1b8b6282349e79fb998c2ee2(OK) FlinT
18  176.36.211.138:2304   31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Maxik
19  85.113.135.224:21253  15   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Yarik
20  164.215.91.74:2304    125  d0ee5caf1b8b6282349e79fb998c2ee2(OK) Qpitman
21  193.169.36.194:33475  125  d0ee5caf1b8b6282349e79fb998c2ee2(OK) Urielf
22  77.222.103.104:2304   47   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Mr_Fail
23  77.37.145.162:2304    16   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Legioner
24  178.218.106.147:2304  141  d0ee5caf1b8b6282349e79fb998c2ee2(OK) DRY
25  91.236.224.109:2304   47   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Lincoln
27  94.232.234.25:2304    172  d0ee5caf1b8b6282349e79fb998c2ee2(OK) [RSA]KA5
28  176.125.78.22:2304    125  d0ee5caf1b8b6282349e79fb998c2ee2(OK) ALON
29  83.220.238.189:30140  110  d0ee5caf1b8b6282349e79fb998c2ee2(OK) 4poK
30  37.146.201.30:2304    63   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Berns_Mack
31  46.231.212.34:2304    0    d0ee5caf1b8b6282349e79fb998c2ee2(OK) [GH] Listopad
33  77.50.204.111:2304    16   d0ee5caf1b8b6282349e79fb998c2ee2(OK) npu3pak
34  83.246.179.250:2304   78   d0ee5caf1b8b6282349e79fb998c2ee2(OK) CrazeXoma
(32 players in total)")]
        [TestCase(@"
Players on server:
[#] [IP Address]:[Port] [Ping] [GUID] [Name]
--------------------------------------------------
0   213.27.75.36:2304     63   d0ee5caf1b8b6282349e79fb998c2ee2(OK) RatniK
1   194.135.155.161:21519 62   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Wifleem
2   188.242.156.212:2304  32   d0ee5caf1b8b6282349e79fb998c2ee2(OK) kovt
3   176.62.108.109:2304   47   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Ice
4   213.88.77.71:2304     16   d0ee5caf1b8b6282349e79fb998c2ee2(OK) 12s
5   95.24.235.69:2304     0    d0ee5caf1b8b6282349e79fb998c2ee2(OK) Frenzy (Lobby)
7   213.141.128.143:2304  0    d0ee5caf1b8b6282349e79fb998c2ee2(OK) Горизонт
8   95.59.46.59:17540     62   d0ee5caf1b8b6282349e79fb998c2ee2(OK) FaZa
9   31.130.124.118:56750  31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) ReLogic
11  185.13.112.89:18891   0    d0ee5caf1b8b6282349e79fb998c2ee2(OK) Veles(RU)
12  87.117.12.210:57364   63   d0ee5caf1b8b6282349e79fb998c2ee2(OK) bezpointov
13  89.251.75.101:2304    31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Rolevik
14  178.88.11.177:13094   93   d0ee5caf1b8b6282349e79fb998c2ee2(OK) GhostWarrior
15  95.82.216.154:2304    31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Artem_Mario
17  77.79.128.173:2304    31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) tomson
(15 players in total)
")]
        public void List_Test(string message)
        {
            var serverMessage = new ServerMessage(0, message);
            var recognizer = new PlayerListRecognizer();
            Assert.IsTrue(recognizer.CanRecognize(serverMessage));
        }


        [Test]
        [TestCase(@"
Players on server:
[#] [IP Address]:[Port] [Ping] [GUID] [Name]
--------------------------------------------------
0   213.27.75.36:2304     63   d0ee5caf1b8b6282349e79fb998c2ee2(OK) RatniK
1   194.135.155.161:21519 62   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Wifleem
2   188.242.156.212:2304  32   d0ee5caf1b8b6282349e79fb998c2ee2(OK) kovt
3   176.62.108.109:2304   47   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Ice
4   213.88.77.71:2304     16   d0ee5caf1b8b6282349e79fb998c2ee2(OK) 12s
5   95.24.235.69:2304     0    d0ee5caf1b8b6282349e79fb998c2ee2(OK) Frenzy (Lobby)
7   213.141.128.143:2304  0    d0ee5caf1b8b6282349e79fb998c2ee2(OK) Горизонт
8   95.59.46.59:17540     62   d0ee5caf1b8b6282349e79fb998c2ee2(OK) FaZa
9   31.130.124.118:56750  31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) ReLogic
11  185.13.112.89:18891   0    d0ee5caf1b8b6282349e79fb998c2ee2(OK) Veles(RU)
12  87.117.12.210:57364   63   d0ee5caf1b8b6282349e79fb998c2ee2(OK) bezpointov
13  89.251.75.101:2304    31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Rolevik
14  178.88.11.177:13094   93   d0ee5caf1b8b6282349e79fb998c2ee2(OK) GhostWarrior
15  95.82.216.154:2304    31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Artem_Mario
17  77.79.128.173:2304    31   d0ee5caf1b8b6282349e79fb998c2ee2(OK) tomson
]
")]
        [TestCase(@"
Players on server:
[#] [IP Address]:[Port] [Ping] [GUID] [Name]
--------------------------------------------------
0   213.27.75.36:2304     63   d0ee5caf1b8b6282349e79fb998c2ee2(OK) RatniK
1   194.135.155.161:21519 62   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Wifleem
2   188.242.156.212:2304  32   d0ee5caf1b8b6282349e79fb998c2ee2(OK) kovt
3   176.62.108.109:2304   47   d0ee5caf1b8b6282349e79fb998c2ee2(OK) Ice
4   213.88.77.71:2304     16   d0ee5caf1b8b6282349e79fb998c2ee2(OK) 12s
5   95.24.235.69:2304     0    d0ee5caf1b8b6282349e79fb998c2ee2(OK) Frenzy (Lobby)
7   213.141.128.143:2304  0    d0ee5caf1b8b6282349e79fb998c2ee2(OK) Горизонт
8   95.59.46.59:17540     62   wrongGUID")]
        public void List_NOT_CORECT_Test(string message)
        {
            var serverMessage = new ServerMessage(0, message);
            var recognizer = new PlayerListRecognizer();
            Assert.IsFalse(recognizer.CanRecognize(serverMessage));
        }
    }
}