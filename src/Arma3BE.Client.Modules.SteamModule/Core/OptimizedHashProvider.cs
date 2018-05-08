using System;

namespace Arma3BE.Client.Modules.SteamModule.Core
{
    public class OptimizedHashProviderFactory : IMd5ProviderFactory
    {
        public IMd5Provider Create()
        {
            return new OptimizedHashProvider();
        }
    }


    public class OptimizedHashProvider : IMd5Provider
    {
        private readonly UInt32[] _buffer = new uint[4];

        public uint ComputeUIntHash(uint input)
        {
            ComputeByteHash(_buffer, input);
            return _buffer[0];
        }

        public byte[] ComputeByteHash(uint input)
        {
            ComputeByteHash(_buffer, input);
            return BitConverter.GetBytes(_buffer[0]);
        }

        long lmin = 0x0110000100000000;
        public byte[] ComputeByteHash(long input)
        {
            var uintInput = (uint)(input - lmin);
            ComputeByteHash(_buffer, uintInput);

            var result = new byte[16];
            for (var i = 0; i < 4; i++)
            {
                var bytes = BitConverter.GetBytes(_buffer[i]);
                for (var j = 0; j < 4; j++)
                {
                    result[i * 4 + j] = bytes[j];
                }
            }
            return result;
        }


        // Taken from https://ru.stackoverflow.com/a/768244/179763

        private readonly UInt32[] _block = { 0, 0, 0x00800110, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x50, 0 };
        //              [10]=0x80 [9]=1, [8]=16
        private void ComputeByteHash(UInt32[] state, uint input)
        {
            UInt32 a = state[0] = 0x67452301;
            UInt32 b = state[1] = 0xEFCDAB89;
            UInt32 c = state[2] = 0x98BADCFE;
            UInt32 d = state[3] = 0x10325476;
            UInt32 x;

            _block[0] = 0x4542 | ((input << 16) & 0xFFFF0000);
            _block[1] = 0x00010000 | ((input >> 16) & 0xFFFF);

            x = (a + (d ^ (b & (c ^ d))) + 0xD76AA478 + _block[0]); a = ((x << 7) | (x >> (32 - 7))) + b;
            x = (d + (c ^ (a & (b ^ c))) + 0xE8C7B756 + _block[1]); d = ((x << 12) | (x >> (32 - 12))) + a;
            x = (c + (b ^ (d & (a ^ b))) + 0x242070DB + _block[2]); c = ((x << 17) | (x >> (32 - 17))) + d;
            x = (b + (a ^ (c & (d ^ a))) + 0xC1BDCEEE + _block[3]); b = ((x << 22) | (x >> (32 - 22))) + c;
            x = (a + (d ^ (b & (c ^ d))) + 0xF57C0FAF + _block[4]); a = ((x << 7) | (x >> (32 - 7))) + b;
            x = (d + (c ^ (a & (b ^ c))) + 0x4787C62A + _block[5]); d = ((x << 12) | (x >> (32 - 12))) + a;
            x = (c + (b ^ (d & (a ^ b))) + 0xA8304613 + _block[6]); c = ((x << 17) | (x >> (32 - 17))) + d;
            x = (b + (a ^ (c & (d ^ a))) + 0xFD469501 + _block[7]); b = ((x << 22) | (x >> (32 - 22))) + c;
            x = (a + (d ^ (b & (c ^ d))) + 0x698098D8 + _block[8]); a = ((x << 7) | (x >> (32 - 7))) + b;
            x = (d + (c ^ (a & (b ^ c))) + 0x8B44F7AF + _block[9]); d = ((x << 12) | (x >> (32 - 12))) + a;
            x = (c + (b ^ (d & (a ^ b))) + 0xFFFF5BB1 + _block[10]); c = ((x << 17) | (x >> (32 - 17))) + d;
            x = (b + (a ^ (c & (d ^ a))) + 0x895CD7BE + _block[11]); b = ((x << 22) | (x >> (32 - 22))) + c;
            x = (a + (d ^ (b & (c ^ d))) + 0x6B901122 + _block[12]); a = ((x << 7) | (x >> (32 - 7))) + b;
            x = (d + (c ^ (a & (b ^ c))) + 0xFD987193 + _block[13]); d = ((x << 12) | (x >> (32 - 12))) + a;
            x = (c + (b ^ (d & (a ^ b))) + 0xA679438E + _block[14]); c = ((x << 17) | (x >> (32 - 17))) + d;
            x = (b + (a ^ (c & (d ^ a))) + 0x49B40821 + _block[15]); b = ((x << 22) | (x >> (32 - 22))) + c;
            //Раунд 1
            x = (a + (c ^ (d & (b ^ c))) + 0xF61E2562 + _block[1]); a = ((x << 5) | (x >> (32 - 5))) + b;
            x = (d + (b ^ (c & (a ^ b))) + 0xC040B340 + _block[6]); d = ((x << 9) | (x >> (32 - 9))) + a;
            x = (c + (a ^ (b & (d ^ a))) + 0x265E5A51 + _block[11]); c = ((x << 14) | (x >> (32 - 14))) + d;
            x = (b + (d ^ (a & (c ^ d))) + 0xE9B6C7AA + _block[0]); b = ((x << 20) | (x >> (32 - 20))) + c;
            x = (a + (c ^ (d & (b ^ c))) + 0xD62F105D + _block[5]); a = ((x << 5) | (x >> (32 - 5))) + b;
            x = (d + (b ^ (c & (a ^ b))) + 0x02441453 + _block[10]); d = ((x << 9) | (x >> (32 - 9))) + a;
            x = (c + (a ^ (b & (d ^ a))) + 0xD8A1E681 + _block[15]); c = ((x << 14) | (x >> (32 - 14))) + d;
            x = (b + (d ^ (a & (c ^ d))) + 0xE7D3FBC8 + _block[4]); b = ((x << 20) | (x >> (32 - 20))) + c;
            x = (a + (c ^ (d & (b ^ c))) + 0x21E1CDE6 + _block[9]); a = ((x << 5) | (x >> (32 - 5))) + b;
            x = (d + (b ^ (c & (a ^ b))) + 0xC33707D6 + _block[14]); d = ((x << 9) | (x >> (32 - 9))) + a;
            x = (c + (a ^ (b & (d ^ a))) + 0xF4D50D87 + _block[3]); c = ((x << 14) | (x >> (32 - 14))) + d;
            x = (b + (d ^ (a & (c ^ d))) + 0x455A14ED + _block[8]); b = ((x << 20) | (x >> (32 - 20))) + c;
            x = (a + (c ^ (d & (b ^ c))) + 0xA9E3E905 + _block[13]); a = ((x << 5) | (x >> (32 - 5))) + b;
            x = (d + (b ^ (c & (a ^ b))) + 0xFCEFA3F8 + _block[2]); d = ((x << 9) | (x >> (32 - 9))) + a;
            x = (c + (a ^ (b & (d ^ a))) + 0x676F02D9 + _block[7]); c = ((x << 14) | (x >> (32 - 14))) + d;
            x = (b + (d ^ (a & (c ^ d))) + 0x8D2A4C8A + _block[12]); b = ((x << 20) | (x >> (32 - 20))) + c;
            //Раунд 2
            x = (a + (b ^ c ^ d) + 0xFFFA3942 + _block[5]); a = ((x << 4) | (x >> (32 - 4))) + b;
            x = (d + (a ^ b ^ c) + 0x8771F681 + _block[8]); d = ((x << 11) | (x >> (32 - 11))) + a;
            x = (c + (d ^ a ^ b) + 0x6D9D6122 + _block[11]); c = ((x << 16) | (x >> (32 - 16))) + d;
            x = (b + (c ^ d ^ a) + 0xFDE5380C + _block[14]); b = ((x << 23) | (x >> (32 - 23))) + c;
            x = (a + (b ^ c ^ d) + 0xA4BEEA44 + _block[1]); a = ((x << 4) | (x >> (32 - 4))) + b;
            x = (d + (a ^ b ^ c) + 0x4BDECFA9 + _block[4]); d = ((x << 11) | (x >> (32 - 11))) + a;
            x = (c + (d ^ a ^ b) + 0xF6BB4B60 + _block[7]); c = ((x << 16) | (x >> (32 - 16))) + d;
            x = (b + (c ^ d ^ a) + 0xBEBFBC70 + _block[10]); b = ((x << 23) | (x >> (32 - 23))) + c;
            x = (a + (b ^ c ^ d) + 0x289B7EC6 + _block[13]); a = ((x << 4) | (x >> (32 - 4))) + b;
            x = (d + (a ^ b ^ c) + 0xEAA127FA + _block[0]); d = ((x << 11) | (x >> (32 - 11))) + a;
            x = (c + (d ^ a ^ b) + 0xD4EF3085 + _block[3]); c = ((x << 16) | (x >> (32 - 16))) + d;
            x = (b + (c ^ d ^ a) + 0x04881D05 + _block[6]); b = ((x << 23) | (x >> (32 - 23))) + c;
            x = (a + (b ^ c ^ d) + 0xD9D4D039 + _block[9]); a = ((x << 4) | (x >> (32 - 4))) + b;
            x = (d + (a ^ b ^ c) + 0xE6DB99E5 + _block[12]); d = ((x << 11) | (x >> (32 - 11))) + a;
            x = (c + (d ^ a ^ b) + 0x1FA27CF8 + _block[15]); c = ((x << 16) | (x >> (32 - 16))) + d;
            x = (b + (c ^ d ^ a) + 0xC4AC5665 + _block[2]); b = ((x << 23) | (x >> (32 - 23))) + c;
            //Раунд 3
            x = (a + (c ^ (b | ~d)) + 0xF4292244 + _block[0]); a = ((x << 6) | (x >> (32 - 6))) + b;
            x = (d + (b ^ (a | ~c)) + 0x432AFF97 + _block[7]); d = ((x << 10) | (x >> (32 - 10))) + a;
            x = (c + (a ^ (d | ~b)) + 0xAB9423A7 + _block[14]); c = ((x << 15) | (x >> (32 - 15))) + d;
            x = (b + (d ^ (c | ~a)) + 0xFC93A039 + _block[5]); b = ((x << 21) | (x >> (32 - 21))) + c;
            x = (a + (c ^ (b | ~d)) + 0x655B59C3 + _block[12]); a = ((x << 6) | (x >> (32 - 6))) + b;
            x = (d + (b ^ (a | ~c)) + 0x8F0CCC92 + _block[3]); d = ((x << 10) | (x >> (32 - 10))) + a;
            x = (c + (a ^ (d | ~b)) + 0xFFEFF47D + _block[10]); c = ((x << 15) | (x >> (32 - 15))) + d;
            x = (b + (d ^ (c | ~a)) + 0x85845DD1 + _block[1]); b = ((x << 21) | (x >> (32 - 21))) + c;
            x = (a + (c ^ (b | ~d)) + 0x6FA87E4F + _block[8]); a = ((x << 6) | (x >> (32 - 6))) + b;
            x = (d + (b ^ (a | ~c)) + 0xFE2CE6E0 + _block[15]); d = ((x << 10) | (x >> (32 - 10))) + a;
            x = (c + (a ^ (d | ~b)) + 0xA3014314 + _block[6]); c = ((x << 15) | (x >> (32 - 15))) + d;
            x = (b + (d ^ (c | ~a)) + 0x4E0811A1 + _block[13]); b = ((x << 21) | (x >> (32 - 21))) + c;
            x = (a + (c ^ (b | ~d)) + 0xF7537E82 + _block[4]); a = ((x << 6) | (x >> (32 - 6))) + b;
            x = (d + (b ^ (a | ~c)) + 0xBD3AF235 + _block[11]); d = ((x << 10) | (x >> (32 - 10))) + a;
            x = (c + (a ^ (d | ~b)) + 0x2AD7D2BB + _block[2]); c = ((x << 15) | (x >> (32 - 15))) + d;
            x = (b + (d ^ (c | ~a)) + 0xEB86D391 + _block[9]); b = ((x << 21) | (x >> (32 - 21))) + c;

            state[0] = state[0] + a;
            state[1] = state[1] + b;
            state[2] = state[2] + c;
            state[3] = state[3] + d;

        }
    }
}