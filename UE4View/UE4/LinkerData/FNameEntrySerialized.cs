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
            Name = reader.ToFString();

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
