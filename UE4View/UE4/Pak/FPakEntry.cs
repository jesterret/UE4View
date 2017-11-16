using System.Collections.Generic;

namespace UE4View.UE4.Pak
{
    public class FPakEntry : USerializable
    {
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

            public override FArchive Serialize(FArchive reader)
            {
                CompressionStart = reader.ToInt64();
                CompressionEnd = reader.ToInt64();
                return reader;
            }
        }
        /** Offset into pak file where the file is stored.*/
        public long Offset;
        /** Serialized file size. */
        public long Size;
        /** Uncompressed file size. */
        public long UncompressedSize;
        /** Compression method. */
        public ECompressionFlags CompressionMethod;
        /** File SHA1 value. */
        byte[] Hash = new byte[20];
        /** Array of compression blocks that describe how to decompress this pak entry. */
        public List<FPakCompressedBlock> CompressionBlocks;
        /** Size of a compressed block in the file. */
        public uint CompressionBlockSize;
        /** True is file is encrypted. */
        public byte bEncrypted;

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

        public override FArchive Serialize(FArchive reader)
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

                bEncrypted = reader.ToByte();
                CompressionBlockSize = reader.ToUInt32();
            }
            return reader;
        }
    }
}
