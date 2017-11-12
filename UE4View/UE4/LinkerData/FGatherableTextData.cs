using System.Text;

namespace UE4View.UE4
{
    class FGatherableTextData : USerializable
    {
        public string NamespaceName;
        public FTextSourceData SourceData = new FTextSourceData();
        public FTextSourceSiteContext[] SourceSiteContexts;

        public override FArchive Serialize(FArchive reader)
        {
            NamespaceName = reader.ToFString(Encoding.UTF8);
            SourceData.Serialize(reader);
            SourceSiteContexts = reader.ToArray<FTextSourceSiteContext>();
            return reader;
        }
    }
}
