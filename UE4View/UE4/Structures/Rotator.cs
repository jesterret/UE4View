using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE4View.UE4.Structures
{
    class Rotator : UStruct
    {
        public override void Serialize(FArchive reader, FPropertyTag tag)
        {
            var Pitch = reader.ToFloat();
            var Roll = reader.ToFloat();
            var Yaw = reader.ToFloat();
            Value = $"Rotator [Pitch={Pitch}, Roll={Roll}, Yaw={Yaw}]";
        }
    }
}
