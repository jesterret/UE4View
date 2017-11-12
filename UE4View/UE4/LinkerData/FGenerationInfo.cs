namespace UE4View.UE4
{
    public class FGenerationInfo : USerializable
    {
        public int ExportCount;
        public int NameCount;

        public override FArchive Serialize(FArchive reader)
        {
            ExportCount = reader.ToInt32();
            NameCount = reader.ToInt32();
            return reader;
        }
    }
}
