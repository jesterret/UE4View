using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.NativeStructs
{
    class Vector : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag)
        {
            float X = reader.ToFloat();
            float Y = reader.ToFloat();
            float Z = reader.ToFloat();
            Value = $"Vector [X={X}, Y={Y}, Z={Z}]";
        }
    }
}
