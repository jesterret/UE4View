using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Structures
{
    class LinearColor : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag)
        {
            var r = reader.ToFloat();
            var g = reader.ToFloat();
            var b = reader.ToFloat();
            var a = reader.ToFloat();
            Value = $"Linear Color [A={a}, R={r}, G={g}, B={b}]";
        }
    }
}
