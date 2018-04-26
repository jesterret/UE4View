using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Structures
{
    class Color : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag)
        {
            var r = reader.ToByte();
            var g = reader.ToByte();
            var b = reader.ToByte();
            var a = reader.ToByte();
            Value = $"Color [A={a}, R={r}, G={g}, B={b}"; ;
        }
    }
}
