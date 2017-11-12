using System;

namespace UE4View.UE4.Pak
{
    public class FPakInfo
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
        public bool bEncryptedIndex;

        public static long GetSerializedSize()
        {
            return 45;
        }

        public FPakInfo(byte[] bytes)
        {
            if (bytes != null && bytes.Length < GetSerializedSize())
            {
                Magic = 0;
                return;
            }

            bEncryptedIndex = BitConverter.ToBoolean(bytes, 0);
            Magic = BitConverter.ToUInt32(bytes, 1);
            Version = BitConverter.ToInt32(bytes, 5);
            IndexOffset = BitConverter.ToInt64(bytes, 9);
            IndexSize = BitConverter.ToInt64(bytes, 17);
            Array.Copy(bytes, 25, IndexHash, 0, 20);

            if (Version < (int)PakVersion.IndexEncryption)
                bEncryptedIndex = false;
        }
    }
}
