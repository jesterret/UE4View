using System;
using System.Collections.Generic;
using System.IO;

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

            MountPoint = reader.ToFString();
            int NumEntries = reader.ToInt32();
            for (int i = 0; i < NumEntries; i++)
            {
                string FileName = reader.ToFString();
                var entry = new FPakEntry(reader.ToByteArray((int)FPakEntry.GetSerializedSize(Info.Version)), Info.Version);
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
            // For some reason, actual data is 0x35 bytes after the read offset, need to investigate what's going on
            if (entry != null)
                return ReadStreamData(entry.Offset + 0x35, entry.Size);

            return null;
        }
        public void Close()
        {
            stream.Close();
        }
        public byte[] ReadEntryByAbsoluteName(string name)
        {
            if (AbsoluteIndex.ContainsKey(name))
                return ReadEntry(AbsoluteIndex[name]);
            else
                return null;
        }

        public string OriginalFileName { get; private set; }

        public FPakInfo Info { get; private set; }
        public FFileIndex Index { get; } = new FFileIndex();
        public Dictionary<string, FPakEntry> AbsoluteIndex = new Dictionary<string, FPakEntry>();
        public string MountPoint { get; private set; }

        public DateTime CreationTime { get; private set; }
        public DateTime AccessTime { get; private set; }
        public DateTime WriteTime { get; private set; }

        internal FileStream stream = null;
    }
}
