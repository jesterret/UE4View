using System.Collections.Generic;
using System.Linq;

namespace UE4View.UE4
{
    class FLocMetadataObject : USerializable
    {
        Dictionary<string, FLocMetadataValue> Values = new Dictionary<string, FLocMetadataValue>();
        public override FArchive Serialize(FArchive reader)
        {
            var count = reader.ToInt32();
            foreach(var i in Enumerable.Range(0, count))
            {
                string Key = reader.ToFString();
                var value = new FLocMetadataValue();
                value.Serialize(reader);
                Values.Add(Key, value);
            }
            return reader;
        }
    }
}
