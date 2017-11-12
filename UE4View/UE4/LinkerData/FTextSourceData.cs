namespace UE4View.UE4
{
    class FTextSourceData : USerializable
    {
        public string SourceString;
        public FLocMetadataObject SourceStringMetaData = new FLocMetadataObject();
        public override FArchive Serialize(FArchive reader)
        {
            SourceString = reader.ToFString();
            SourceStringMetaData.Serialize(reader);
            return reader;
        }
    }
}
