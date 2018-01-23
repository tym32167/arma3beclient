using Arma3BEClient.Common.Extensions;
using Arma3BEClient.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Arma3BE.Client.Modules.SteamModule.Core
{
    public class Uint32ToTiont32AllHashesFileGenerator
    {
        private readonly ILog _log;
        private readonly IMd5ProviderFactory _md5ProviderFactory;

        public Uint32ToTiont32AllHashesFileGenerator(IMd5ProviderFactory md5ProviderFactory, ILog log)
        {
            _md5ProviderFactory = md5ProviderFactory;
            _log = log;
        }

        public void GenerateFile(string folder, IProgress<int> progress, CancellationToken cancellationToken)
        {
            var step1Folder = Path.Combine(folder, "step1");
            Step1_Generate256UnsortedFiles(step1Folder, progress, cancellationToken);

            if (cancellationToken.IsCancellationRequested) return;

            Step2_GenerateIndexAndIdsFile(step1Folder, folder, progress, cancellationToken);
            Directory.Delete(step1Folder, true);
        }

        private void Step1_Generate256UnsortedFiles(string folder, IProgress<int> progress, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Timing: 0:10:17
            using (_log.Time(nameof(Step1_Generate256UnsortedFiles)))
            {
                var writers = new BinaryWriter[256];

                try
                {
                    for (var i = 0; i < writers.Length; i++)
                    {
                        if (cancellationToken.IsCancellationRequested) return;

                        var fname = $"{i}.bin";
                        var path = Path.Combine(folder, fname);

                        if (File.Exists(path))
                            File.Delete(path);

                        writers[i] = path.CreateWriter().Buffered(1 * 1024 * 1024).ToBinaryWriter();
                    }

                    var tasks = new List<Task>();
                    long total = 0;

                    foreach (var tuple in ForEach(Math.Min(4, Environment.ProcessorCount)))
                    {
                        if (cancellationToken.IsCancellationRequested) return;
                        var local = tuple;
                        tasks.Add(Task.Factory.StartNew(() =>
                        {
                            var md5Provicer = _md5ProviderFactory.Create();
                            for (uint i = local.Item1; i <= local.Item2; i++)
                            {
                                if (cancellationToken.IsCancellationRequested) return;

                                var hash = md5Provicer.ComputeByteHash(i);
                                var writer = writers[hash[0]];

                                lock (writer)
                                {
                                    writer.Write(hash, 0, 4);
                                    writer.Write(i);
                                }

                                Interlocked.Increment(ref total);

                                if ((total + 1) / 100000000 > total / 100000000)
                                {
                                    var percent = total * 50.0 / uint.MaxValue;
                                    progress.Report((int)percent);
                                }

                                if (i == uint.MaxValue) break;
                            }
                        }, TaskCreationOptions.LongRunning));
                    }

                    Task.WaitAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    throw;
                }
                finally
                {
                    foreach (var writer in writers)
                        writer.Dispose();
                }
            }
        }

        private void Step2_GenerateIndexAndIdsFile(string step1Folder, string step5Folder, IProgress<int> progress, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(step5Folder))
                Directory.CreateDirectory(step5Folder);

            // Timing: 0:15:33
            using (_log.Time(nameof(Step2_GenerateIndexAndIdsFile)))
            {
                var idsFile = Path.Combine(step5Folder, "ids.bin");
                if (File.Exists(idsFile))
                    File.Delete(idsFile);

                using (var idsWriter = idsFile.CreateWriter().Buffered(10 * 1024 * 1024))
                {
                    var indexFileName = Path.Combine(step5Folder, "index.bin");
                    if (File.Exists(indexFileName))
                        File.Delete(indexFileName);

                    var fileIndexes = new uint[256 * 256 * 256];
                    var filebuffer = new byte[256 * 256 * 256 * 8 + 1024 * 1024 * 2];
                    var tempBuffer = new byte[256 * 256 * 256 * 8 + 1024 * 1024 * 2];

                    var indexes = new uint[256 * 256 * 256];

                    for (var i = 0; i < 256; i++)
                    {
                        if (cancellationToken.IsCancellationRequested) return;

                        var fileName = $"{i}.bin";
                        var inputFile = Path.Combine(step1Folder, fileName);

                        _log.Info($"Processing {fileName}");

                        int inputLen;

                        using (var sr = inputFile.CreateReader())
                        {
                            inputLen = sr.Read(filebuffer, 0, filebuffer.Length);
                        }

                        CreateMathes(filebuffer, inputLen, indexes);

                        Clear(fileIndexes);
                        CreateMathes(filebuffer, inputLen, fileIndexes, 1);
                        CreateIndexes(fileIndexes);
                        SortByHash(filebuffer, tempBuffer, inputLen, fileIndexes, 1);

                        for (var j = 0; j < inputLen; j += 8)
                        {
                            if (cancellationToken.IsCancellationRequested) return;
                            idsWriter.Write(tempBuffer, j + 4, 4);
                        }

                        progress.Report((int)(50.0 * i / 256 + 50));
                    }

                    CreateIndexes(indexes, 1);

                    using (var indexWriter = indexFileName.CreateWriter().Buffered(10 * 1024 * 1024)
                        .ToBinaryWriter())
                    {
                        for (var i = 0; i < 256; i++)
                            for (var ii = 0; ii < 256; ii++)
                                for (var jj = 0; jj < 256; jj++)
                                {
                                    if (cancellationToken.IsCancellationRequested) return;
                                    var ind = 256 * 256 * i + 256 * ii + jj;
                                    indexWriter.Write(indexes[ind]);
                                }
                    }
                }
            }
        }

        private static void Clear(uint[] source)
        {
            for (var i = 0; i < source.Length; i++)
                source[i] = 0;
        }

        internal static void CreateIndexes(uint[] indexes, uint wordLen = 8)
        {
            var curPos = 0u;
            for (var j = 0; j < indexes.Length; j++)
            {
                var count = indexes[j];
                indexes[j] = curPos * wordLen;
                curPos += count;
            }
        }

        internal static void CreateMathes(byte[] inputBytes, int inputLen, uint[] indexes, int baseoffset = 0)
        {
            var rowCount = inputLen / 8;

            for (var j = 0; j < rowCount; j++)
            {
                var inpInd = j * 8;

                var i1 = inputBytes[inpInd + baseoffset];
                var i2 = inputBytes[inpInd + 1 + baseoffset];
                var i3 = inputBytes[inpInd + 2 + baseoffset];

                indexes[i1 * 256 * 256 + i2 * 256 + i3]++;
            }
        }

        internal static void SortByHash(byte[] inputBytes, byte[] tempBuffer, int inputLen, uint[] indexes, int baseoffset = 0)
        {
            var rowCount = inputLen / 8;
            for (uint j = 0; j < rowCount; j++)
            {
                var inpInd = j * 8;

                var i1 = inputBytes[inpInd + baseoffset];
                var i2 = inputBytes[inpInd + 1 + baseoffset];
                var i3 = inputBytes[inpInd + 2 + baseoffset];

                var nextInd = indexes[i1 * 256 * 256 + i2 * 256 + i3];
                Copy(inputBytes, tempBuffer, inpInd, nextInd);
                indexes[i1 * 256 * 256 + i2 * 256 + i3] += 8;
            }
        }

        private static void Copy(byte[] source, byte[] dest, uint i, uint j)
        {
            for (var k = 0; k < 8; k++)
            {
                var index0 = i + k;
                var index1 = j + k;

                if (index0 >= source.Length) break;
                if (index1 >= source.Length) break;

                dest[index1] = source[index0];
            }
        }

        private static IEnumerable<Tuple<uint, uint>> ForEach(int parts)
        {
            var step = (uint)(uint.MaxValue / parts);
            var cmin = 0u;
            var cmax = cmin + step;

            for (var i = 0; i < parts; i++)
            {
                yield return Tuple.Create(cmin, cmax);
                cmin = cmax + 1;
                cmax += step;
                if (i == parts - 2) cmax = uint.MaxValue;
            }
        }
    }
}