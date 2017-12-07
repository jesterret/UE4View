using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    class ObjectProperty : UProperty
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag = null)
        {
            var index = reader.ToInt32();
            if (reader is UAsset.UAsset asset)
                Value = asset.ImpExp(index);
            else
                Value = index;

            return reader;
        }
    }
}
