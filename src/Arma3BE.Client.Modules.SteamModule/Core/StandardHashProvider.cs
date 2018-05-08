using System;
using System.Security.Cryptography;

namespace Arma3BE.Client.Modules.SteamModule.Core
{
    public class StandardHashProvider : IMd5Provider
    {
        private static readonly byte[] Parts = { 0x42, 0x45, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();


        public uint ComputeUIntHash(uint input)
        {
            var hash = ComputeByteHash(input);
            return BitConverter.ToUInt32(hash, 0);
        }

        public byte[] ComputeByteHash(uint input)
        {
            Parts[0] = 0x42;
            Parts[1] = 0x45;

            Parts[2] = (byte)(input << 24 >> 24);
            Parts[3] = (byte)(input << 16 >> 24);
            Parts[4] = (byte)(input << 8 >> 24);
            Parts[5] = (byte)(input << 0 >> 24);

            Parts[6] = 1;
            Parts[7] = 0;
            Parts[8] = 16;
            Parts[9] = 1;

            var hash = Md5.ComputeHash(Parts);
            return hash;
        }

        public byte[] ComputeByteHash(long input)
        {
            Parts[0] = 0x42;
            Parts[1] = 0x45;

            for (var i = 2; i < 10; i++)
            {
                var res = input % 256;
                input = input / 256;
                Parts[i] = (byte)res;
            }

            var hash = Md5.ComputeHash(Parts);
            return hash;
        }
    }
}