namespace UE4View.UE4
{
    class FTextSourceData : USerializable
    {
        public string SourceString;
        public FLocMetadataObject SourceStringMetaData;

        public FTextSourceData(FArchive reader) : base(reader)
        {
        }

        public override void Serialize(FArchive reader)
        {
            SourceString = reader.ToFString();
            SourceStringMetaData = new FLocMetadataObject(reader);
        }
    }
}
