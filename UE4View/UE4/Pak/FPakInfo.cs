using System;

namespace UE4View.UE4.Pak
{
    public class FPakInfo : USerializable
    {
        /** Magic number to use in header */
        public const long PakFile_Magic = 0x5A6F12E1;
        /** Size of cached data. */
        const long MaxChunkDataSize = 64 * 1024;

        /** Version numbers. */
        public enum PakVersion
        {
            Initial = 1,
            NoTimestamps = 2,
            CompressionEncryption = 3,
            IndexEncryption = 4,

            Latest = IndexEncryption
        };

        /** Pak file magic value. */
        public uint Magic;
        /** Pak file version. */
        public int Version;
        /** Offset to pak file index. */
        public long IndexOffset;
        /** Size (in bytes) of pak file index. */
        public long IndexSize;
        /** Index SHA1 value. */
        public byte[] IndexHash = new byte[20];
        /** Flag indicating if the pak index has been encrypted. */
        public byte bEncryptedIndex;

        public static long GetSerializedSize()
        {
            return 45;
        }

        public override FArchive Serialize(FArchive reader)
        {
            bEncryptedIndex = reader.ToByte();
            Magic = reader.ToUInt32();
            Version = reader.ToInt32();
            IndexOffset = reader.ToInt64();
            IndexSize = reader.ToInt64();
            IndexHash = reader.ToByteArray(20);
            
            if (Version < (int)PakVersion.IndexEncryption)
                bEncryptedIndex = 0;

            return reader;
        }
    }
}
