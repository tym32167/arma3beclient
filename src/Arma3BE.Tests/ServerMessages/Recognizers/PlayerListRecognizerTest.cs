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
0   5.141.88.80:59558     31   someGUIDnumbers(OK) hobot
1   37.204.36.17:2304     15   someGUIDnumbers(OK) ZeRoKoOl
2   94.75.36.1:21455      47   someGUIDnumbers(OK) ~SCORPION~
3   5.139.190.80:2304     47   someGUIDnumbers(OK) StasCherniy
4   188.186.73.4:2304     47   someGUIDnumbers(OK) Dread
5   62.122.208.170:2304   16   someGUIDnumbers(OK) Ranger  (Lobby)
6   37.193.69.47:2304     47   someGUIDnumbers(OK) MORKLEB (Lobby)
7   5.44.174.54:2304      109  someGUIDnumbers(OK) tuman-CS
9   46.180.53.73:52060    62   someGUIDnumbers(OK) AL1R
10  188.244.37.117:2304   15   someGUIDnumbers(OK) ����177
11  95.25.234.161:2304    78   someGUIDnumbers(OK) Jo_Boo
12  176.100.92.208:2304   110  someGUIDnumbers(OK) Kvazar
13  31.163.200.93:2304    78   someGUIDnumbers(OK) 34th
14  178.34.203.78:18944   47   someGUIDnumbers(OK) Aleksandres (Lobby)
15  46.150.163.240:2304   16   someGUIDnumbers(OK) (F)TypucT
16  81.1.232.163:56395    78   someGUIDnumbers(OK) [BB]Ghostis
17  217.197.240.227:2304  16   someGUIDnumbers(OK) FlinT
18  176.36.211.138:2304   31   someGUIDnumbers(OK) Maxik
19  85.113.135.224:21253  15   someGUIDnumbers(OK) Yarik
20  164.215.91.74:2304    125  someGUIDnumbers(OK) Qpitman
21  193.169.36.194:33475  125  someGUIDnumbers(OK) Urielf
22  77.222.103.104:2304   47   someGUIDnumbers(OK) Mr_Fail
23  77.37.145.162:2304    16   someGUIDnumbers(OK) Legioner
24  178.218.106.147:2304  141  someGUIDnumbers(OK) DRY
25  91.236.224.109:2304   47   someGUIDnumbers(OK) Lincoln
27  94.232.234.25:2304    172  someGUIDnumbers(OK) [RSA]KA5
28  176.125.78.22:2304    125  someGUIDnumbers(OK) ALON
29  83.220.238.189:30140  110  someGUIDnumbers(OK) 4poK
30  37.146.201.30:2304    63   someGUIDnumbers(OK) Berns_Mack
31  46.231.212.34:2304    0    someGUIDnumbers(OK) [GH] Listopad
33  77.50.204.111:2304    16   someGUIDnumbers(OK) npu3pak
34  83.246.179.250:2304   78   someGUIDnumbers(OK) CrazeXoma
(32 players in total)")]
        [TestCase(@"
Players on server:
[#] [IP Address]:[Port] [Ping] [GUID] [Name]
--------------------------------------------------
0   213.27.75.36:2304     63   SomeGUIDNUMBERS(OK) RatniK
1   194.135.155.161:21519 62   SomeGUIDNUMBERS(OK) Wifleem
2   188.242.156.212:2304  32   SomeGUIDNUMBERS(OK) kovt
3   176.62.108.109:2304   47   SomeGUIDNUMBERS(OK) Ice
4   213.88.77.71:2304     16   SomeGUIDNUMBERS(OK) 12s
5   95.24.235.69:2304     0    SomeGUIDNUMBERS(OK) Frenzy (Lobby)
7   213.141.128.143:2304  0    SomeGUIDNUMBERS(OK) Горизонт
8   95.59.46.59:17540     62   SomeGUIDNUMBERS(OK) FaZa
9   31.130.124.118:56750  31   SomeGUIDNUMBERS(OK) ReLogic
11  185.13.112.89:18891   0    SomeGUIDNUMBERS(OK) Veles(RU)
12  87.117.12.210:57364   63   SomeGUIDNUMBERS(OK) bezpointov
13  89.251.75.101:2304    31   SomeGUIDNUMBERS(OK) Rolevik
14  178.88.11.177:13094   93   SomeGUIDNUMBERS(OK) GhostWarrior
15  95.82.216.154:2304    31   SomeGUIDNUMBERS(OK) Artem_Mario
17  77.79.128.173:2304    31   SomeGUIDNUMBERS(OK) tomson
(15 players in total)
")]
        public void Player_List_Test(string message)
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
0   213.27.75.36:2304     63   SomeGUIDNUmbers(OK) RatniK
1   194.135.155.161:21519 62   SomeGUIDNUmbers(OK) Wifleem
2   188.242.156.212:2304  32   SomeGUIDNUmbers(OK) kovt
3   176.62.108.109:2304   47   SomeGUIDNUmbers(OK) Ice
4   213.88.77.71:2304     16   SomeGUIDNUmbers(OK) 12s
5   95.24.235.69:2304     0    SomeGUIDNUmbers(OK) Frenzy (Lobby)
7   213.141.128.143:2304  0    SomeGUIDNUmbers(OK) Горизонт
8   95.59.46.59:17540     62   SomeGUIDNUmbers(OK) FaZa
9   31.130.124.118:56750  31   SomeGUIDNUmbers(OK) ReLogic
11  185.13.112.89:18891   0    SomeGUIDNUmbers(OK) Veles(RU)
12  87.117.12.210:57364   63   SomeGUIDNUmbers(OK) bezpointov
13  89.251.75.101:2304    31   SomeGUIDNUmbers(OK) Rolevik
14  178.88.11.177:13094   93   SomeGUIDNUmbers(OK) GhostWarrior
15  95.82.216.154:2304    31   SomeGUIDNUmbers(OK) Artem_Mario
17  77.79.128.173:2304    31   SomeGUIDNUmbers(OK) tomson
]
")]
        [TestCase(@"
Players on server:
[#] [IP Address]:[Port] [Ping] [GUID] [Name]
--------------------------------------------------
0   213.27.75.36:2304     63   someGUID(OK) RatniK
1   194.135.155.161:21519 62   someGUID(OK) Wifleem
2   188.242.156.212:2304  32   someGUID(OK) kovt
3   176.62.108.109:2304   47   someGUID(OK) Ice
4   213.88.77.71:2304     16   someGUID(OK) 12s
5   95.24.235.69:2304     0    someGUID(OK) Frenzy (Lobby)
7   213.141.128.143:2304  0    someGUID(OK) Горизонт
8   95.59.46.59:17540     62   wrongGUID")]
        public void Player_List_NOT_CORECT_Test(string message)
        {
            var serverMessage = new ServerMessage(0, message);
            var recognizer = new PlayerListRecognizer();
            Assert.IsFalse(recognizer.CanRecognize(serverMessage));
        }
    }
}