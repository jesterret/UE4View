using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography;

namespace UE4View.UE4.Pak
{
    public class FPakFile
    {
        public FPakFile(string FileName)
        {
            FilePath = Path.GetDirectoryName(FileName);
            if (File.Exists(FileName))
            {
                CreationTime = File.GetCreationTime(FileName);
                AccessTime = File.GetLastAccessTime(FileName);
                WriteTime = File.GetLastWriteTime(FileName);
                OriginalFileName = Path.GetFileName(FileName);

                stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                if(stream != null)
                    Initialize();
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

        private void Initialize()
        {
            var InfoSize = FPakInfo.GetSerializedSize();
            var InfoData = ReadStreamData(-InfoSize, InfoSize);
            Info.Serialize(InfoData);

            if (Info.Magic == FPakInfo.PakFile_Magic)
                LoadIndex();
        }
        private void LoadIndex()
        {
            var IndexData = ReadStreamData(Info.IndexOffset, Info.IndexSize);
            if (Info.bEncryptedIndex != 0)
                DecryptData(ref IndexData);

            using (var reader = new FArchive(IndexData))
            {
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
            }
        }

        private void DecryptData(ref byte[] data)
        {
            using (var rijndael = new RijndaelManaged())
            {
                rijndael.Mode = CipherMode.ECB;
                rijndael.Padding = PaddingMode.Zeros;
                rijndael.IV = new byte[16];
                rijndael.BlockSize = 128;

                // TODO: This is a "Darwin Project" encryption key. Change this to a dictionary of encryption keys and match them with the specific games
                rijndael.Key = System.Text.Encoding.UTF8.GetBytes("e3VqgSMhuaPw75fm0PdGZCN3ASwpVOk5Ij7iLf8VOEdqGL6aw05JeX0RHMgBvypd").Take(32).ToArray(); // Darwin
                // rijndael.Key = System.Text.Encoding.UTF8.GetBytes("y298qjSb115NqQ3Agad30DWn2QYrTI8CT6aP05l2PBV9Qe92S94PdoVCCy06A38L").Take(32).ToArray(); // Fortnite

                using (var crypt = rijndael.CreateDecryptor())
                {
                    using (MemoryStream msDecrypt = new MemoryStream(data))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, crypt, CryptoStreamMode.Read))
                        {
                            csDecrypt.CopyTo(new MemoryStream(data));
                        }
                    }
                }
            }
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
                return dir.Files
                    .Where(kv => new WildcardPattern(parts[0]).IsMatch(kv.Key))
                    .Select(kv => kv.Value)
                    .SingleOrDefault();
            }
            else
            {
                var dirs = dir.Directories
                    .Where(kv => new WildcardPattern(parts[0]).IsMatch(kv.Key))
                    .Select(kv => kv.Value);

                foreach(var idx in dirs)
                {
                    var found = RecursiveSearch(parts.Skip(1).ToArray(), idx);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        public void Close()
        {
            stream.Close();
        }
        public string OriginalFileName { get; private set; }
        public string FilePath { get; private set; }

        public FPakInfo Info { get; } = new FPakInfo();
        public FFileIndex Index { get; } = new FFileIndex();
        public Dictionary<string, FPakEntry> AbsoluteIndex { get; } = new Dictionary<string, FPakEntry>();
        public string MountPoint { get; private set; }

        public DateTime CreationTime { get; private set; }
        public DateTime AccessTime { get; private set; }
        public DateTime WriteTime { get; private set; }

        internal FileStream stream = null;
    }
}
