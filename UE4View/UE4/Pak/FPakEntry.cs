using System.Collections.Generic;

namespace UE4View.UE4.Pak
{
    public class FPakEntry
    {
        internal class FPakCompressedBlock
        {
            long CompressionStart;
            long CompressionEnd;

            public FPakCompressedBlock(byte[] bytes)
            {
                var x = new FArchive(bytes);
                CompressionStart = x.ToInt64();
                CompressionEnd = x.ToInt64();
            }
        }
        /** Offset into pak file where the file is stored.*/
        public long Offset;
        /** Serialized file size. */
        public long Size;
        /** Uncompressed file size. */
        public long UncompressedSize;
        /** Compression method. */
        int CompressionMethod;
        /** File SHA1 value. */
        byte[] Hash = new byte[20];
        /** Array of compression blocks that describe how to decompress this pak entry. */
        List<FPakCompressedBlock> CompressionBlocks;
        /** Size of a compressed block in the file. */
        uint CompressionBlockSize;
        /** True is file is encrypted. */
        public byte bEncrypted;

        public FPakEntry(byte[] bytes, int Version)
        {
            if (bytes != null && bytes.Length != GetSerializedSize(Version))
            {
                Offset = 0;
                Size = 0;
                return;
            }

            var reader = new FArchive(bytes);
            Offset = reader.ToInt64();
            Size = reader.ToInt64();
            UncompressedSize = reader.ToInt64();
            CompressionMethod = reader.ToInt32();
            if(Version <= (int)FPakInfo.PakVersion.Initial)
            {
                reader.ToInt64();
            }
            reader.ToByteArray(20).CopyTo(Hash, 0);
            if(Version >= (int)FPakInfo.PakVersion.CompressionEncryption)
            {
                if (CompressionMethod != 0)
                {
                    CompressionBlocks = new List<FPakCompressedBlock>(reader.ToInt32());
                    for (int i = 0; i < CompressionBlocks.Capacity; i++)
                        CompressionBlocks.Add(new FPakCompressedBlock(reader.ToByteArray(sizeof(long) * 2)));
                }
                bEncrypted = reader.ToByte();
                CompressionBlockSize = reader.ToUInt32();
            }
            return;
        }

        public static long GetSerializedSize(int Version)
        {
            long SerializedSize = sizeof(long) + sizeof(long) + sizeof(long) + sizeof(int) + 20;
            if(Version >= (int)FPakInfo.PakVersion.CompressionEncryption)
            {
                SerializedSize += sizeof(byte) + sizeof(uint);
            }
            if(Version < (int)FPakInfo.PakVersion.NoTimestamps)
            {
                SerializedSize += sizeof(long);
            }
            return SerializedSize;
        }
    }
}
