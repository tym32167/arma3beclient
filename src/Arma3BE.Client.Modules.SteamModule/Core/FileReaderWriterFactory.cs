using System.IO;

namespace Arma3BE.Client.Modules.SteamModule.Core
{
    public static class FileReaderWriterFactory
    {
        private const int BufferLen = 100 * 1024 * 1024;

        public static BufferedStream Buffered(this Stream source, int bufferLen = BufferLen)
        {
            return new BufferedStream(source, bufferLen);
        }

        public static BinaryWriter ToBinaryWriter(this Stream stream)
        {
            return new BinaryWriter(stream);
        }

        public static BinaryReader ToBinaryReader(this Stream stream)
        {
            return new BinaryReader(stream);
        }

        public static FileStream CreateReader(this string src)
        {
            return File.OpenRead(src);
        }

        public static FileStream CreateWriter(this string src)
        {
            return File.OpenWrite(src);
        }
    }
}
