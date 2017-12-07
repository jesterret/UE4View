using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    class BoolProperty : UProperty
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag = null)
        {
            Value = tag.BoolVal != 0;
            return reader;
        }
    }
}
