using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.NativeStructs
{
    class IntPoint : UStruct
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag)
        {
            var x = reader.ToInt32();
            var y = reader.ToInt32();
            Value = string.Format("{{ X={0}, Y={1} }}", x, y);
            return reader;
        }
    }
}
