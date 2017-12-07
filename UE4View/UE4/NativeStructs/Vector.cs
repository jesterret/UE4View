using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.NativeStructs
{
    class Vector : UStruct
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag)
        {
            float X, Y, Z;
            X = reader.ToFloat();
            Y = reader.ToFloat();
            Z = reader.ToFloat();
            Value = string.Format("{{ {0}, {1}, {2} }}", X, Y, Z);
            return reader;
        }
    }
}
