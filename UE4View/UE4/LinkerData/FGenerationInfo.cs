namespace UE4View.UE4
{
    public class FGenerationInfo : USerializable
    {
        public int ExportCount;
        public int NameCount;

        public FGenerationInfo(FArchive reader) : base(reader)
        {
        }

        public override void Serialize(FArchive reader)
        {
            ExportCount = reader.ToInt32();
            NameCount = reader.ToInt32();
        }
    }
}
