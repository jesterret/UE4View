using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.NativeStructs
{
    class LinearColor : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag)
        {
            var r = reader.ToFloat()*255;
            var g = reader.ToFloat()*255;
            var b = reader.ToFloat()*255;
            var a = reader.ToFloat()*255;
            Value = System.Drawing.Color.FromArgb((int)a, (int)r, (int)g, (int)b);
        }
    }
}
