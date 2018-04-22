using System.Collections.Generic;
using System.Text;

namespace UE4View.UE4
{
    class FGatherableTextData : USerializable
    {
        public string NamespaceName;
        public FTextSourceData SourceData;
        public List<FTextSourceSiteContext> SourceSiteContexts;

        public FGatherableTextData(FArchive reader) : base(reader)
        {
        }

        public override void Serialize(FArchive reader)
        {
            NamespaceName = reader.ToFString();
            SourceData = new FTextSourceData(reader);
            SourceSiteContexts = reader.ToArray<FTextSourceSiteContext>();
        }
    }
}
