using Arma3BE.Server.Abstract;
using BattleNET;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arma3BE.Server.Mocks
{
    public class MockBattleEyeServer : IBattlEyeServer
    {
        private Timer _timer;

        public MockBattleEyeServer()
        {
            _timer = new Timer(Tick, null, 2000, Timeout.Infinite);
        }

        private void Tick(object state)
        {
            MockMessage($"(Global) bot: Текущее время {DateTime.UtcNow}");
            MockMessage("RCon admin #4 (99.99.99.99:9999) logged in");
            MockMessage("(Unknown) ToxaBes: тест для тима");

            //MockMessage($"Player #12 sanya disconnected");
            //MockMessage($"Player #1 Baz (be1a6a29ee0eb0851b586b2ed97ad110) has been kicked by BattlEye: Client not responding");
            //MockMessage($"Player #19 Gektor (23d1b17ce5e26ce5f69f2bddbe781d9e) has been kicked by BattlEye: Admin Ban ([vosur][15.09.15 15:14:56] Sabotage)");
            //MockMessage($"Player #6 Atamur (002da0e59f61916bc13ef5ef9bfee7e7) has been kicked by BattlEye: Admin Kick ([tim][22.11.15 15:19:44] Flud)");
            //MockMessage($"Player #11 Iteron (46.98.121.255:2304) connected");
            //MockMessage($"Verified GUID (e3807f6355d182b3ecb7eb7cf1540d40) of player #11 Iteron");
            //MockMessage($"Player #21 TipsyOstrich - GUID: c7679b5ec67c557e479a2a5057083a26 (unverified)");

            MockMessage(GetPlayers());

            _timer?.Change(5000, Timeout.Infinite);
        }


        private int _messageIds;

        private void MockMessage(string message)
        {
            BattlEyeMessageReceived?.Invoke(new BattlEyeMessageEventArgs(message, ++_messageIds));
        }

        public void Dispose()
        {
            DisposeTimer();
        }

        private void DisposeTimer()
        {
            _timer?.Dispose();
            _timer = null;
        }

        public bool Connected { get; private set; }

        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            Task.Factory.StartNew(() =>
            {
                MockMessage($"(Global) bot: Sended command {command} with params {parameters}");

                switch (command)
                {
                    case BattlEyeCommand.Players:
                        var pl = GetPlayers();
                        MockMessage(pl);
                        break;
                    case BattlEyeCommand.Bans:
                        MockMessage(bans);
                        break;
                    case BattlEyeCommand.Missions:
                        MockMessage(missions);
                        break;
                    case BattlEyeCommand.admins:
                        MockMessage(admins);
                        break;
                }

            });

            return 0;
        }

        public void Disconnect()
        {
            Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(t =>
            {
                Connected = false;
                OnBattlEyeDisconnected(new BattlEyeDisconnectEventArgs(default(BattlEyeLoginCredentials), BattlEyeDisconnectionType.Manual));
            });
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;

        public BattlEyeConnectionResult Connect()
        {
            Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(t =>
            {
                Connected = true;
                OnBattlEyeConnected(new BattlEyeConnectEventArgs(default(BattlEyeLoginCredentials), BattlEyeConnectionResult.Success));
            }).ConfigureAwait(false);

            return BattlEyeConnectionResult.Success;
        }

        private static string ChatMessage
        {
            get { return $"(Global) bot: Текущее время {DateTime.UtcNow}"; }
        }


        private static string prev = null;
        static string GetPlayers()
        {
            prev = prev == players2 ? players : players2;
            return prev;
        }


        private static string players =
            @"Players on server:
[#] [IP Address]:[Port] [Ping] [GUID] [Name]
--------------------------------------------------
0   46.149.73.255:2304     140  2c65319280ac51b220aef667552a7f06(OK) 0dmin
1   195.64.206.232:50652  205  5b0f07ce2400706dc9b3c7519beb5372(OK) SEGITO
2   81.20.205.242:17319   105  80f14b5add6cad33d5efe876cead577b(OK) Shinku
3   88.214.186.177:62404  435  2b3a3b6556a8c55dbb982de15f936a04(OK) portugalec
4   212.3.137.202:52971   105  2bae76795dc6dde5daf4681946530dcf(OK) DAN
5   78.60.57.242:2304     205  44ecfad5812b18f1cb6fce2ba0656de3(OK) Xenus
6   5.167.178.186:2304    105  ebabb77436d464d69f34741666f2cff3(OK) Free Style
7   83.149.44.204:63055   205  8fd0670162548982c2b780ba0397efeb(OK) Alex
8   128.71.243.56:2304    112  e0ebb4d1c80afc7d4063ace915e4a1b0(OK) FunnyBlooD
9   31.8.2.185:19945      265  bb70b1a929a5c20cb4e4ee04cb3f31da(OK) Renegade
10  37.232.159.82:2354    105  20446897121fafa82c1069ed07820f27(OK) Максим
11  176.213.109.193:2304  134  9d255d146ad8dc7a4aef240f828b1fb2(OK) ГНК
12  90.226.223.35:2304    123  6696c4be65554dd945475f3746bbcfa4(OK) Cheeki Breeki
14  78.60.254.40:2304     123  a0e3a667e2df374b2bcb153f83eaa172(OK) RomaFreemen417
15  128.71.57.216:2304    105  1abb90bd84d47007f1ea369ea1d2f15c(OK) rinat
17  95.56.214.138:11069   205  ac93a589cc1b5b89396caa6f322fb2f3(OK) GhostWarrior
(16 players in total)";


        private static string players2 =
            @"Players on server:
[#] [IP Address]:[Port] [Ping] [GUID] [Name]
--------------------------------------------------
0   46.149.73.255:2304     140  2c65319280ac51b220aef667552a7f06(OK) 0dmin
3   88.214.186.177:62404  435  2b3a3b6556a8c55dbb982de15f936a04(OK) portugalec
4   212.3.137.202:52971   105  2bae76795dc6dde5daf4681946530dcf(OK) DAN
5   78.60.57.242:2304     205  44ecfad5812b18f1cb6fce2ba0656de3(OK) Xenus
6   5.167.178.186:2304    105  ebabb77436d464d69f34741666f2cff3(OK) Free Style
7   83.149.44.204:63055   205  8fd0670162548982c2b780ba0397efeb(OK) Alex
8   128.71.243.56:2304    112  e0ebb4d1c80afc7d4063ace915e4a1b0(OK) FunnyBlooD
9   31.8.2.185:19945      265  bb70b1a929a5c20cb4e4ee04cb3f31da(OK) Renegade
10  37.232.159.82:2354    105  20446897121fafa82c1069ed07820f27(OK) Максим
11  176.213.109.193:2304  134  9d255d146ad8dc7a4aef240f828b1fb2(OK) ГНК
12  90.226.223.35:2304    123  6696c4be65554dd945475f3746bbcfa4(OK) Cheeki Breeki
14  78.60.254.40:2304     123  a0e3a667e2df374b2bcb153f83eaa172(OK) RomaFreemen417
15  128.71.57.216:2304    105  1abb90bd84d47007f1ea369ea1d2f15c(OK) rinat
17  95.56.214.138:11069   205  ac93a589cc1b5b89396caa6f322fb2f3(OK) GhostWarrior
(14 players in total)";

        private static string admins =
            @"Connected RCon admins:
[#] [IP Address]:[Port]
-----------------------------
0 46.149.73.255:58606
1 31.8.2.185:19880
2 109.173.19.42:54771
3 89.70.28.82:49701
";

        private static string missions =
            @"Missions on server:
RF_Liberators_0_0_7.Sara.pbo
RF_partisan_rhs_0_8_5.Takistan.pbo
RF_Liberators_0_0_6.Sara.pbo
RF_SC_VTN_0_4_2.Catalina.pbo
";

        private static string bans =
            @"GUID Bans:
[#] [GUID] [Minutes left] [Reason]
----------------------------------------
0  331e64806b30277076963283eea16438 perm destruction of equipment, TeamKill. Ban perm
1  8ce223565255b304f01ce5c3b3830d77 perm GameHack. Ban perm
2  09905030887961bf10183125880092bf perm TeamKill. Ban perm
3  c20b9665b392c4c7633d14b151a4deb3 perm destruction of equipment. Ban perm
4  d6ca371207002e53d1a7db741e2c3740 perm Teamkill. Ban perm
5  5a63b36a6047baf28e48a409fd905158 perm TeamKill. Ban perm
6  f9f7daec9196ebd1d58f631a6e91dd63 perm TeamKill. Ban perm
7  6690ea0ce46ea8eb1d8b06828ebe2221 perm TeamKill. Ban perm
8  66666bf19cd978145152974f148424cc perm destruction of equipment. Ban perm
9  7489a913db778954e9bddb4b36469a24 perm TeamKill. Ban perm
10 cbed8802b79d73c26c40650e238ef2bf perm destruction of equipment. Ban perm
11 ab7288b7456b83c204588fb80d15f703 perm Troll. Ban perm
12 bfe7fb953c08e74a1da37cd49f1093ac perm friendly Fire. Ban perm
13 bd9e33db2bac6dd0580b86b126e02359 perm friendly Fire. Ban perm
14 9861f6f2ad52ae0892b8277b72de84b0 perm Friendly fire. Ban perm
15 938ff246b4134b2e412ddf0f2b999fac perm TEamKill. Ban perm
16 eee3227c3d7ba3b628a67bc474e23b31 perm TeamKill. Ban perm
17 a2cb6ec7b2b64423f5ea5a22debfdca0 perm TeamKill. Ban perm
18 a1fd7ffb06dec4b7568ee47d8947e05d perm TeamKill. Ban perm
19 c7763411d5073956e7c517b550772f8a perm TeamKIll. Ban perm
20 f1ba3f606626779cd8d76ec73c3e4ef9 perm destruction of equipment. Ban perm
21 9f0e76794c784d6a324dcf7cdccdb06b perm Friendy fire. Ban perm
22 c50eac45236cacbed9fed6428925e542 perm TeamKill. Ban perm
23 baab3c2baea45e4b280bec2f66ef7db6 perm TeamKill. Ban perm
24 bb21173f0492a3e38681b030a6171629 perm friendly Fire. Ban perm
25 143eea8651030f1bedf316b8dd220035 perm friendly Fire. Ban perm
26 761f5bb129d6218d77666d4fab71cca0 perm TeamKill. Ban perm
27 faaba22913ce88e8ac4b7cac12981474 perm TeamKill. Ban perm
28 8306f6d1ae902e1db5411070ee9750f1 perm TeamKill. Ban perm
29 09bc80ebf8f703e905de69d7f804a831 perm TeamKill. Ban perm
30 1911f7800f5578f4921c60bc0858f4e5 perm TeamKill. Ban perm
31 ca1cc768e40080fa755b0c08fca0d791 perm TeamKill. Ban perm
32 efe5b062482125757f5792aaf94d724d perm TeamKill. Ban perm
33 edfaf581f7943535bc61030694262577 perm TeamKill. Ban perm
34 942dea253f71745a96ba60d730f7de86 perm TeamKill. Ban perm

IP Bans:
[#] [IP Address] [Minutes left] [Reason]
----------------------------------------------";

        protected virtual void OnBattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            BattlEyeConnected?.Invoke(args);
        }

        protected virtual void OnBattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            BattlEyeDisconnected?.Invoke(args);
        }
    }
}