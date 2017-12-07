using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.NativeStructs
{
    class LinearColor : UStruct
    {
        public override FArchive Serialize(FArchive reader, FPropertyTag tag)
        {
            float a, r, g, b;
            r = reader.ToFloat()*255;
            g = reader.ToFloat()*255;
            b = reader.ToFloat()*255;
            a = reader.ToFloat()*255;
            Value = System.Drawing.Color.FromArgb((int)a, (int)r, (int)g, (int)b);
            return reader;
        }
    }
}
