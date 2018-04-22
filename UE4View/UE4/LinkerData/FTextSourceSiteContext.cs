namespace UE4View.UE4
{
    class FTextSourceSiteContext : USerializable
    {
        public string KeyName;
        public string SiteDescription;
        public bool IsEditorOnly;
        public bool IsOptional;
        public FLocMetadataObject InfoMetaData;
        public FLocMetadataObject KeyMetaData;
        public override void Serialize(FArchive reader)
        {
            KeyName = reader.ToFString();
            SiteDescription = reader.ToFString();
            IsEditorOnly = reader.ToBoolean();
            IsOptional = reader.ToBoolean();
            InfoMetaData = new FLocMetadataObject(reader);
            KeyMetaData = new FLocMetadataObject(reader);
        }
    }
}
