using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UE4View.UE4.Pak
{
    public class FPakFile
    {
        public FPakFile(string FileName)
        {
            if (File.Exists(FileName))
            {
                OriginalFileName = Path.GetFileName(FileName);
                CreationTime = File.GetCreationTime(FileName);
                AccessTime = File.GetLastAccessTime(FileName);
                WriteTime = File.GetLastWriteTime(FileName);
                stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                if(stream != null)
                {
                    ReadPakInfo();
                    if (Info.Magic == FPakInfo.PakFile_Magic && Info.bEncryptedIndex == false)
                    {
                        ReadPakIndex();
                    }
                }
            }
        }

        private byte[] ReadStreamData(long Offset, long Size)
        {
            if (stream != null)
            {
                var data = new byte[Size];
                int streamoffset = 0;
                stream.Seek(Offset, Offset < 0 ? SeekOrigin.End : SeekOrigin.Begin);
                do
                {
                    streamoffset += stream.Read(data, streamoffset, (int)Size);
                    if (streamoffset == 0)
                        break;
                } while (streamoffset != Size);

                return data;
            }
            return null;
        }

        private void ReadPakInfo()
        {
            var InfoSize = FPakInfo.GetSerializedSize();
            var InfoData = ReadStreamData(-InfoSize, InfoSize);
            stream.Seek(0, SeekOrigin.Begin);
            Info = new FPakInfo(InfoData);
        }
        private void ReadPakIndex()
        {
            var IndexData = ReadStreamData(Info.IndexOffset, Info.IndexSize);
            var reader = new FArchive(IndexData);
            reader.Version = Info.Version;
            MountPoint = reader.ToFString();
            int NumEntries = reader.ToInt32();
            for (int i = 0; i < NumEntries; i++)
            {
                string FileName = reader.ToFString();
                var entry = new FPakEntry();
                entry.Serialize(reader);
                AbsoluteIndex.Add(FileName, entry);
                Index.Add(FileName, entry);
            }
            return;
        }

        public byte[] ReadEntry(object entry)
        {
            return ReadEntry(entry as FPakEntry);
        }
        public byte[] ReadEntry(FPakEntry entry)
        {
            if (entry != null)
            {
                if(entry.CompressionMethod == FPakEntry.ECompressionFlags.COMPRESS_None)
                    return ReadStreamData(entry.Offset, entry.Size);
                else
                {
                    var decompressed = new byte[entry.UncompressedSize];
                    int Index = 0;
                    int offset = 0;
                    foreach(var block in entry.CompressionBlocks)
                    {
                        var CompressedBlockSize = block.CompressionEnd - block.CompressionStart;
                        var UncompressedBlockSize = Math.Min(entry.UncompressedSize - entry.CompressionBlockSize*Index, entry.CompressionBlockSize);
                        var data = ReadStreamData(block.CompressionStart, CompressedBlockSize);

                        if (entry.CompressionMethod == FPakEntry.ECompressionFlags.COMPRESS_ZLIB)
                        {
                            using (var compressed = new MemoryStream(data, 2, data.Length - 2)) // skip 2 bytes for zlib specification
                            {
                                using (var decstream = new System.IO.Compression.DeflateStream(compressed, System.IO.Compression.CompressionMode.Decompress))
                                {
                                    offset += decstream.Read(decompressed, offset, (int)UncompressedBlockSize);
                                }
                            }
                        }
                        // TODO: Test if GZip decompresses fine.
                        else if (entry.CompressionMethod == FPakEntry.ECompressionFlags.COMPRESS_GZIP)
                        {
                            using (var compressed = new MemoryStream(data))
                            {
                                using (var decstream = new System.IO.Compression.GZipStream(compressed, System.IO.Compression.CompressionMode.Decompress))
                                {
                                    offset += decstream.Read(decompressed, offset, (int)UncompressedBlockSize);
                                }
                            }
                        }
                        Index++;
                    }
                    return decompressed;
                }
            }
            return null;
        }

        /// <summary>
        ///  Retrieves asset contents by its full path. Supports wildcard '*'.
        /// </summary>
        /// <param name="name">Path to asset</param>
        /// <returns>Requested asset content as byte array if found. Null otherwise</returns>
        public byte[] ReadEntryByName(string name)
        {
            var pathparts = name.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            return ReadEntry(RecursiveSearch(pathparts, Index));
        }
        public byte[] ReadEntryByAbsoluteName(string name)
        {
            if (AbsoluteIndex.ContainsKey(name))
                return ReadEntry(AbsoluteIndex[name]);
            else
                return null;
        }

        private FPakEntry RecursiveSearch(string[] parts, FFileIndex dir)
        {
            if (parts.Length == 1)
            {
                // final path element
                if (parts[0] != "*")
                {
                    return dir.Files
                        .Where(kv => kv.Key == parts[0])
                        .Select(kv => kv.Value)
                        .SingleOrDefault();
                }
                

            }
            else if (parts[0] == "*")
            {
                foreach (var kv in dir.Directories)
                {
                    var found = RecursiveSearch(parts.Skip(1).ToArray(), kv.Value);
                    if (found != null)
                        return found;
                }
            }
            else
            {
                var idx = dir.Directories
                    .Where(kv => kv.Key == parts[0])
                    .Select(kv => kv.Value)
                    .SingleOrDefault();
                return RecursiveSearch(parts.Skip(1).ToArray(), idx);
            }
            return null;
        }

        public void Close()
        {
            stream.Close();
        }
        public string OriginalFileName { get; private set; }

        public FPakInfo Info { get; private set; }
        public FFileIndex Index { get; } = new FFileIndex();
        public Dictionary<string, FPakEntry> AbsoluteIndex { get; } = new Dictionary<string, FPakEntry>();
        public string MountPoint { get; private set; }

        public DateTime CreationTime { get; private set; }
        public DateTime AccessTime { get; private set; }
        public DateTime WriteTime { get; private set; }

        internal FileStream stream = null;
    }
}
