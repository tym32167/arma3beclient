using Arma3BE.Server.Models;
using Arma3BE.Server.Recognizers;
using NUnit.Framework;

namespace Arma3BEClient.Tests.ServerMessages.Recognizers
{
    [TestFixture]
    public class BanListRecognizerTest
    {
        [Test]
        [TestCase(@"GUID Bans:
[#] [GUID] [Minutes left] [Reason]
----------------------------------------
0  d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill,destruction of equipment. Ban perm!
1  d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment, TeamKill. Ban perm
2  d0ee5caf1b8b6282349e79fb998c2ee2 perm GameHack. Ban perm
3  d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
4  d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment. Ban perm
5  d0ee5caf1b8b6282349e79fb998c2ee2 perm Teamkill. Ban perm
6  d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
7  d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
8  d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
9  d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment. Ban perm
10 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
11 d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment. Ban perm
12 d0ee5caf1b8b6282349e79fb998c2ee2 perm Troll. Ban perm
13 d0ee5caf1b8b6282349e79fb998c2ee2 perm friendly Fire. Ban perm
14 d0ee5caf1b8b6282349e79fb998c2ee2 perm friendly Fire. Ban perm
15 d0ee5caf1b8b6282349e79fb998c2ee2 perm Friendly fire. Ban perm
16 d0ee5caf1b8b6282349e79fb998c2ee2 perm TEamKill. Ban perm
17 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
18 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
19 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
20 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKIll. Ban perm
21 d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment. Ban perm
22 d0ee5caf1b8b6282349e79fb998c2ee2 perm Friendy fire. Ban perm
23 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
24 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
25 d0ee5caf1b8b6282349e79fb998c2ee2 perm friendly Fire. Ban perm
26 d0ee5caf1b8b6282349e79fb998c2ee2 perm friendly Fire. Ban perm
27 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
28 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
29 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
30 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
31 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
32 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
33 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
34 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
35 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
36 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
37 d0ee5caf1b8b6282349e79fb998c2ee2 perm Cheater. Ban perm
38 d0ee5caf1b8b6282349e79fb998c2ee2 perm Cheater. Ban perm
39 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
40 d0ee5caf1b8b6282349e79fb998c2ee2 perm diversions. Ban perm
41 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
42 d0ee5caf1b8b6282349e79fb998c2ee2 perm friendly Fire. Ban perm
43 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban Perm
44 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
45 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
46 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
47 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
48 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
49 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
50 d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment. Ban perm
51 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
52 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
53 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
54 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
55 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
56 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
57 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
58 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
59 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
60 d0ee5caf1b8b6282349e79fb998c2ee2 perm Diversions. Ban perm
61 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
62 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
63 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill,Diversions. Ban perm
64 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
65 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. sabotage. Ban perm
66 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
67 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
68 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
69 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
70 d0ee5caf1b8b6282349e79fb998c2ee2 perm TK. ����������� ������� �� �����
71 d0ee5caf1b8b6282349e79fb998c2ee2 perm AntiRussia
72 d0ee5caf1b8b6282349e79fb998c2ee2 perm Where are your manners? Goodbye
73 d0ee5caf1b8b6282349e79fb998c2ee2 perm Admin Ban - Teamkill, PERM!
74 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. BAN PERMANENT
75 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban Perm!
76 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
77 d0ee5caf1b8b6282349e79fb998c2ee2 perm Perm Ban from Mexan Rw1
78 d0ee5caf1b8b6282349e79fb998c2ee2 perm Oskorblenie admina
79 d0ee5caf1b8b6282349e79fb998c2ee2 perm TimKill! Perm
80 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
81 d0ee5caf1b8b6282349e79fb998c2ee2 perm Teamkill
82 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban Perm
83 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
84 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
85 d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment,TeamKill. Ban perm
86 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
87 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
88 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
89 d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment. Ban perm
90 d0ee5caf1b8b6282349e79fb998c2ee2 perm Cheater. Ban perm
91 d0ee5caf1b8b6282349e79fb998c2ee2 perm Teamkill (A11archer, USA)
92 d0ee5caf1b8b6282349e79fb998c2ee2 perm Hack of the server 29.07.2014
93 d0ee5caf1b8b6282349e79fb998c2ee2 perm No bad no bad
94 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][18.08.14 12:15:25] Teamkill
95 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][18.08.14 13:24:27] neadekvat, ban by a11archer
96 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][18.08.14 21:28:48] Teamkill
97 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][18.08.14 22:11:52] Teamkill
98 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][19.08.14 04:44:36] Teamkill
99 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][19.08.14 05:42:30] TEAMKILL
100 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][19.08.14 05:58:35] TEAMKILL
101 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][19.08.14 06:00:28] teamkill
102 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][19.08.14 06:53:25] SABOTAGE
103 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][20.08.14 14:02:14] Teamkill
104 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][20.08.14 14:02:36] Teamkill
105 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][20.08.14 22:34:14] Teamkill
106 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][20.08.14 22:36:09] Teamkill
107 d0ee5caf1b8b6282349e79fb998c2ee2 perm [A11Archer][21.08.14 06:08:39] Teamkill � ������ ����
108 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][22.08.14 22:38:33] Troll anti russian
109 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][23.08.14 16:04:28] Teamkill
110 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][23.08.14 16:55:59] Sabotage
111 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][23.08.14 18:13:21] Sabotage
112 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][23.08.14 21:17:00] Teamkill
113 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][24.08.14 14:12:40] Teamkill
114 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][24.08.14 21:14:55] Teamkill
115 d0ee5caf1b8b6282349e79fb998c2ee2 perm [A11Archer][25.08.14 07:05:04] TK and vehicle destroy
116 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][26.08.14 06:37:17] Sabotage
117 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][26.08.14 08:58:44] Teamkill
118 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][26.08.14 09:24:20] neadekvtat
119 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][26.08.14 10:04:09] Teamkill
120 d0ee5caf1b8b6282349e79fb998c2ee2 perm [A11Archer][27.08.14 04:38:09] ������� �� ������?
121 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][27.08.14 18:27:03] Teamkill
122 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
123 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][28.08.14 23:27:17] Sabotage
124 d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment. Ban perm
125 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][29.08.14 21:14:21] Teamkill
126 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][30.08.14 10:28:09] Teamkill
127 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][30.08.14 14:51:58] Teamkill
128 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][30.08.14 18:12:01] Teamkill
129 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][30.08.14 18:12:07] Teamkill
130 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][31.08.14 02:29:22] Teamkill
131 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][31.08.14 02:29:39] Teamkill
132 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][31.08.14 11:33:30] Teamkill
133 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][01.09.14 10:32:49] Teamkill
134 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][01.09.14 10:33:18] Teamkill
135 d0ee5caf1b8b6282349e79fb998c2ee2 perm TK �� �����
136 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][01.09.14 11:57:59] Vehicle Destroy
137 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][01.09.14 18:01:29] Flud
138 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][01.09.14 18:04:24] Inadequate
139 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][01.09.14 18:04:33] Flud
140 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][01.09.14 19:16:14] Teamkill
141 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][02.09.14 21:34:49] Teamkill, Sabotage
142 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
143 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
144 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
145 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
146 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][06.09.14 11:08:35] Teamkill, VehicleDestroy
147 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][06.09.14 14:23:29] Teamkill
148 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][06.09.14 22:37:40] Teamkill
149 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][07.09.14 13:25:11] Teamkill
150 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][07.09.14 15:34:54] Teamkill,Sabotage
151 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][07.09.14 16:59:53] Sabotage� Teamkill
152 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][09.09.14 12:21:33] Teamkill
153 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][10.09.14 04:46:33] Teamkill
154 d0ee5caf1b8b6282349e79fb998c2ee2 perm GameHack
155 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
156 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][11.09.14 19:54:03] Teamkill
157 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
158 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][13.09.14 02:57:40] Inadequate
159 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][13.09.14 04:20:19] Sabotage
160 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][13.09.14 04:56:26] Inadequate
161 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
162 d0ee5caf1b8b6282349e79fb998c2ee2 perm Bad NickName. Ban perm
163 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban Perm
164 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
165 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
166 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][13.09.14 18:43:40] Teamkill
167 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][14.09.14 10:34:55] Teamkill
168 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][14.09.14 12:38:35] Sabotage, Teamkill
169 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][14.09.14 16:16:41] Teamkill
170 d0ee5caf1b8b6282349e79fb998c2ee2 perm ����������� ������� �� �����
171 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][15.09.14 12:25:59] Teamkill
172 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][15.09.14 12:27:46] Teamkill
173 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][15.09.14 12:33:07] Teamkill
174 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][17.09.14 17:55:36] Teamkill
175 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][17.09.14 18:33:29] Teamkill
176 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][17.09.14 18:33:47] Sabotage
177 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][19.09.14 20:04:06] Teamkill
178 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. VehicleDestroy. PERMANENT!
179 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][17.09.14 12:19:11] Teamkill
180 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][20.09.14 15:13:33] Teamkill
181 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][22.09.14 09:11:14] Teamkill
182 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][23.09.14 11:09:20] Teamkill
183 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][23.09.14 12:36:19] Teamkill
184 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][23.09.14 13:00:27] Teamkill
185 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][23.09.14 16:57:15] Teamkill
186 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][23.09.14 17:45:15] Teamkill
187 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][24.09.14 10:51:51] Teamkill
188 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][25.09.14 13:37:33] Sabotage
189 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][25.09.14 13:39:06] Sabotage
190 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][26.09.14 09:24:32] Teamkill
191 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][27.09.14 12:33:58] Inadequate
192 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][27.09.14 12:34:35] Sabotage
193 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][27.09.14 14:07:55] Sabotage
194 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][28.09.14 09:55:51] Teamkill
195 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][26.09.14 18:54:50] Reklama
196 d0ee5caf1b8b6282349e79fb998c2ee2 perm TK
197 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][29.09.14 18:33:17] Sabotage
198 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][01.10.14 12:23:53] Teamkill
199 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][03.10.14 13:13:59] Flud
200 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][03.10.14 17:04:22] Teamkill Sabotage
201 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][04.10.14 12:56:23] Teamkill
202 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][05.10.14 07:17:25] Teamkill
203 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][05.10.14 17:53:55] Sabotage
204 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][05.10.14 17:56:15] Sabotage
205 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][10.10.14 08:24:19] Teamkill
206 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][10.10.14 19:53:06] Teamkill
207 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[K]STELS][12.10.14 16:52:44] MassTeamKill. Ban perm
208 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][14.10.14 12:36:31] Sabotage
209 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][14.10.14 18:19:58] Sabotage
210 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][14.10.14 19:19:47] Flud
211 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][14.10.14 20:51:03] Teamkill
212 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][14.10.14 21:21:12] Flud
213 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][15.10.14 14:29:34] Teamkill perm!
214 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][15.10.14 18:32:38] Troll
215 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[K]STELS][17.10.14 13:03:09] Cheating. Ban perm
216 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][17.10.14 13:42:02] Teamkill
217 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][17.10.14 17:03:26] Teamkill
218 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][18.10.14 01:36:46] Teamkill. Fill nickname
219 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][18.10.14 11:10:04] Sabotage
220 d0ee5caf1b8b6282349e79fb998c2ee2 perm [KA5][18.10.14 14:52:00] Sabotage
221 d0ee5caf1b8b6282349e79fb998c2ee2 perm Troll. Ban perm
222 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
223 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[K]STELS][22.10.14 16:26:27] MassTeamKill. Ban perm
224 d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment. Ban perm
225 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][23.10.14 17:17:45] Teamkill perm
226 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][24.10.14 09:52:42] Sabotage
227 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][24.10.14 11:37:01] Teamkill
228 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][24.10.14 19:19:58] Teamkill
229 d0ee5caf1b8b6282349e79fb998c2ee2 perm [longbow][24.10.14 15:55:20] teamkill. ban perm
230 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
231 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
232 d0ee5caf1b8b6282349e79fb998c2ee2 perm Sabotage. Ban perm
233 d0ee5caf1b8b6282349e79fb998c2ee2 perm Sabotage. Ban perm
234 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][24.10.14 14:20:04] Teamkill
235 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][24.10.14 15:19:20] Teamkill
236 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][24.10.14 16:31:46] Teamkill
237 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[K]STELS][24.10.14 22:31:50] Mass.Teamkill. Ban perm
238 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[K]STELS][24.10.14 22:33:56] Mass.Teamkill. Ban perm
239 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][25.10.14 15:25:15] VehicleDestroy again. PERMANENT!!
240 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][25.10.14 15:48:38] Teamkill
241 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][25.10.14 18:45:35] Teamkill
242 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][25.10.14 19:29:35] Teamkill
243 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][25.10.14 19:34:40] Inadequate
244 d0ee5caf1b8b6282349e79fb998c2ee2 perm [KA5][26.10.14 03:47:37] Teamkill
245 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][26.10.14 10:55:51] Sabotage
246 d0ee5caf1b8b6282349e79fb998c2ee2 perm [A11Archer][28.08.14 12:47:03] TK
247 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][26.10.14 14:18:16] Sabotage
248 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][27.10.14 13:46:07] VehicleDestroy
249 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][29.10.14 13:32:15] Teamkill Sabotage Perm
250 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][29.10.14 19:29:22] Sabotage
251 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][29.10.14 19:32:57] Teamkill
252 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][01.11.14 15:43:08] Sabotage, Teamkill
253 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][02.11.14 18:52:04] Teamkill
254 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][04.11.14 08:29:31] Troll
255 d0ee5caf1b8b6282349e79fb998c2ee2 perm [longbow][04.11.14 16:40:27] Teamkill
256 d0ee5caf1b8b6282349e79fb998c2ee2 perm [longbow][04.11.14 16:36:46] Sabotage
257 d0ee5caf1b8b6282349e79fb998c2ee2 perm [longbow][04.11.14 16:39:58] Teamkill
258 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][04.11.14 17:56:24] Teamkill
259 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][04.11.14 18:21:03] Sabotage, VehicleDestroy
260 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][05.11.14 15:12:26] TeamKill, deliberate deception
261 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][05.11.14 16:15:09] VehicleStealing
262 d0ee5caf1b8b6282349e79fb998c2ee2 perm [longbow][04.11.14 18:00:45] Teamkill
263 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][07.11.14 16:00:44] Sabotage
264 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][08.11.14 15:38:22] Teamkill
265 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][08.11.14 17:05:02] VehicleDestroy
266 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][08.11.14 18:56:13] Sabotage
267 d0ee5caf1b8b6282349e79fb998c2ee2 perm [longbow][09.11.14 09:36:47] Teamkill
268 d0ee5caf1b8b6282349e79fb998c2ee2 perm [longbow][10.11.14 07:44:54] Sabotage
269 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][10.11.14 19:36:28] Teamkill
270 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][11.11.14 15:23:41] Sabotage
271 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][11.11.14 15:29:49] Sabotage PERM!
272 d0ee5caf1b8b6282349e79fb998c2ee2 perm [longbow][10.11.14 13:29:17] Sabotage
273 d0ee5caf1b8b6282349e79fb998c2ee2 perm [TRenG][12.11.14 16:38:19] Sabotage
274 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][17.11.14 19:22:47] VehicleDestroy
275 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][29.10.14 14:57:22] Sabotage
276 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][29.10.14 14:57:37] Teamkill
277 d0ee5caf1b8b6282349e79fb998c2ee2 perm [��??����][02.11.14 09:13:35] Flud
278 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][02.11.14 11:13:18] Teamkill
279 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][03.11.14 14:43:44] Teamkill
280 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][12.11.14 08:19:54] Teamkill
281 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][16.11.14 14:53:55] Teamkill
282 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][15.11.14 09:20:17] Sabotage
283 d0ee5caf1b8b6282349e79fb998c2ee2 perm [longbow][14.11.14 19:47:13] cheater
284 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][17.11.14 21:56:46] Inadequate
285 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][24.10.14 16:40:12] Sabotage
286 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
287 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][24.10.14 15:36:28] Teamkill
288 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][24.10.14 16:46:41] Sabotage
289 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][27.10.14 14:47:18] Teamkill
290 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][19.11.14 16:23:53] Sabotage
291 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][19.11.14 18:42:26] Cheater
292 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][20.11.14 16:56:18] Teamkill
293 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][20.11.14 16:56:18] Teamkill
294 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][22.11.14 13:31:18] Sabotage
295 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][22.11.14 15:21:41] Teamkill
296 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][25.11.14 16:41:40] Sabotage
297 d0ee5caf1b8b6282349e79fb998c2ee2 perm [longbow][24.11.14 20:30:33] Teamkill
298 d0ee5caf1b8b6282349e79fb998c2ee2 perm [KA5][27.11.14 15:32:17] Teamkill, sabotage
299 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][28.11.14 15:54:43] cheater
300 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][29.11.14 11:48:00] Teamkill
301 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][29.11.14 21:15:07] Sabotage
302 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][30.11.14 17:49:45] Teamkill
303 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][30.11.14 17:50:24] Teamkill
304 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][01.12.14 17:44:53] Teamkill
305 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][02.12.14 05:59:05] Sabotage
306 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][02.12.14 14:32:14] Sabotage
307 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][02.12.14 16:20:03] Inadequate
308 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][02.12.14 16:28:22] Sabotage
309 d0ee5caf1b8b6282349e79fb998c2ee2 perm [KA5][03.12.14 12:30:57] Teamkill
310 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][04.12.14 19:37:36] Sabotage
311 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][06.12.14 08:44:43] Sabotage
312 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][06.12.14 20:47:29] Sabotage, TeamKill
313 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][08.10.14 16:02:58] Sabotage
314 d0ee5caf1b8b6282349e79fb998c2ee2 perm ��� �����
315 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
316 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][19.08.14 06:00:28] teamkill
317 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][06.12.14 23:31:40] Teamkill
318 d0ee5caf1b8b6282349e79fb998c2ee2 perm ��� �����
319 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][11.12.14 16:00:00] Teamkill, Sabotage
320 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][16.12.14 17:18:46] Teamkill
321 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][16.12.14 17:07:22] cheat
322 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][11.12.14 20:35:30] Teamkill
323 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][04.11.14 12:22:16] Sabotage
324 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][17.12.14 15:40:59] Teamkill
325 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][20.12.14 14:02:05] Cheater
326 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][20.12.14 23:43:42] Sabotage
327 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][21.12.14 00:46:42] Sabotage
328 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][21.12.14 14:46:55] Sabotage, Teamkill
329 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][22.12.14 18:19:42] Sabotage
330 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][23.12.14 18:37:23] Teamkill
331 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][24.12.14 20:00:47] Sabotage, Teamkill 
332 d0ee5caf1b8b6282349e79fb998c2ee2 perm [KA5][25.12.14 12:53:42] Sabotage
333 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][26.12.14 15:31:57] Sabotage
334 d0ee5caf1b8b6282349e79fb998c2ee2 perm [TRenG][28.12.14 09:05:00] ����������� �������
335 d0ee5caf1b8b6282349e79fb998c2ee2 perm [KA5][29.12.14 06:30:51] Sabotage
336 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][30.12.14 20:27:34] Sabotage
337 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][01.01.15 00:50:27] Sabotage
338 d0ee5caf1b8b6282349e79fb998c2ee2 perm [KA5][01.01.15 06:40:15] Teamkill
339 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][01.01.15 17:52:23] Teamkill, Sabotage
340 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][03.01.15 17:20:32] Sabotage, TK
341 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][06.01.15 17:03:43] Inadequate, Teamkill, Sabotage
342 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][06.01.15 17:04:39] Inadequate, Teamkill, Sabotage
343 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][07.01.15 01:05:27] Teamkill
344 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][07.01.15 03:31:40] Inadequate
345 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][07.01.15 03:32:32] Inadequate
346 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][07.01.15 12:42:51] Sabotage
347 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][09.01.15 08:12:46] Teamkill
348 d0ee5caf1b8b6282349e79fb998c2ee2 perm [A11Archer][09.01.15 15:46:40] Cheating 09.01.2015 name - dennis
349 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][09.01.15 21:17:28] Sabotage
350 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][24.01.15 15:15:04] Sabotage
351 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][25.01.15 12:02:37] Teamkill
352 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][28.01.15 16:38:04] Teamkill
353 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][01.02.15 00:45:42] Sabotage
354 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][02.02.15 10:02:21] Sabotage
355 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill. Ban perm
356 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill.destruction of equipment. Ban perm
357 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill.destruction of equipment. Ban perm
358 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill.destruction of equipment. Ban perm
359 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][14.02.15 11:23:24] Teamkill
360 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][15.02.15 11:37:56] Sabotage
361 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][15.02.15 19:21:59] Sabotage
362 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][16.02.15 13:04:36] Sabotage
363 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][19.02.15 08:39:11] Inadequate,  Cheater
364 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][19.02.15 08:39:49] Inadequate,  Cheater
365 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][19.02.15 13:29:09] Sabotage
366 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][19.02.15 18:00:16] Sabotage
367 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][19.02.15 18:00:41] Sabotage
368 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][20.02.15 13:59:41] Sabotage
369 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][20.02.15 14:02:52] Sabotage
370 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][22.02.15 09:55:28] Sabotage
371 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][24.02.15 10:49:21] Sabotage,Teamkill
372 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][27.02.15 17:40:23] Sabotage, Teamkill
373 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][01.03.15 13:34:58] Sabotage,Teamkill
374 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][03.03.15 19:35:34] Sabotage
375 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][03.03.15 19:39:21] Sabotage
376 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][05.03.15 14:19:49] Sabotage
377 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][05.03.15 19:53:22] Sabotage
378 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][05.03.15 20:26:57] Cheater
379 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][05.03.15 20:44:51] Cheater
380 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][06.03.15 16:00:07] Sabotage
381 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][06.03.15 19:35:07] Insulting,Teamkill
382 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][06.03.15 21:58:32] Sabotage
383 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][08.03.15 14:08:06] Sabotage
384 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][11.03.15 21:37:15] Sabotage
385 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][11.03.15 21:57:08] Teamkill
386 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Noart][13.03.15 14:44:34] ���������� � ������?
387 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][14.03.15 09:37:43] Sabotage, Teamkill
388 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][14.03.15 12:17:53] Cheater
389 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][16.03.15 19:46:24] Sabotage
390 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][23.03.15 07:55:01] Sabotage
391 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][23.03.15 09:11:24] Sabotage. Theft(award)
392 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Noart][24.03.15 15:13:54] Teamkill
393 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][25.03.15 17:23:59] cheater
394 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][27.03.15 19:06:43] Teamkill, Sabotage
395 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][27.03.15 20:54:38] Teamkill, Sabotage
396 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][28.03.15 08:47:37] rasizm & flud PERM
397 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][28.03.15 21:32:34] Teamkill, Sabotage
398 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][30.03.15 18:20:25] Sabotage
399 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][31.03.15 05:03:57] Sabotage
400 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][01.04.15 10:17:28] Sabotage, Teamkill
401 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][01.04.15 13:27:20] Sabotage
402 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][02.04.15 04:05:42] cheater
403 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][02.04.15 19:28:31] Teamkill, Sabotage
404 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][02.04.15 19:33:00] Sabotage
405 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][02.04.15 19:47:09] Sabotage
406 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][02.04.15 21:59:52] Inadequate
407 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[SO]vosur][08.04.15 18:02:34] Teamkill
408 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][15.04.15 20:16:57] Sabotage
409 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][15.04.15 20:19:09] Sabotage
410 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][16.04.15 10:51:45] Teamkill
411 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][16.04.15 10:52:53] Teamkill
412 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][20.04.15 15:36:04] cheater
413 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][21.04.15 16:05:30] Sabotage
414 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Noart][23.04.15 13:46:37] ���������� �� ���
415 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][03.05.15 19:54:38] cheater
416 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][04.05.15 15:52:41] cheater
417 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][07.05.15 19:39:23] Sabotage
418 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][08.05.15 13:25:31] insulting players on a national bas
419 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][08.05.15 15:43:36] Sabotage
420 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][08.05.15 18:38:34] Sabotage
421 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][10.05.15 08:54:29] Sabotage
422 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][11.05.15 14:52:03] Sabotage
423 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][15.05.15 17:08:07] Sabotage
424 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][20.05.15 14:39:16] �� ��� � ����
425 d0ee5caf1b8b6282349e79fb998c2ee2 420567 [vosur][20.05.15 18:18:43] Troll
426 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][22.05.15 16:17:32] Sabotage, Teamkill
427 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][24.05.15 08:12:04] cheater
428 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][24.05.15 08:34:02] Sabotage
429 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][26.05.15 20:11:10] Sabotage
430 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][28.05.15 17:14:00] Sabotage
431 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][31.05.15 16:53:11] Sabotage
432 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][01.06.15 09:53:04] Sabotage, Teamkill, Theft(award)
433 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[K]STELS][02.06.15 20:31:20] Teamkill
434 d0ee5caf1b8b6282349e79fb998c2ee2 perm [[K]STELS][03.06.15 16:53:12] Teamkill
435 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][06.06.15 13:34:05] cheater
436 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][06.06.15 13:35:04] cheater
437 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][06.06.15 13:35:36] cheater
438 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][06.06.15 21:07:31] Sabotage
439 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][07.06.15 20:28:41] Sabotage
440 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][07.06.15 20:29:32] Sabotage
441 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][08.06.15 09:42:13] CHEATER
442 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][08.06.15 20:33:33] Troll
443 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][08.06.15 20:41:14] Teamkill
444 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][09.06.15 12:43:15] Thieft (Award)
445 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Noart][09.06.15 18:49:48] Sabotage
446 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][13.06.15 16:02:30] Sabotage
447 d0ee5caf1b8b6282349e79fb998c2ee2 perm [STELS][16.06.15 13:53:18] Sabotage
448 d0ee5caf1b8b6282349e79fb998c2ee2 perm [STELS][18.06.15 06:06:43] Teamkill. Ban perm
449 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][23.06.15 13:48:54] Sabotage
450 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][26.06.15 12:52:34] insulting admin
451 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][27.06.15 09:42:37] Sabotage
452 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][27.06.15 23:31:16] Teamkill
453 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Noart][30.06.15 13:48:01] Teamkill
454 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][30.06.15 19:12:12] Sabotage
455 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][02.07.15 19:15:27] Flud,insulting admin,Inadequate
456 d0ee5caf1b8b6282349e79fb998c2ee2 255  [Rembowest14][02.07.15 21:06:27] Sabotage
457 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][02.07.15 21:20:36] Sabotage
458 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][03.07.15 18:59:22] Sabotage
459 d0ee5caf1b8b6282349e79fb998c2ee2 perm [tim][04.07.15 14:41:15] Teamkill
460 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][05.07.15 10:36:02] Teamkill
461 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][05.07.15 10:36:44] Teamkill
462 d0ee5caf1b8b6282349e79fb998c2ee2 perm [RFADMIN][05.07.15 17:32:25] Teamkill
463 d0ee5caf1b8b6282349e79fb998c2ee2 perm [TRenG][11.03.15 09:07:24] Sabotage
464 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Admin][22.03.15 13:49:23] Teamkill
465 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Admin][22.03.15 13:49:34] Teamkill
466 d0ee5caf1b8b6282349e79fb998c2ee2 perm [RFADMIN][23.03.15 09:07:01] Teamkill
467 d0ee5caf1b8b6282349e79fb998c2ee2 perm [RFADMIN][23.03.15 09:30:35] Teamkill
468 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][23.03.15 17:07:34] Teamkill
469 d0ee5caf1b8b6282349e79fb998c2ee2 perm [TRenG][30.03.15 15:53:51] Teamkill
470 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][31.03.15 07:08:37] Teamkill
471 d0ee5caf1b8b6282349e79fb998c2ee2 perm [RFADMIN][02.04.15 23:59:06] Troll
472 d0ee5caf1b8b6282349e79fb998c2ee2 perm [TRenG][03.04.15 17:57:22] Teamkill
473 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][05.04.15 07:41:14] Teamkill
474 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Grim][06.04.15 07:12:21] TEAMKILL
475 d0ee5caf1b8b6282349e79fb998c2ee2 perm [TRenG][12.04.15 13:06:02] Teamkill
476 d0ee5caf1b8b6282349e79fb998c2ee2 perm [TRenG][03.05.15 10:45:06] Teamkill
477 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][30.05.15 14:08:40] Teamkill
478 d0ee5caf1b8b6282349e79fb998c2ee2 perm [�������][02.07.15 18:52:08] Sabotage
479 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Noart][05.07.15 19:53:09] Cheater
480 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Noart][05.07.15 19:53:49] Matias
481 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][06.07.15 16:33:31] Sabotage
482 d0ee5caf1b8b6282349e79fb998c2ee2 5748 [S.W.A.T][06.07.15 16:39:43] Sabotage
483 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Noart][07.07.15 11:15:35] Inadequate
484 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][07.07.15 15:26:56] Sabotage
485 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][07.07.15 23:33:03] Teamkill
486 d0ee5caf1b8b6282349e79fb998c2ee2 perm [vosur][08.07.15 10:29:55] Sabotage
487 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Noart][08.07.15 20:18:01] Teamkill
488 d0ee5caf1b8b6282349e79fb998c2ee2 perm [����][09.07.15 10:52:58] bad name ""Administrator""
489 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Noart][09.07.15 17:00:24] Sabotage
490 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][10.07.15 17:12:34] Sabotage, Teamkill
491 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][11.07.15 12:31:02] Sabotage, Teamkill
492 d0ee5caf1b8b6282349e79fb998c2ee2 perm [DeDPikhto = rus =][13.07.15 20:19:04] Fill nickname
493 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][16.07.15 14:55:22] Sabotage
494 d0ee5caf1b8b6282349e79fb998c2ee2 perm [S.W.A.T][16.07.15 14:58:30] Sabotage
495 d0ee5caf1b8b6282349e79fb998c2ee2 2405 5[S.W.A.T][19.07.15 09:46:52] insulting the player
496 d0ee5caf1b8b6282349e79fb998c2ee2 perm [Rembowest14][19.07.15 11:30:45] Sabotage
497 d0ee5caf1b8b6282349e79fb998c2ee2 25718 [S.W.A.T][20.07.15 13:29:55] Sabotage

IP Bans:
[#] [IP Address] [Minutes left] [Reason]
----------------------------------------------
365 212.90.39.93    perm Admin Ban TK ���??���� �� �����
")]
        public void List_Test(string message)
        {
            var serverMessage = new ServerMessage(0, message);
            var recognizer = new BanListRecognizer();
            Assert.IsTrue(recognizer.CanRecognize(serverMessage));
        }


        [Test]
        [TestCase(@"
GUID Bans:
[#] [GUID] [Minutes left] [Reason]
----------------------------------------
0  d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill, destruction of equipment.Ban perm!
1  d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment, TeamKill.Ban perm
2  d0ee5caf1b8b6282349e79fb998c2ee2 perm GameHack.Ban perm
3  d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill.Ban perm
4  d0ee5caf1b8b6282349e79fb9d98c2ee2 perm destruction of equipment.Ban perm
5  d0ee5caf1b8b6282349e79fb998c2ee2 perm Teamkill.Ban perm
6  d0ee5caf1b8b6282349e79fb98c2ee2 perm TeamKill.Ban perm
7  d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill.Ban perm
8  d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill.Ban perm
9  d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment.Ban perm
10 d0ee5caf1b8b6282349e79fb998c2ee2 perm TeamKill.Ban perm
11 d0ee5caf1b8b6282349e79fb998c2ee2 perm destruction of equipment.Ban perm
12 d0ee5caf1b8b6282349e79fb998c2ee2 perm Troll.Ban perm
")]
        public void List_NOT_CORECT_Test(string message)
        {
            var serverMessage = new ServerMessage(0, message);
            var recognizer = new BanListRecognizer();
            Assert.IsFalse(recognizer.CanRecognize(serverMessage));
        }
    }
}