using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.NativeStructs
{
    class Color : UStruct
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag)
        {
            var r = reader.ToByte();
            var g = reader.ToByte();
            var b = reader.ToByte();
            var a = reader.ToByte();
            Value = System.Drawing.Color.FromArgb(a, r, g, b);
            return reader;
        }
    }
}
