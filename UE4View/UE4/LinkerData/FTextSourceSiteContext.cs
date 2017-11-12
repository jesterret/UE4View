namespace UE4View.UE4
{
    class FTextSourceSiteContext : USerializable
    {
        public string KeyName;
        public string SiteDescription;
        public bool IsEditorOnly;
        public bool IsOptional;
        public FLocMetadataObject InfoMetaData = new FLocMetadataObject();
        public FLocMetadataObject KeyMetaData = new FLocMetadataObject();
        public override FArchive Serialize(FArchive reader)
        {
            KeyName = reader.ToFString();
            SiteDescription = reader.ToFString();
            IsEditorOnly = reader.ToBoolean();
            IsOptional = reader.ToBoolean();
            InfoMetaData.Serialize(reader);
            KeyMetaData.Serialize(reader);
            return reader;
        }
    }
}
