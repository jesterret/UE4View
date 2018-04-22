using System.Collections.Generic;
using System.Linq;

namespace UE4View.UE4
{
    class FLocMetadataObject : USerializable
    {
        Dictionary<string, FLocMetadataValue> Values = new Dictionary<string, FLocMetadataValue>();

        public FLocMetadataObject(FArchive reader) : base(reader)
        {
        }

        public override void Serialize(FArchive reader)
        {
            var count = reader.ToInt32();
            foreach(var i in Enumerable.Range(0, count))
            {
                string Key = reader.ToFString();
                var value = new FLocMetadataValue(reader);
                Values.Add(Key, value);
            }
        }
    }
}
