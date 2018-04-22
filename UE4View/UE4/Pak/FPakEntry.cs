using System.Collections.Generic;
using System.Linq;

namespace UE4View.UE4.Pak
{
    public class FPakEntry : USerializable
    {
        public FPakEntry(FArchive reader) : base(reader)
        {
        }
        public enum ECompressionFlags
        {
            /** No compression																*/
            COMPRESS_None = 0x00,
            /** Compress with ZLIB															*/
            COMPRESS_ZLIB = 0x01,
            /** Compress with GZIP															*/
            COMPRESS_GZIP = 0x02,
            /** Prefer compression that compresses smaller (ONLY VALID FOR COMPRESSION)		*/
            COMPRESS_BiasMemory = 0x10,
            /** Prefer compression that compresses faster (ONLY VALID FOR COMPRESSION)		*/
            COMPRESS_BiasSpeed = 0x20,
        };
        public class FPakCompressedBlock : USerializable
        {
            public long CompressionStart;
            public long CompressionEnd;

            public override void Serialize(FArchive reader)
            {
                CompressionStart = reader.ToInt64();
                CompressionEnd = reader.ToInt64();
            }
        }
        /** Offset into pak file where the file is stored.*/
        public long Offset { get; private set; }
        /** Serialized file size. */
        public long Size { get; private set; }
        /** Uncompressed file size. */
        public long UncompressedSize { get; private set; }
        /** Compression method. */
        public ECompressionFlags CompressionMethod { get; private set; }
        /** File SHA1 value. */
        byte[] Hash { get; } = new byte[20];
        /** Array of compression blocks that describe how to decompress this pak entry. */
        public List<FPakCompressedBlock> CompressionBlocks { get; private set; }
        /** Size of a compressed block in the file. */
        public uint CompressionBlockSize { get; private set; }
        /** True is file is encrypted. */
        public byte Encrypted { get; private set; }

        public long GetSerializedSize(int version)
        {
            long SerializedSize = sizeof(long) + sizeof(long) + sizeof(long) + sizeof(int) + 20;
            if(version >= (int)FPakInfo.PakVersion.CompressionEncryption)
            {
                SerializedSize += sizeof(byte) + sizeof(uint);
                if(CompressionMethod != ECompressionFlags.COMPRESS_None)
                {
                    // Size += sizeof(FPakCompressedBlock) * BlockCount + ArrayProperty -> Count
                    SerializedSize += (sizeof(long) * 2) * CompressionBlocks.Count + sizeof(int);
                }
            }
            if(version < (int)FPakInfo.PakVersion.NoTimestamps)
            {
                SerializedSize += sizeof(long);
            }
            return SerializedSize;
        }

        public override void Serialize(FArchive reader)
        {
            Offset = reader.ToInt64() + GetSerializedSize(reader.Version); // Offset + PakHeader -> OffsetInPak
            Size = reader.ToInt64();
            UncompressedSize = reader.ToInt64();
            CompressionMethod = (ECompressionFlags)reader.ToInt32();
            if (reader.Version <= (int)FPakInfo.PakVersion.Initial)
            {
                reader.ToInt64(); // FDateTime Timestamp
            }
            reader.ToByteArray(20).CopyTo(Hash, 0);
            if (reader.Version >= (int)FPakInfo.PakVersion.CompressionEncryption)
            {
                if (CompressionMethod != ECompressionFlags.COMPRESS_None)
                    CompressionBlocks = reader.ToArray<FPakCompressedBlock>();

                Encrypted = reader.ToByte();
                CompressionBlockSize = reader.ToUInt32();
            }
        }

        public string GetSha1()
        {
            return string.Join(string.Empty, Hash.Select(b => b.ToString("x2")).ToArray());
        }
    }
}
