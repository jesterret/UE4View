using System;
using System.Collections.Generic;
using System.Linq;
using UE4View.UE4.VorbisBank.Xml;

namespace UE4View.UE4.VorbisBank
{

    // TODO: Cleanup the code, it's really messy because I didn't believe it would be that simple.
    class BankFile
    {
        int offset;
        byte[] Buffer;

        public int Magic;
        public int HeaderLength;
        public int Version;
        public uint SoundBankId;

        public int DIDXMagic;
        public int ChunkLength;

        public class BankTrack
        {
            public uint Id;
            public int Offset;
            public int Length;
            public string Name;
        }

        public List<BankTrack> Files;

        BankFile()
        {
            offset = 0;
            // Buffer = Data;
        }
        public BankFile(byte[] Data, SoundBankInfo info = null) : this()
        {
            Buffer = Data;

            Magic = ToInt32();
            if (Magic == 0x44484B42)
            {
                HeaderLength = ToInt32();
                var HeaderEnd = Tell();
                Version = ToInt32();
                SoundBankId = ToUInt32();

                Seek(HeaderEnd + HeaderLength);

                DIDXMagic = ToInt32();
                if (DIDXMagic == 0x58444944)
                {
                    ChunkLength = ToInt32();
                    var filecount = ChunkLength / 12;
                    Files = new List<BankTrack>(filecount);
                    foreach (var i in Enumerable.Range(0, filecount))
                    {
                        var trackId = ToUInt32();
                        Files.Add(new BankTrack()
                        {
                            Id = trackId,
                            Offset = ToInt32(),
                            Length = ToInt32(),
                            Name = info != null ? info.GetTrackName(SoundBankId, trackId) : trackId.ToString() + ".wem"
                        });
                    }
                }

                var DataMagic = ToInt32();
                var DataLen = ToInt32();
            }
        }
        public byte[] ReadTrack(BankTrack track)
        {
            var data = new byte[track.Length];
            Array.Copy(Buffer, offset + track.Offset, data, 0, track.Length);
            return data;
        }
        public int ToInt32()
        {
            var val = BitConverter.ToInt32(Buffer, offset);
            offset += sizeof(int);
            return val;
        }
        public uint ToUInt32()
        {
            var val = BitConverter.ToUInt32(Buffer, offset);
            offset += sizeof(uint);
            return val;
        }

        public BankFile Skip(int len)
        {
            offset += len;
            return this;
        }
        public BankFile Seek(int off)
        {
            offset = off;
            return this;
        }
        public int Tell()
        {
            return offset;
        }
        public int Length()
        {
            return Buffer.Length;
        }
    }
}
