using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.PropertyTypes
{
    class ObjectProperty : UProperty<int>
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag = null)
        {
            Value = reader.ToInt32();
            if (Value > 0)
                Debugger.Break();

            return reader;
        }
    }
}
