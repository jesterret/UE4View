using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Structures
{
    class Quat : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag = null)
        {
            float X = reader.ToFloat();
            float Y = reader.ToFloat();
            float Z = reader.ToFloat();
            float W = reader.ToFloat();
            Value = $"Quat [X={X}, Y={Y}, Z={Z}, W={W}]";
        }
    }
}
