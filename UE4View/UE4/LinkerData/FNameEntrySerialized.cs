using System.Text;

namespace UE4View.UE4
{
    public class FNameEntrySerialized : USerializable
    {
        public string Name;
        public ushort NonCasePreservingHash;
        public ushort CasePreservingHash;
        public override FArchive Serialize(FArchive reader)
        {
            var len = reader.ToInt32();
            // Negative length denotes UTF16
            if (len < 0)
            {
                len = -len;
                Name = reader.ToFString(Encoding.Unicode, len*2);
            }
            else
                Name = reader.ToFString(Encoding.UTF8, len);

            // Not implemented for older versions
            if (reader.Version >= (int)ObjectVersion.EUnrealEngineObjectUE4Version.VER_UE4_NAME_HASHES_SERIALIZED)
            {
                NonCasePreservingHash = reader.ToUInt16();
                CasePreservingHash = reader.ToUInt16();
            }

            return reader;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
