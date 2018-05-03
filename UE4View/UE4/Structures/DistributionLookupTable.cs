using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Structures
{
    class DistributionLookupTable : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            //var Op = reader.ToByte();
            //var EntryCount = reader.ToByte();
            //var EntryStride = reader.ToByte();
            //var SubEntryStride = reader.ToByte();
            //var TimeScale = reader.ToFloat();
            //var TimeBias = reader.ToFloat();
            //var Values = new PropertyTypes.ArrayProperty<PropertyTypes.FloatProperty>();
            //Values.Serialize(reader, null);
            //var LockFlag = reader.ToByte();
            base.Serialize(reader, tag);
        }
    }
}
