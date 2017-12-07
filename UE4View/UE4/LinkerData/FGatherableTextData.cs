using System.Collections.Generic;
using System.Text;

namespace UE4View.UE4
{
    class FGatherableTextData : USerializable
    {
        public string NamespaceName;
        public FTextSourceData SourceData = new FTextSourceData();
        public List<FTextSourceSiteContext> SourceSiteContexts;

        public override FArchive Serialize(FArchive reader)
        {
            NamespaceName = reader.ToFString();
            SourceData.Serialize(reader);
            SourceSiteContexts = reader.ToArray<FTextSourceSiteContext>();
            return reader;
        }
    }
}
