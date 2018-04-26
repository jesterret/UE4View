using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    class UInt64Property : UProperty
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            Value = reader.ToUInt64();
        }
    }
}
